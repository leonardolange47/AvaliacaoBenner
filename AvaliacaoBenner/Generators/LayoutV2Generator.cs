using AvaliacaoBenner.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaliacaoBenner.Generators
{
    public class LayoutV2Generator : ILayoutGenerator
    {
        public LayoutResult Generate(IEnumerable<Company> companies)
        {
            var result = new LayoutResult();
            void Inc(string key) => result.Counts[key] = result.Counts.TryGetValue(key, out var v) ? v + 1 : 1;

            foreach (var company in companies)
            {
                result.Lines.Add($"00|{company.Cnpj}|{company.Name}|{company.Phone}");
                Inc("00");

                foreach (var doc in company.Documents)
                {
                    result.Lines.Add($"01|{doc.Model}|{doc.Number}|{doc.Value:F2}");
                    Inc("01");

                    foreach (var item in doc.Items)
                    {
                        var itemNumber = item.ItemNumber.HasValue ? item.ItemNumber.Value.ToString() : "";
                        result.Lines.Add($"02|{itemNumber}|{item.Description}|{item.Value:F2}");
                        Inc("02");

                        if (item.Categories != null)
                        {
                            foreach (var c in item.Categories)
                            {
                                result.Lines.Add($"03|{c.Number}|{c.Description}");
                                Inc("03");
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
