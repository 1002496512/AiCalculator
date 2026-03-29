namespace AiClient
{
    public interface IChatService
    {
        Task<string> GetResponseAsync(List<ChatMessage> history);
    }
}
