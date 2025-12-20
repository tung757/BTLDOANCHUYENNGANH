using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WEBLAPTOP.Service;
using WEBLAPTOP.Models;

namespace WEBLAPTOP.Controllers.Api
{
    public class TuVanController : ApiController
    {
        private readonly DARKTHESTORE db = new DARKTHESTORE();

        [HttpPost]
        [Route("api/advice")]
        public async Task<IHttpActionResult> GetAdvice(UserQuestion request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Question))
                return BadRequest("Question is required.");

            var products = db.SANPHAMs
                .Select(p => new { p.TenSP, p.GiaBan, p.Mota })
                .ToList();

            string prompt = $@"
                Bạn là nhân viên tư vấn laptop. Hãy trả lời dựa trên dữ liệu sau:

                DANH SÁCH SẢN PHẨM:
                {string.Join("\n", products.Select(p => $"{p.TenSP} - {p.GiaBan} VNĐ - {p.Mota}"))}

                CÂU HỎI KHÁCH HÀNG:
                {request.Question}

                TRẢ LỜI CHI TIẾT VÀ KHÔNG CÓ DẤU * , NGẮN GỌN:";
            try
            {
                string answer = await ChatGPTService.Ask(prompt);
                //string answer = await GeminiService.Ask(prompt);
                //string answerFormat = answer.Replace("*", "");
                return Ok(new { answer });
            }
            catch (System.Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class UserQuestion
    {
        public string Question { get; set; }
    }
}
