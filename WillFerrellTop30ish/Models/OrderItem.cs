using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WillFerrellTop30ish.Controllers;

namespace WillFerrellTop30ish.Models
{
    public class OrderItem
    {
        public Movie ItemOrdered { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}