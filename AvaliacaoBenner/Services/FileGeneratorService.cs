using AvaliacaoBenner.Generators;
using AvaliacaoBenner.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaliacaoBenner.Services
{
    public class FileGeneratorService
    {
        public string GenerateFile(IEnumerable<Company> companies, int layoutVersion, string outputFolder, string? outputFileName = null)
        {
            if (companies == null) throw new ArgumentNullException(nameof(companies));
            if (string.IsNullOrWhiteSpace(outputFolder)) throw new ArgumentNullException(nameof(outputFolder));

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            foreach (var company in companies)
            {
                if (company?.Documents == null) continue;

                foreach (var doc in company.Documents)
                {
                    var sumItems = doc.Items?.Sum(i => i.Value) ?? 0m;
                    if (sumItems != doc.Value)
                    {
                        throw new InvalidOperationException(
                            $"Validação falhou: documento '{doc.Number}' (empresa {company.Cnpj}) valor do documento {doc.Value:F2} != soma dos itens {sumItems:F2}"
                        );
                    }
                }
            }

            var generator = LayoutGeneratorFactory.GetGenerator(layoutVersion);

            var layoutResult = generator.Generate(companies ?? Enumerable.Empty<Company>());

            var lines = new List<string>(layoutResult.Lines ?? Enumerable.Empty<string>());

            var counts = layoutResult.Counts ?? new Dictionary<string, int>();
            var expectedTypes = new[] { "00", "01", "02", "03" };

            foreach (var t in expectedTypes)
            {
                counts.TryGetValue(t, out var qty);
                lines.Add($"09|{t}|{qty}");
            }

            lines.Add($"99|{lines.Count}");

            var fileName = string.IsNullOrWhiteSpace(outputFileName)
                ? $"saida_{DateTime.Now:yyyyMMdd_HHmmss}_v{layoutVersion}.txt"
                : outputFileName;

            var outPath = Path.Combine(outputFolder, fileName);

            File.WriteAllLines(outPath, lines, Encoding.UTF8);

            return outPath;
        }
    }
}
