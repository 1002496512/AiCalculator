using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.GenAI;


namespace AiClient
{
    public class GeminiMetadataService
    {
        private readonly Client _client;
        private List<string> _availableModels;

        public GeminiMetadataService(string apiKey)
        {
            _client = new Client(apiKey: apiKey);
            _availableModels = new List<string>();
        }

        // תכונה (Property) שמאפשרת לגשת לרשימה אחרי שהיא נטענה
        public List<string> AvailableModels
        {
            get { return _availableModels; }
        }

        // פעולה לטעינה מה-API
        public async Task LoadModelsAsync()
        {
            _availableModels.Clear();
            try
            {
                // קבלת זרם המודלים
                var response = await _client.Models.ListAsync();

                // מעבר אסינכרוני על המודלים
                await foreach (var model in response)
                {
                    bool supportsChat = false;

                    // בדיקה האם המודל תומך בפעולת יצירת תוכן
                    if (model.SupportedActions != null)
                    {
                        foreach (var action in model.SupportedActions)
                        {
                            // בדרך כלל הערך הוא "generateContent"
                            if (action.ToString() == "generateContent")
                            {
                                supportsChat = true;
                                break;
                            }
                        }
                    }

                    // אם המודל תומך בצאט, נוסיף אותו לרשימה שלנו
                    if (supportsChat)
                    {
                        // ניקוי השם מהתחילית "models/"
                        string cleanName = model.Name.Replace("models/", "");
                        _availableModels.Add(cleanName);
                    }
                }
            }
            catch (Exception ex)
            {
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
