using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Toulbar2RestCore.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ILogger _logger;

        public ValuesController(ILoggerFactory logger){
            this._logger = logger.CreateLogger("Toulbar2RestCore.Controllers.ValuesController");
        }  

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            this._logger.LogInformation(LoggerEvents.RequestPassed, " I N F O R M A T I O N ");
            this._logger.LogWarning(LoggerEvents.RequestPassed, " W A R N I N G ");
            this._logger.LogCritical(LoggerEvents.RequestFailed, new Exception(), " E X C E P T I O N ");
            
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
