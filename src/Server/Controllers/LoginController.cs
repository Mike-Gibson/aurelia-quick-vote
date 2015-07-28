using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using WebAPIApplication.Hubs;

using  Microsoft.Framework.Logging;

namespace WebAPIApplication.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private IConnectionManager _connectionManager;
        private IHubContext _votingHub;
        private ILogger<LoginController> _logger;
        
        public LoginController(IConnectionManager connectionManager, ILogger<LoginController> logger)
        {
            _connectionManager = connectionManager;
            _votingHub = _connectionManager.GetHubContext<VotingHub>();
            _logger = logger;
        }
        
                
        [HttpGet] // TODO: TEMP!
        [Route("throw")]
        public string Throw()
        {
            throw new Exception("test");
        }
        
        [HttpPost]
        [Route("Login")]
        public Guid Login([FromBody]string name)
        {
            _logger.LogInformation("Client with name '{0}' logged in", name);
            
            _votingHub.Clients.All.userLoggedIn(name, "omg");
            
            return Guid.NewGuid();
        }

        [HttpDelete("{sessionId}")]
        public void Delete(Guid sessionId)
        {
        }
    }
}
