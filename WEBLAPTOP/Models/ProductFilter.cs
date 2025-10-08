using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBLAPTOP.Models
{
    public class ProductFilter
    {
        public string sort { get; set; } = "default";
        public string display { get; set; } = "default";
        public int? price_start { get; set; }
        public int? price_end {  get; set; }
        public int? categories_id { get; set; }
    }
}