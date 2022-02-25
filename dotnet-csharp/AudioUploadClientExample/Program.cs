using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudioUploadClientExample
{
    public static class Program
    {
        private const string BaseApiUrl = "https://integrations.capturi.ai/v1/";
        private static string? _apiToken;
        private const bool UploadAudioUsingExternalConversationId = false;

        public static async Task Main()
        {
            _apiToken = Environment.GetEnvironmentVariable("API-TOKEN"); //An API token called 'API-TOKEN' must be defined - do a search for the key in the environment variables
            if (string.IsNullOrWhiteSpace(_apiToken))
            {   //if not found, set API token in code
                _apiToken = "INSERT_SECRET_API_TOKEN_HERE!";
            }
            
            await RunAsync();
        }

        private static async Task RunAsync()
        {
            var externalConversationId = Guid.NewGuid();
            const string fileName = "test-recording.wav";
            try
            {
                var conversationId = await CreateConversationUsingExternalId(externalConversationId);
                Console.WriteLine("Created conversation was assigned ConversationId: " + conversationId);

                if (UploadAudioUsingExternalConversationId)
                {
                    await UploadAudioFileUsingExternalConversationId(fileName, externalConversationId);
                }
                else
                {
                    await UploadAudioFileUsingConversationId(fileName, conversationId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        //Http Client Setup with default base Api Url and a raw API token key
        private static HttpClient SetupHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BaseApiUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", _apiToken);
            return httpClient;
        }

        //Conversation Creation
        private static async Task<Guid> CreateConversationUsingExternalId(Guid externalConversationId)
        {
            using var httpClient  = SetupHttpClient();
            var request = CreateConversationRequest(externalConversationId);
            var responseCreateConversation = await httpClient.PostAsJsonAsync("conversation", request);
            if (responseCreateConversation.StatusCode != HttpStatusCode.OK)
            {
                var contents = await responseCreateConversation.Content.ReadAsStringAsync();
                Console.WriteLine("Failed creating conversation. Remote server responded: " + contents);
            }
            responseCreateConversation.EnsureSuccessStatusCode();
            Console.WriteLine($"Conversation with external id: '{externalConversationId}' was successfully uploaded");
            
            var content = await responseCreateConversation.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<CreateConversationResponse>(content);
            return string.IsNullOrWhiteSpace(json?.Uid) ? Guid.Empty : Guid.Parse(json.Uid);
        }
        
        private static object CreateConversationRequest(Guid externalConversationId)
        {
            var conversationRequest = new
            {
                ExternalId = externalConversationId,
                NumberOfSpeakers = 1,
                PhoneNumber = "11223344",
                Title = "Demo call",
                Labels = new List<string>(),
                DateTime = DateTime.UtcNow,
                Outcome = "Success",
                OutcomeReason = "We have a great product",
                AgentId = Guid.NewGuid(),
                AgentName = "Agent Schmidt",
                AgentEmail = "agent-schmidt@capturi.com",
            };
            return conversationRequest;
        }
        
        private class CreateConversationResponse
        {
            [JsonPropertyName("UID")]
            public string Uid { set; get; } = string.Empty;
        }
        
        //Audio file upload using external conversationId and assigned conversationId
        private static async Task UploadAudioFileUsingConversationId(string fileName, Guid conversationId)
        {
            var httpClient = SetupHttpClient();
            var multipartFormDataContent = SetupMultipartFormDataContent(fileName);
            var response = await httpClient.PostAsync($"audio/{conversationId}", multipartFormDataContent);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var contents = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Failed uploading audio file data. Remote server responded: " + contents);
            }
            response.EnsureSuccessStatusCode();
            Console.WriteLine($"Audio file: '{fileName}' was successfully uploaded using ConversationId: '{conversationId}'");
        }
        private static async Task UploadAudioFileUsingExternalConversationId(string audioFileName, Guid externalConversationId)
        {
            var httpClient = SetupHttpClient();
            var multipartFormDataContent = SetupMultipartFormDataContent(audioFileName);

            var response = await httpClient.PostAsync($"audio/external/{externalConversationId}", multipartFormDataContent);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var contents = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Failed uploading audio file data. Remote server responded: " + contents);
            }
            response.EnsureSuccessStatusCode();
            Console.WriteLine($"Audio file: '{audioFileName}' was successfully uploaded using ExternalConversationId: '{externalConversationId}'");
        }
        private static MultipartFormDataContent SetupMultipartFormDataContent(string audioFileName)
        {
            var fileStreamContent = new StreamContent(File.OpenRead(audioFileName));
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");

            var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(fileStreamContent, name: "data", fileName: audioFileName);
            return multipartFormDataContent;
        }
    }
}