using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class LLMService
{
    private static readonly HttpClient client = new HttpClient();

    public async Task<string> AskLLMAsync(string prompt)
    {
        var requestBody = new
        {
            model = "mistral",
            prompt = prompt,
            stream = false
        };

        var json = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("http://localhost:11434/api/generate", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return $"[HATA] API yanıtı başarısız: {response.StatusCode}\n\n{responseBody}";
        }

        dynamic result = JsonConvert.DeserializeObject(responseBody);
        return result.response;  // Ollama'nın döndürdüğü alan
    }
}
