using AiClient;

namespace Test
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string apiKey = "";
            GeminiMetadataService geminiMetadataService = new GeminiMetadataService(apiKey);
            await geminiMetadataService.LoadModelsAsync();
            List<string> models = geminiMetadataService.AvailableModels;
            foreach (string model in models)
            { 
                Console.WriteLine(model);       
            }
            Console.ReadLine();

        }
    }
}
