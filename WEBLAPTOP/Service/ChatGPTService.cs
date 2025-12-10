using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WEBLAPTOP.Service
{
    public static class ChatGPTService
    {
        static readonly HttpClient client = new HttpClient();

        public static async Task<string> Ask(string prompt)
        {
            var apiKey = ConfigurationManager.AppSettings["OpenChatAI"];

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var body = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var json = JsonConvert.SerializeObject(body);

            var response = await client.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            var result = await response.Content.ReadAsStringAsync();

            // Kiểm tra lỗi
            if (!response.IsSuccessStatusCode)
            {
                return "Lỗi gọi OpenAI API: " + result;
            }

            dynamic data = JsonConvert.DeserializeObject(result);
            return data.choices[0].message.content.ToString();
        }
    }
}
