using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Toulbar2RestCore.Controllers
{
    
    
    [Produces("application/json")]
    [Route("Toulbar2REST/Example")]
    public class ExampleController : Controller
    {
        private readonly ILogger _logger;
        public ExampleController(ILoggerFactory logger){
            this._logger = logger.CreateLogger("Toulbar2RestCore.Controllers.ExampleController");
        }

        // GET: Toulbar2REST/Example
        [HttpGet("{id}")]
        public string Get(int id)
        {
            
            this._logger.LogInformation(LoggerEvents.RequestPassed, "Processing request ...");
            string directoryPath = @"app";
            string trailer = ".wcsp";
            StringBuilder output = new StringBuilder();
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = $"toulbar2";
                process.StartInfo.Arguments = $"-s {directoryPath}/examples/{id}{trailer}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                process.WaitForExit();
                string error = process.StandardError.ReadToEnd();
                output.Append(process.StandardOutput.ReadToEnd());
                process.Close();
            }
            catch (Exception e)
            {
                output.Append(e.StackTrace);
                Console.Out.WriteLine(e.StackTrace);
                this._logger.LogError(LoggerEvents.ExampleFileError, e, "An exception occured");
            }
            
            return output.ToString();
        }
    }
}
