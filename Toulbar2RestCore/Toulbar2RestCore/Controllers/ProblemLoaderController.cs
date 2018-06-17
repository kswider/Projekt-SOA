using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Toulbar2RestCore.Models;
using Toulbar2RestCore.Models.InternalClasses;
using Microsoft.Extensions.Logging;
using Toulbar2RestCore.Common;

namespace Toulbar2RestCore.Controllers
{
    [Produces("application/json")]
    public class ProblemLoaderController : Controller
    {
        private readonly ILogger _logger;
        
        public ProblemLoaderController(ILoggerFactory logger){
            this._logger = logger.CreateLogger("Toulbar2RestCore.Controllers.ProblemLoaderController");
        }

        // POST: Toulbar2REST/ProblemLoader/wcnf
        [Route("Toulbar2REST/ProblemLoader/wcsp")]
        [HttpPost]
        public ResponseModel Post([FromBody]WCSPModel value)
        {
            this._logger.LogInformation(LoggerEvents.RequestPassed, "Processing request ...");
            string directoryPath = @"";
            string fileFullPath;
            Dictionary<int, string> dict;
            (fileFullPath, dict) = CreateWCSPFile(value, directoryPath);
            string output = Toulbar2Operations.RunToulbar2(fileFullPath, _logger);

            // Creating response:
            var response = new ResponseModel();
            response.RawOutput = output;
            var rgx = new Regex(@"(New solution:) (\d+) (.*\n) (.*)");
            var match = rgx.Match(output);
            int maxWeight = value.Functions.Select(x => x.Weight).Sum();
            int weight = int.Parse(match.Groups[2].Value);
            response.AccomplishementPercentage = (maxWeight - weight) / (double)maxWeight * 100;
            string[] variables = match.Groups[4].Value.Split(" ");
            int counter = 1;
            foreach (string variable in variables)
            {
                int v = int.Parse(variable);
                response.Variables.Add(new Variable() { Name = dict[counter], Value = v });
                counter++;
            }

            var rgx2 = new Regex(@"Optimum: \d+ in (\d+) .*and ([0-9]*.?[0-9]*)");
            match = rgx2.Match(output);
            response.Memory = int.Parse(match.Groups[1].Value);
            response.Time = double.Parse(match.Groups[2].Value);

            this._logger.LogInformation(LoggerEvents.ResponseCreated, "Succesfully created response");

            return response;
        }

        // POST: Toulbar2REST/ProblemLoader/wcnf
        [Route("Toulbar2REST/ProblemLoader/wcnf")]
        [HttpPost]
        public ResponseModel Post([FromBody]WCNFModel value)
        {
            this._logger.LogInformation(LoggerEvents.RequestPassed, "Processing request ...");
            string directoryPath = @"";
            string fileFullPath;
            Dictionary<int, string> dict;
            
            (fileFullPath, dict) = CreateWCNFFile(value, directoryPath);
            this._logger.LogInformation(LoggerEvents.FileCreated, "File created, file path: {PATH}", fileFullPath);
            string output = Toulbar2Operations.RunToulbar2(fileFullPath, _logger);

            // Creating response:
            var response = new ResponseModel();
            response.RawOutput = output;
            var rgx = new Regex(@"(New solution:) (\d+) (.*\n) (.*)");
            var match = rgx.Match(output);
            int maxWeight = value.Functions.Select(x => x.Weight).Sum();
            int weight = int.Parse(match.Groups[2].Value);
            response.AccomplishementPercentage = (maxWeight - weight) / (double)maxWeight * 100;
            string[] variables = match.Groups[4].Value.Split(" ");
            int counter = 1;
            foreach (string variable in variables)
            {
                int v = int.Parse(variable);
                response.Variables.Add(new Variable() { Name = dict[counter], Value = v });
                counter++;
            }

            var rgx2 = new Regex(@"Optimum: \d+ in (\d+) .*and ([0-9]*.?[0-9]*)");
            match = rgx2.Match(output);
            response.Memory = int.Parse(match.Groups[1].Value);
            response.Time = double.Parse(match.Groups[2].Value);

            this._logger.LogInformation(LoggerEvents.ResponseCreated, "Succesfully created response");

            return response;
        }

        private (string, Dictionary<int,string>) CreateWCSPFile(WCSPModel value, string directoryPath)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"problemloader {value.Variables.Count} {value.Variables.Select(x => x.MaxVal + 1).Max()} {value.Functions.Count} {value.UpperBound}");
            var variablesMap = new Dictionary<string, int>();
            var reverseVariablesMap = new Dictionary<int, string>();
            int counter = 0;
            foreach(Variable v in value.Variables)
            {
                variablesMap.Add(v.Name, counter);
                reverseVariablesMap.Add(counter, v.Name);
                sb.Append($"{v.MaxVal + 1} ");
                counter++;
            }
            sb.AppendLine();
            foreach(Function f in value.Functions)
            {
                String[] args = f.Value.Split(" ");
                sb.Append($"{args.Count()} ");
                foreach(string arg in args)
                {
                    sb.Append($"{variablesMap[arg]} ");
                }
                sb.Append($"-1 {f.Name} {f.Weight}"); // TODO
            }
            
            Random random = new Random();
            string fileFullPath = $"{directoryPath}{random.Next(10000)}tmp.wcsp";
            System.IO.File.WriteAllText(fileFullPath, sb.ToString());

             this._logger.LogInformation(LoggerEvents.ResponseCreated, "Succesfully created response");
            return (fileFullPath, reverseVariablesMap);
        }

        private (string, Dictionary<int,string>) CreateWCNFFile(WCNFModel value, string directoryPath)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"p wcnf {value.Variables.Count} {value.Functions.Count}");

            var variablesMap = new Dictionary<string, int>();
            var reverseVariablesMap = new Dictionary<int, string>();
            int counter = 1;
            foreach (string v in value.Variables)
            {
                variablesMap.Add(v, counter);
                reverseVariablesMap.Add(counter, v);
                counter++;
            }

            foreach (Function f in value.Functions)
            {
                sb.Append($"{f.Weight} ");
                string[] args = f.Content.Split(" ");
                foreach(string arg in args)
                {
                    if (arg[0].Equals('-'))
                    {
                        sb.Append($"{-variablesMap[arg.Substring(1)]} ");
                    }
                    else
                    {
                        sb.Append($"{variablesMap[arg]} ");
                    }
                }
                sb.AppendLine("0");
            }

            Random random = new Random();
            string fileFullPath = $"{directoryPath}{random.Next(10000)}tmp.wcnf";
            Console.WriteLine($"Sciezka pliku: {fileFullPath}");
            System.IO.File.WriteAllText(fileFullPath, sb.ToString());

            return (fileFullPath, reverseVariablesMap);
        }
    }


}
