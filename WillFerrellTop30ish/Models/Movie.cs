using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.ComponentModel.DataAnnotations;

namespace WillFerrellTop30ish.Models
{
    public class Movie
    {
        public string ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [AllowHtml]
        public string XmlData { get; set; }
        [Required]
        public int Stock { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}