using AvaliacaoBenner.Generators;
using AvaliacaoBenner.Models;
using AvaliacaoBenner.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace AvaliacaoBenner.Test
{
    [TestFixture]
    public class FileGeneratorServiceTests
    {
        private string _tempFolder = "";

        [SetUp]
        public void SetUp()
        {
            _tempFolder = Path.Combine(Path.GetTempPath(), "FileGenTests_" + Guid.NewGuid());
            Directory.CreateDirectory(_tempFolder);
        }

        [TearDown]
        public void TearDown()
        {
            // Limpa override e pasta temporária
            LayoutGeneratorFactory.Override = null;

            if (Directory.Exists(_tempFolder))
                Directory.Delete(_tempFolder, true);
        }

        [Test]
        public void GenerateFile_UsaGeradorFake_e_GeraArquivoComLinhas09e99()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company
                {
                    Cnpj = "1112223330001",
                    Name = "Empresa Teste",
                    Phone = "1234",
                    Documents = new List<Document>
                    {
                        new Document
                        {
                            Model = "NF",
                            Number = "001",
                            Value = 30m,
                            Items = new List<Item>
                            {
                                new Item { Description = "A", Value = 10m },
                                new Item { Description = "B", Value = 20m }
                            }
                        }
                    }
                }
            };

            // Mock do ILayoutGenerator
            var mockGen = new Mock<ILayoutGenerator>();
            var fakeResult = new LayoutResult
            {
                Lines = new List<string>
                {
                    "00|1112223330001|Empresa Teste|1234",
                    "01|NF|001|30.00",
                    "02|A|10.00",
                    "02|B|20.00"
                },
                Counts = new Dictionary<string, int>
                {
                    { "00", 1 }, { "01", 1 }, { "02", 2 }, { "03", 0 }
                }
            };
            mockGen.Setup(g => g.Generate(It.IsAny<IEnumerable<Company>>()))
                   .Returns(fakeResult);

            // Substitui a factory apenas para este teste
            var original = LayoutGeneratorFactory.Override;
            LayoutGeneratorFactory.Override = (v) => mockGen.Object;

            try
            {
                var svc = new FileGeneratorService();

                // Act
                var outPath = svc.GenerateFile(companies, 1, _tempFolder, "saida_test.txt");

                // Assert
                Assert.That(File.Exists(outPath), Is.True, "Arquivo de saída não foi gerado");

                var lines = File.ReadAllLines(outPath).ToList();

                // Verifica conteúdo fake gerado
                Assert.That(lines, Does.Contain("00|1112223330001|Empresa Teste|1234"));
                Assert.That(lines, Does.Contain("01|NF|001|30.00"));

                // Verifica linhas 09 para cada tipo
                Assert.That(lines, Does.Contain("09|00|1"));
                Assert.That(lines, Does.Contain("09|01|1"));
                Assert.That(lines, Does.Contain("09|02|2"));
                // Tipo 03 sem ocorrências deverá ser 0
                Assert.That(lines, Does.Contain("09|03|0"));

                // Linha 99 existe e contém o total
                Assert.That(lines.Any(l => l.StartsWith("99|")), Is.True, "Linha 99 ausente");
            }
            finally
            {
                // Restaura override original
                LayoutGeneratorFactory.Override = original;
            }
        }

        [Test]
        public void GenerateFile_DeveLancarInvalidOperationException_SeSomaItensDiferirDoValorDocumento()
        {
            // Arrange: documento com soma dos itens diferente
            var companies = new List<Company>
            {
                new Company
                {
                    Cnpj = "X",
                    Documents = new List<Document>
                    {
                        new Document
                        {
                            Number = "D1",
                            Value = 50m,
                            Items = new List<Item>
                            {
                                new Item { Description = "I1", Value = 20m },
                                new Item { Description = "I2", Value = 20m }, // soma = 40 != 50
                            }
                        }
                    }
                }
            };

            var svc = new FileGeneratorService();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => svc.GenerateFile(companies, 1, _tempFolder));
            Assert.That(ex.Message, Does.Contain("Validação falhou"));
        }
    }
}
