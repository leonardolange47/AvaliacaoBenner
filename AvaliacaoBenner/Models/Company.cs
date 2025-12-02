using System;
using System.Collections.Generic;
using System.Text;

namespace AvaliacaoBenner.Models
{
    public class Company
    {
        public string Cnpj { get; set; } = "";
        public string Name { get; set; } = "";
        public string Phone { get; set; } = "";
        public List<Document> Documents { get; set; } = new();
    }
}
