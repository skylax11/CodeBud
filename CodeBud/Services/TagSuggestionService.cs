using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CodeBud.Services
{
    public class TagSuggestionService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<List<string>> GetSuggestedTagsAsync(string inputText)
        {
            // API'ye gönderilecek veri
            var requestData = new { text = inputText };
            var jsonContent = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("http://localhost:5000/api/predict-tags", content);
                if (!response.IsSuccessStatusCode)
                    return new List<string>();

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TagSuggestionResponse>(responseJson);

                return result?.Tags ?? new List<string>();
            }
            catch
            {
                // Servis erişilemiyorsa boş liste dön
                return new List<string>();
            }
        }

        // Gelen JSON cevabının modeli
        private class TagSuggestionResponse
        {
            [JsonProperty("tags")]
            public List<string> Tags { get; set; }
        }
    }
}
