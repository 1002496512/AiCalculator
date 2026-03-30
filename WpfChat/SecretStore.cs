using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfChat
{
   
        internal static class SecretStore
        {
        private static DirectoryInfo dir = new DirectoryInfo(AppContext.BaseDirectory);
        private static string dir1 = Directory.GetCurrentDirectory();
        // Store per-user data in LocalApplicationData and keep the file name the same.
        // This makes the location more appropriate for user-specific app data and avoids
        // writing to the working directory of the process.
        private static readonly string _folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AiCalculator", "WpfChat", "SecretData");
            private static readonly string _file = Path.Combine(_folder, "secrets.json");
        public static void SaveApiKey(string apiKey)
            {
                try
                {
                    Directory.CreateDirectory(_folder);
                    var doc = new { ApiKey = apiKey };
                    var json = JsonSerializer.Serialize(doc);
                    File.WriteAllText(_file, json);
                }
                catch
                {
                    // ignore errors
                }
            }

            public static string? GetApiKey()
            {
                // Prefer an environment variable (e.g. loaded from a .env during development)
                var env = Environment.GetEnvironmentVariable("API_KEY");
                if (!string.IsNullOrEmpty(env)) return env;

                try
                {
                    if (!File.Exists(_file)) return null;
                    var json = File.ReadAllText(_file);
                    var doc = JsonSerializer.Deserialize<JsonElement>(json);
                    if (doc.TryGetProperty("ApiKey", out var v))
                        return v.GetString();
                    return null;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
