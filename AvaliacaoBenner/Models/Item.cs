using System;
using System.Collections.Generic;
using System.Text;

namespace AvaliacaoBenner.Models
{
    public class Item
    {
        public int? ItemNumber { get; set; } = null;
        public string Description { get; set; } = "";
        public decimal Value { get; set; }
        public List<Category>? Categories { get; set; }
    }
}
