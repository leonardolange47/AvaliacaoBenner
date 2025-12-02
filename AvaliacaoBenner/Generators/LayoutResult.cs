using System;
using System.Collections.Generic;
using System.Text;

namespace AvaliacaoBenner.Generators
{
    public class LayoutResult
    {
        public List<string> Lines { get; set; } = new();
        public Dictionary<string, int> Counts { get; set; } = new();
    }
}
