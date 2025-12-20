using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WEBLAPTOP.Service
{
    public static class GeminiService
    {
        static readonly HttpClient client = new HttpClient();

        public static async Task<string> Ask(string prompt)
        {
            var apiKey = ConfigurationManager.AppSettings["GeminiKey"];
            var model = "gemini-2.5-flash"; // Khuyên dùng tên ngắn gọn này hoặc gemini-1.5-flash-002
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

            // Cấu hình thử lại
            int maxRetries = 5;
            int delayBase = 5000; // 2 giây

            for (int i = 0; i < maxRetries; i++)
            {
                using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    var body = new
                    {
                        contents = new[] { new { parts = new[] { new { text = prompt } } } }
                    };

                    var json = JsonConvert.SerializeObject(body);
                    requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.SendAsync(requestMessage);
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic data = JsonConvert.DeserializeObject(result);
                        try { return data.candidates[0].content.parts[0].text; }
                        catch { return "❌ Không đọc được câu trả lời: " + result; }
                    }

                    // Nếu gặp lỗi 503 (Overloaded) hoặc 429 (Too Many Requests), thì chờ và thử lại
                    if ((int)response.StatusCode == 503 || (int)response.StatusCode == 429)
                    {
                        if (i == maxRetries - 1) return "❌ Server quá tải sau 3 lần thử: " + result;

                        // Chờ tăng dần (Exponential Backoff): 2s, 4s, 8s...
                        await Task.Delay(delayBase * (i + 1));
                        continue;
                    }

                    return "❌ Lỗi gọi Gemini API: " + result;
                }
            }
            return "❌ Lỗi không xác định.";
        }
    }
}
