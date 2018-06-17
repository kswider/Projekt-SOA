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
            int maxWeight = Toulbar2Operations.CalcualteMaxWeightFromFile(file.Content, file.Type);
            var rgx = new Regex(@"New solution: .*\n (.*)\nOptimum: (\d+) in (\d+) .* and (\d+\.?\d*)");
            var match = rgx.Match(output);
            if (match.Success)
            {
                string[] variables = match.Groups[1].Value.Split(" ");
                int counter = 1;

                foreach (string variable in variables)
                {
                    int v = int.Parse(variable);
                    response.Variables.Add(new Variable() { Name = dict[counter], Value = v });
                    counter++;
                }

                int weight = 0;
                int.TryParse(match.Groups[2].Value, out weight);
                response.AccomplishementPercentage = (maxWeight - weight) / (double)maxWeight * 100;
                int memory = 0;
                int.TryParse(match.Groups[3].Value, out memory);
                response.Memory = memory;
                double time = 0;
                double.TryParse(match.Groups[4].Value, out time);
                response.Time = time;
            }
            else
            {
                response.RawOutput += "\nSolution not found!!!";
            }
            this._logger.LogInformation(LoggerEvents.ResponseCreated, "Succesfully created response");

            return response;
        }
    }
}
