using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace InventoryAndShipmentManagementSystem
{
    public partial class IASMS : Form
    {
        private HttpClient _client;
        public IASMS()
        {
            InitializeComponent();
            _client = new HttpClient();
            RefreshData();
            GetAllProducts();
        }
        public void RefreshData()
        {
            textProductId.Enabled = false;
            textName.Enabled = false;
            textPrice.Enabled = false;
            textQuantity.Enabled = false;
            textProductId.Text = string.Empty;
            textName.Text = string.Empty;
            textPrice.Text = string.Empty;
            textQuantity.Text = string.Empty;
            Save.Visible = false;
            Update.Visible = false;
            Delete.Visible = false;
            labelError1.Visible=false;
        }
        private async void GetAllProducts()
        {
            var baseAddress = "https://localhost:44350/api/Product/GetAllProducts";
            var jsonstring = await _client.GetStringAsync(baseAddress);
            Response datalist = JsonConvert.DeserializeObject<Response>(jsonstring);
            dataGridView1.DataSource = datalist.ListProducts;
            lblError.Visible = false;
        }

        private void AddProduct_Click(object sender, EventArgs e)
        {
            textProductId.Enabled = false;
            textName.Enabled = true;
            textPrice.Enabled = true;
            textQuantity.Enabled = true;
            Save.Visible = true;
            Update.Visible = false;
            Delete.Visible = false;

            textProductId.Text = string.Empty;
            textName.Text = string.Empty;
            textPrice.Text = string.Empty;
            textQuantity.Text = string.Empty;
        }

        private async void Save_Click(object sender, EventArgs e)
        {
            var baseAddress = "https://localhost:44350/api/Product/AddProduct";
            var newItem = new AddProduct
            {
                ProductName = textName.Text,
                Price = textPrice.Text,
                Quantity = textQuantity.Text
            };

            var json = JsonConvert.SerializeObject(newItem);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(baseAddress, content);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Product added successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshData();
                GetAllProducts(); 
            }
            else
            {               
                var errorResponse = await response.Content.ReadAsStringAsync();
                var validationErrors = JsonConvert.DeserializeObject<Response>(errorResponse);
                if (validationErrors?.Errors != null && validationErrors.Errors.Any())
                {
                    labelError1.Visible = true;
                    string errorMessages = string.Join("\n", validationErrors.Errors.SelectMany(ev => ev.Value));
                    labelError1.Text = $"Validation Errors:\n{errorMessages}";
                }
                else
                {
                    labelError1.Visible = true;
                    labelError1.Text = "Error creating item.";
                }
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedRow = dataGridView1.Rows[e.RowIndex];
                int ProductId = (int)selectedRow.Cells["ProductId"].Value;
                string ProductName = selectedRow.Cells["ProductName"].Value.ToString();
                string Price = selectedRow.Cells["Price"].Value.ToString();
                string Quantity = selectedRow.Cells["Quantity"].Value.ToString();

                textProductId.Text = ProductId.ToString();
                textName.Text = ProductName;
                textPrice.Text = Price;
                textQuantity.Text = Quantity;

                textProductId.Enabled = false;
                textName.Enabled = true;
                textPrice.Enabled = true;
                textQuantity.Enabled = true;
                Save.Visible = false;
                Update.Visible = true;
                Delete.Visible = true;
            }
        }

        private async void Update_Click(object sender, EventArgs e)
        {
            var baseAddress = "https://localhost:44350/api/Product/UpdateProduct";
            var updatedItem = new Product
            {
                ProductId =Convert.ToInt32(textProductId.Text),
                ProductName = textName.Text,
                Price = textPrice.Text,
                Quantity = textQuantity.Text
            };

            var json = JsonConvert.SerializeObject(updatedItem);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(baseAddress, content);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Product updated successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshData();
                GetAllProducts();
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                var validationErrors = JsonConvert.DeserializeObject<Response>(errorResponse);
                if (validationErrors?.Errors != null && validationErrors.Errors.Any())
                {
                    labelError1.Visible = true;
                    string errorMessages = string.Join("\n", validationErrors.Errors.SelectMany(ev => ev.Value));
                    labelError1.Text = $"Validation Errors:\n{errorMessages}";
                }
                else
                {
                    labelError1.Visible = true;
                    labelError1.Text = "Error creating item.";
                }
            }

        }

        private async void Delete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you want to delete it?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                var baseAddress = "https://localhost:44350/api/Product/DeleteProduct";
                var itemId = int.Parse(textProductId.Text);
                var response = await _client.DeleteAsync($"{baseAddress}/{itemId}");
                if (response.IsSuccessStatusCode)
                {
                    RefreshData();
                    GetAllProducts();
                }
                else
                {
                    labelError1.Visible = true;
                    labelError1.Text = "Error when deleting item.";
                }
            }
            else
            {
                return;
            }

        }

       

    }
}
