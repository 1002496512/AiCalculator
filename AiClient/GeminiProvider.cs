using AiClient;
using Google.GenAI;
using Google.GenAI.Types;
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

        public async Task<string> GetResponseAsync1(List<ChatMessage> history)
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
                    if (firstCandidate.Content != null &&
                        firstCandidate.Content.Parts != null &&
                        firstCandidate.Content.Parts.Count > 0)
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

        public async Task<string> GetResponseAsync(List<ChatMessage> history)
        {
            try
            {
                // 1. יצירת רשימה של אובייקטי Content עבור גוגל
                List<Content> googleContents = new List<Content>();

                // 2. מעבר על ההיסטוריה שלנו והמרה לפורמט של גוגל
                foreach (ChatMessage msg in history)
                {
                    Google.GenAI.Types.Content chatContent = new Google.GenAI.Types.Content();

                    // הגדרת התפקיד (Role)
                    if (msg.Role == ChatRole.User)
                    {
                        chatContent.Role = "user";
                    }
                    else
                    {
                        chatContent.Role = "model";
                    }

                    // הוספת הטקסט
                    Part textPart = new Part();
                    textPart.Text = msg.Content;
                    chatContent.Parts.Add(textPart);

                    googleContents.Add(chatContent);
                }

                // 3. שליחת הבקשה - כאן אנחנו מעבירים את שם המודל ואת רשימת התכנים
                // הפעולה מקבלת את שם המודל כפרמטר ראשון ואת ה-Contents כפרמטר שני
                var response = await _client.Models.GenerateContentAsync(_modelName, googleContents);

                // 4. שליפת התשובה (בדיקות Null מפורשות)
                if (response != null && response.Candidates != null && response.Candidates.Count > 0)
                {
                    var candidate = response.Candidates[0];
                    if (candidate.Content != null && candidate.Content.Parts != null && candidate.Content.Parts.Count > 0)
                    {
                        return candidate.Content.Parts[0].Text;
                    }
                }

                return "המודל לא החזיר תשובה.";
            }
            catch (Exception ex)
            {
                return "שגיאת מערכת: " + ex.Message;
            }
        }
    }
}
