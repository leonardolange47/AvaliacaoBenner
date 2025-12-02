using AvaliacaoBenner.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaliacaoBenner.Generators
{
    public interface ILayoutGenerator
    {
        LayoutResult Generate(IEnumerable<Company> companies);
    }
}
