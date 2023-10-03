using System.Runtime.Serialization;
using System.Text.Json;

namespace OpenAIAudioTranscription
{
    class Program
    {
        private static readonly string AzureOpenAIEndpoint = "<<REDACTED>>";
        private static readonly string AzureOpenAIKey = "<<REDACTED>>"; 
        private static readonly string ModelName = "whisper";

        private static readonly Dictionary<string, string> FileList = new Dictionary<string, string>
        {
            {"a", "what-is-it-like-to-be-a-crocodile-27706.mp3"},
            {"b", "toutes-les-femmes-de-ma-vie-164527.mp3"},
            {"c", "what-can-i-do-for-you-npc-british-male-99751.mp3"}
        };

        static async Task Main(string[] args)
        {
            Console.WriteLine("Choose a file:");
            foreach (var fileEntry in FileList)
            {
                Console.WriteLine($"{fileEntry.Key} -> {fileEntry.Value}");
            }

            var chosenFileKey = Console.ReadLine();
            if (FileList.TryGetValue(chosenFileKey ?? "", out var fileName))
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Add("api-key", AzureOpenAIKey);

                        using (var audioFileStream = new FileStream($"../../../../../assets/{fileName}", FileMode.Open))
                        {
                            var formData = new MultipartFormDataContent
                            {
                                { new StreamContent(audioFileStream), "file", fileName }
                            };

                            var response = await httpClient.PostAsync($"{AzureOpenAIEndpoint}/openai/deployments/{ModelName}/audio/transcriptions?api-version=2023-09-01-preview", formData);
                            var responseContent = await response.Content.ReadAsStringAsync();

                            if (response.IsSuccessStatusCode)
                            {
                                var options = new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                };
                                var jsonResponse = JsonSerializer.Deserialize<Response>(responseContent, options);
                                Console.WriteLine(jsonResponse.Text.ToString());
                            }
                            else
                            {
                                Console.WriteLine("Failed to transcribe audio.");
                                Console.WriteLine($"Response: {responseContent}");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occurred: {e.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid file");
            }
        }
    }
}

[DataContract]
class Response
{
    [DataMember]
    public string Text { get; set; }
}
