using Anthropic.SDK;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace McpClient
{
    internal class AnthropicService : AIService
    {
        public AnthropicService(IList<McpClientTool> tools)
        {
            _client = new AnthropicClient(new APIAuthentication(GetAnthropicApiKey()))
                .Messages
                .AsBuilder()
                .UseFunctionInvocation()
                .Build();

            _options = new ChatOptions
            {
                MaxOutputTokens = 1000,
                ModelId = "claude-3-5-sonnet-20241022",
                Tools = [.. tools],
                ToolMode = ChatToolMode.RequireAny
            };
        }

        private static string GetAnthropicApiKey()
        {
            return "<anthropic-api-key>";
        }
    }
}
