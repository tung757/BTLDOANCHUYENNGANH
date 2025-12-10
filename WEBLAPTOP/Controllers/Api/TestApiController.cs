using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WEBLAPTOP.Controllers.Api
{
    public class TestApiController : ApiController
    {
        [HttpGet]
        [Route("api/test/hello")]
        public IHttpActionResult Hello()
        {
            return Ok(new { message = "API đang hoạt động!" });
        }
    }
}
