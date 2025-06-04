using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class LLMService
{
    private static readonly HttpClient client = new HttpClient();

    public async Task<string> AskLLMAsync(string question)
    {
        var json = JsonConvert.SerializeObject(new { question });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("http://localhost:5001/ask", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return $"[HATA] API yanıtı başarısız: {response.StatusCode}\n\n{responseBody}";
        }


        dynamic result = JsonConvert.DeserializeObject(responseBody);
        return result.answer;
    }
}
