using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiClient
{
    public class ChatMessage
    {
        public ChatRole Role { get; set; }
        public string Content { get; set; }

        public ChatMessage(ChatRole role, string content)
        {
            Role = role;
            Content = content;
        }
    }
}
