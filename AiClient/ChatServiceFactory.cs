using AI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiClient
{
    public static class ChatServiceFactory
    {
        public static IChatService CreateService(string providerType, string apiKey,string modelName)
        {
            switch (providerType)
            {
                case "Gemini":
                    return new GeminiProvider(apiKey, modelName);

                case "OpenAI":
                    // כאן יבוא ה-OpenAIProvider שנממש בהמשך
                    throw new NotImplementedException("OpenAI עדיין לא מוממש");

                default:
                    throw new ArgumentException("ספק ה-AI אינו מוכר: " + providerType);
            }
        }
    }
}
