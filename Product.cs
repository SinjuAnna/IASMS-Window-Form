using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAndShipmentManagementSystem
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
    }
    public class Response
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<Product> ListProducts { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }  
    }
    public class AddProduct
    {
        public string ProductName { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
    }
    public class ResponseError
    {
        public string type { get; set; }
        public string title { get; set; }
        //public Product Product { get; set; }
        public string status { get; set; }
        public string errors { get; set; }
        public string traceId { get; set; }
        
    }
}
