using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Toulbar2RestCore.Models;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Toulbar2RestCore.Models.InternalClasses;
using Toulbar2RestCore.Common;

namespace Toulbar2RestCore.Controllers
{
    [Produces("application/json")]
    [Route("Toulbar2REST/File")]
    public class FileController : Controller
    {
        private readonly ILogger _logger;
        
        public FileController(ILoggerFactory logger){
            this._logger = logger.CreateLogger("Toulbar2RestCore.Controllers.FileController");
        }

        // POST: Toulbar2REST/File
        [HttpPost]
        public ResponseModel Post([FromBody]RawTextFileModel file)
        { 
            this._logger.LogInformation(LoggerEvents.RequestPassed, "Processing request");  
            string directoryPath = @"";
            Random random = new Random();
            string fileFullPath = $"{directoryPath}{random.Next(10000)}tmp.{file.Type}";
            System.IO.File.WriteAllText(fileFullPath, file.Content);

            string output = Toulbar2Operations.RunToulbar2(fileFullPath, _logger);
            System.IO.File.Delete(fileFullPath);

            // Creating response:
            var response = new ResponseModel();
            response.RawOutput = output;
            var rgx = new Regex(@"(New solution:) (\d+) (.*\n) (.*)");
            var match = rgx.Match(output);
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

            var rgx2 = new Regex(@"Optimum: \d+ in (\d+) .*and ([0-9]*.?[0-9]*)");
            match = rgx2.Match(output);
            response.Memory = int.Parse(match.Groups[1].Value);
            response.Time = double.Parse(match.Groups[2].Value);

            this._logger.LogInformation(LoggerEvents.ResponseCreated, "Succesfully created response");

            return response;
        }
    }
}
