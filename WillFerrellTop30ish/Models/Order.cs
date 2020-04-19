using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WillFerrellTop30ish.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> OrderItems { get; set; }

    }
}