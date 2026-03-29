using AiClient;
using GenerativeAI;
using Google.GenAI;
using Google.GenAI;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace AI.Core
{
    public class GeminiProvider : IChatService
    {
        private readonly Client _client;
        private readonly string _modelName;

        public GeminiProvider(string apiKey, string modelName)
        {
            _client = new Client(apiKey:apiKey);
            _modelName = modelName;
        }

        public async Task<string> GetResponseAsync(List<ChatMessage> history)
        {
            try
            {
                // לוקחים את ההודעה האחרונה מהרשימה
                ChatMessage lastMessage = history[history.Count - 1];
                string prompt = lastMessage.Content;

                // שליחה למודל
                var response = await _client.Models.GenerateContentAsync(_modelName, prompt);

                // בדיקה מקצועית שהתשובה חזרה בצורה תקינה
                if (response != null &&
                    response.Candidates != null &&
                    response.Candidates.Count > 0)
                {
                    var firstCandidate = response.Candidates[0];
                    if (firstCandidate.Content != null && firstCandidate.Content.Parts != null && firstCandidate.Content.Parts.Count > 0)
                    {
                        return firstCandidate.Content.Parts[0].Text;
                    }
                }

                return "שגיאה: המודל החזיר תשובה ריקה.";
            }
            catch (Exception ex)
            {
                return "שגיאת תקשורת: " + ex.Message;
            }
        }
    }
}
