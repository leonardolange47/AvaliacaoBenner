using System;
using System.Collections.Generic;
using System.Text;

namespace AvaliacaoBenner.Models
{
    public class Document
    {
        public string Model { get; set; } = "";
        public string Number { get; set; } = "";
        public decimal Value { get; set; }
        public List<Item> Items { get; set; } = new();
    }
}
