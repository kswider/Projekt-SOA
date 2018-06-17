using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Toulbar2RestCore.Models;
using Toulbar2RestCore.Models.InternalClasses;

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
        public ResponseModel Get(int id)
        {
            
            this._logger.LogInformation(LoggerEvents.RequestPassed, "Processing request ...");
            string directoryPath = @"app";
            string trailer = ".wcsp";
            StringBuilder output = new StringBuilder();
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = $"toulbar2";
                process.StartInfo.Arguments = $"-s /{directoryPath}/examples/{id}{trailer}";
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

            // Creating response:
            var response = new ResponseModel();
            var rgx = new Regex(@"(New solution:) (\d+) (.*\n) (.*)");
            var match = rgx.Match(output.ToString());
            //int maxWeight = value.Functions.Select(x => x.Weight).Sum();
            int weight = int.Parse(match.Groups[2].Value);
            response.AccomplishementPercentage = 100;// (maxWeight - weight) / (double)maxWeight * 100;
            string[] variables = match.Groups[4].Value.Split(" ");
            int counter = 1;
            foreach (string variable in variables)
            {
                int v = int.Parse(variable);
                response.Variables.Add(new Variable() { Name = counter.ToString(), Value = v });
                counter++;
            }

            var rgx2 = new Regex(@"Optimum: \d+ in (\d+) .*and (\d+)");
            match = rgx2.Match(output.ToString());
            response.Memory = int.Parse(match.Groups[1].Value);
            response.Time = int.Parse(match.Groups[2].Value);

            this._logger.LogInformation(LoggerEvents.ResponseCreated, "Succesfully created response");

            return response;
        }
    }
}
