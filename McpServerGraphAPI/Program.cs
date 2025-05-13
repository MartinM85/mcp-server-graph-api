using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateEmptyApplicationBuilder(settings: null);


        builder.Services.AddMcpServer()
            .WithStdioServerTransport()
            .WithTools<GraphApiTool>();

        builder.Services.AddSingleton(_ =>
        {
            var clientSecretCredential = new ClientSecretCredential(
                tenantId: Environment.GetEnvironmentVariable("TENANT_ID", EnvironmentVariableTarget.Process),
                clientId: Environment.GetEnvironmentVariable("CLIENT_ID", EnvironmentVariableTarget.Process),
                clientSecret: Environment.GetEnvironmentVariable("CLIENT_SECRET", EnvironmentVariableTarget.Process)
            );
            var nationalCloud = Environment.GetEnvironmentVariable("NATIONAL_CLOUD", EnvironmentVariableTarget.Process) ?? "Global";
            var httpClient = GraphClientFactory.Create(tokenCredential: clientSecretCredential, nationalCloud: nationalCloud);
            return httpClient;
        });

        var app = builder.Build();

        await app.RunAsync();
    }

    [McpServerToolType]
    public class GraphApiTool
    {
        private const string FilterParam = "$filter";
        private const string SearchParam = "$search";
        private const string CountParam = "$count";

        [McpServerTool(Name = "my-tenant", Title = "Read info about my tenant")]
        [Description("Tool to interact with Microsoft Graph (Entra)")]
        public static async Task<string> GetGraphApiData(HttpClient client,
            [Description("Microsoft Graph API URL path to call (e.g. '/users', '/groups', '/subscriptions')")] string path,
            [Description("Query parameters for the request like $filter, $count, $search, $orderby, $select. Collection of keys and values")] Dictionary<string, object> queryParameters,
            [Description("Http method like GET, POST, PUT, PATCH or DELETE")] string method,
            [Description("Request body. Optional")] string body,
            [Description("Graph version. Either 'v1.0' or 'beta'")] string graphVersion = "v1.0")
        {
            try
            {
                var requestUrl = $"/{graphVersion}{path}";
                if (queryParameters?.Count > 0)
                {
                    var queryString = string.Join("&", queryParameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                    requestUrl += $"?{queryString}";
                }

                var httpMethod = new HttpMethod(method.ToUpperInvariant());

                var requestMessage = new HttpRequestMessage(httpMethod, requestUrl)
                {
                    Headers = { { "Accept", path.EndsWith(CountParam) ? "text/plain" : "application/json" } }
                };

                if (queryParameters?.ContainsKey(FilterParam) == true || queryParameters?.ContainsKey(SearchParam) == true || path.EndsWith(CountParam))
                {
                    requestMessage.Headers.Add("ConsistencyLevel", "eventual");
                }

                if (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put || httpMethod == HttpMethod.Patch)
                {
                    requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }

                using var response = await client.SendAsync(requestMessage);
                var content = await response.Content.ReadAsStringAsync();

                return response.IsSuccessStatusCode
                    ? content
                    : $"Error: {response.StatusCode}, Content: {content}";
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
    }
}
