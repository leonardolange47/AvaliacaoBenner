using AvaliacaoBenner.Models;
using AvaliacaoBenner.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AvaliacaoDotnet.ConsoleApp1
{
    internal class Program
    {
        private static string configPath = "consoleconfig.json";

        static void Main(string[] args)
        {
            var config = LoadConfig();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Gerador de Arquivo (Avaliação Dotnet) ===");
                Console.WriteLine("1) Configurar arquivo .json (base de dados)");
                Console.WriteLine($"   Atual: {config.JsonPath}");
                Console.WriteLine("2) Configurar diretório de output");
                Console.WriteLine($"   Atual: {config.OutputFolder}");
                Console.WriteLine("3) Gerar arquivo");
                Console.WriteLine("0) Sair");
                Console.Write("Escolha: ");
                var key = Console.ReadLine();

                if (key == "0") break;
                if (key == "1")
                {
                    Console.Write("Caminho do arquivo JSON: ");
                    var p = Console.ReadLine();
                    config.JsonPath = p;
                    SaveConfig(config);
                }
                else if (key == "2")
                {
                    Console.Write("Caminho diretório de output: ");
                    var p = Console.ReadLine();
                    config.OutputFolder = p;
                    SaveConfig(config);
                }
                else if (key == "3")
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(config.JsonPath) || !File.Exists(config.JsonPath))
                        {
                            Console.WriteLine("Arquivo JSON inválido ou não configurado. Pressione Enter.");
                            Console.ReadLine();
                            continue;
                        }

                        Console.Write("Número do leiaute a gerar (ex: 1 ou 2): ");
                        if (!int.TryParse(Console.ReadLine(), out var version))
                        {
                            Console.WriteLine("Versão inválida. Pressione Enter.");
                            Console.ReadLine();
                            continue;
                        }

                        var jsonText = File.ReadAllText(config.JsonPath);
                        var companies = JsonSerializer.Deserialize<List<Company>>(jsonText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (companies == null)
                        {
                            Console.WriteLine("Erro ao ler JSON. Pressione Enter.");
                            Console.ReadLine();
                            continue;
                        }

                        var generatorSvc = new FileGeneratorService();
                        var outPath = generatorSvc.GenerateFile(companies, version, config.OutputFolder);
                        Console.WriteLine($"Arquivo gerado em: {outPath}");
                        Console.WriteLine("Pressione Enter.");
                        Console.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erro: " + ex.Message);
                        Console.WriteLine("Pressione Enter.");
                        Console.ReadLine();
                    }
                }
            }
        }

        private static AppConfig LoadConfig()
        {
            if (!File.Exists(configPath)) return new AppConfig();
            try
            {
                var txt = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<AppConfig>(txt) ?? new AppConfig();
            }
            catch { return new AppConfig(); }
        }

        private static void SaveConfig(AppConfig cfg)
        {
            var txt = JsonSerializer.Serialize(cfg, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configPath, txt);
        }

        private class AppConfig
        {
            public string JsonPath { get; set; } = "";
            public string OutputFolder { get; set; } = "";
        }
    }
}
