using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RPGProject.Data
{
    public static class JsonDatabaseLoader
    {
        /// <summary>
        /// Carrega um JSON genérico em uma lista de objetos, tentando múltiplos caminhos possíveis.
        /// </summary>
        public static List<T> LoadJsonList<T>(string relativePath, string databaseName = "Database")
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory ?? Directory.GetCurrentDirectory();

            var candidates = new List<string>()
            {
                Path.Combine(baseDir, relativePath),
                Path.Combine(baseDir, "Data", Path.GetFileName(relativePath)),
                Path.Combine(baseDir, Path.GetFileName(relativePath)),
                Path.Combine(Directory.GetCurrentDirectory(), relativePath),
                Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(relativePath))
            }.Distinct().ToList();

            Debug.WriteLine($"[{databaseName}] BaseDirectory = {baseDir}");
            Debug.WriteLine($"[{databaseName}] Tentando localizar o JSON:");
            foreach (var c in candidates) Debug.WriteLine("  -> " + c);

            string found = string.Empty;
            foreach (var c in candidates)
            {
                if (File.Exists(c))
                {
                    found = c;
                    break;
                }

                var dir = Path.GetDirectoryName(c) ?? baseDir;
                var fileName = Path.GetFileName(c);
                if (Directory.Exists(dir))
                {
                    var matched = Directory.EnumerateFiles(dir)
                                           .FirstOrDefault(f => string.Equals(Path.GetFileName(f), fileName, StringComparison.OrdinalIgnoreCase));
                    if (matched != null)
                    {
                        found = matched;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(found))
            {
                Debug.WriteLine($"\n[{databaseName}] Arquivo não encontrado. Conteúdo do diretório base:");
                try
                {
                    foreach (var f in Directory.GetFiles(baseDir))
                        Debug.WriteLine("  * " + Path.GetFileName(f));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("  Erro listando baseDir: " + ex.Message);
                }

                var dataDir = Path.Combine(baseDir, "Data");
                Debug.WriteLine($"\n[{databaseName}] Conteúdo de '{dataDir}' (se existir):");
                if (Directory.Exists(dataDir))
                {
                    foreach (var f in Directory.GetFiles(dataDir))
                        Debug.WriteLine("  * " + Path.GetFileName(f));
                }
                else
                {
                    Debug.WriteLine("  (pasta Data não existe em baseDir)");
                }

                throw new FileNotFoundException($"O arquivo JSON não foi encontrado. Locais tentados: {string.Join(", ", candidates)}");
            }

            Debug.WriteLine($"[{databaseName}] Carregando arquivo: {found}");
            string json = File.ReadAllText(found);
            var list = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            Debug.WriteLine($"[{databaseName}] {list.Count} registros carregados.");

            return list;
        }
    }
}