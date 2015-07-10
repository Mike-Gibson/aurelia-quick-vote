using System;
using Microsoft.AspNet.Mvc;

namespace WebAPIApplication.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        [HttpPost]
        public Guid Post([FromBody]string name)
        {
            return Guid.NewGuid();
        }

        [HttpDelete("{sessionId}")]
        public void Delete(Guid sessionId)
        {
        }
    }
}
