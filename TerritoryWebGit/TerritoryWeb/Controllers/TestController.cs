using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TerritoryWeb.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        [Authorize]
        public HttpResponseMessage HelloWorld()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Hello");
        }
    }
}
