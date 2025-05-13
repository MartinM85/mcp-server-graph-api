using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using static System.Environment;

namespace McpClient
{
    internal class AzureOpenAIService : AIService
    {
        public AzureOpenAIService(IList<McpClientTool> tools)
        {
            // Retrieve the OpenAI endpoint from environment variables
            var endpoint = GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? GetOpenAIEndpoint();
            if (string.IsNullOrEmpty(endpoint))
            {
                Console.WriteLine("Please set the AZURE_OPENAI_ENDPOINT environment variable.");
                return;
            }

            var key = GetEnvironmentVariable("AZURE_OPENAI_KEY") ?? GetOpenAIApiKey();
            if (string.IsNullOrEmpty(key))
            {
                Console.WriteLine("Please set the AZURE_OPENAI_KEY environment variable.");
                return;
            }

            var credential = new AzureKeyCredential(key);

            // Initialize the AzureOpenAIClient
            var azureClient = new AzureOpenAIClient(new Uri(endpoint), credential);
            _client = azureClient.GetOpenAIResponseClient("<deployment_name>")
                .AsIChatClient()
                .AsBuilder()
                .UseFunctionInvocation()
                .Build();

            _options = new ChatOptions
            {
                Temperature = (float)0.7,
                MaxOutputTokens = 800,
                TopP = (float)0.95,
                FrequencyPenalty = (float)0,
                PresencePenalty = (float)0,
                Tools = [.. tools],
                ToolMode = ChatToolMode.RequireAny
            };
        }

        private static string GetOpenAIEndpoint()
        {
            return "https://<your_service_name>.openai.azure.com";
        }

        private static string GetOpenAIApiKey()
        {
            return "<open-api-key>";
        }
    }
}
