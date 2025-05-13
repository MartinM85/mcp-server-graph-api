using Microsoft.Extensions.AI;

namespace McpClient
{
    internal abstract class AIService : IDisposable
    {
        protected IChatClient _client;
        protected ChatOptions _options;
        private bool disposedValue;

        public async Task<ChatResponse> GetResponseAsync(string chatMessage)
        {
            return await _client.GetResponseAsync(chatMessage, _options);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _client?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
