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

namespace Toulbar2RestCore.Controllers
{
    [Produces("application/json")]
    [Route("Toulbar2REST/ProblemLoader")]
    public class ProblemLoaderController : Controller
    {
        /*
        // POST: api/ProblemLoader
        [HttpPost]
        public ResponseModel Post([FromBody]WCSPModel value)
        {
            string directoryPath = @"C:\Users\Krzysiek\Desktop\resttest\";
            //string directoryPath = @"/usr/bin/";
            string fileFullPath;
            Dictionary<int, string> dict;
            (fileFullPath, dict) = CreateWCSPFile(value, directoryPath);
            var output = new StringBuilder();
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = $"{directoryPath}toulbar2.exe";
                process.StartInfo.Arguments = $"-s {fileFullPath}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                //process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                process.WaitForExit();
                string error = process.StandardError.ReadToEnd();
                output.Append(process.StandardOutput.ReadToEnd());
                process.Close();
                System.IO.File.Delete(fileFullPath);
                //return CreateResponse(output.ToString(),dict);
                /*
                if (System.IO.File.Exists($@"{directoryPath}\examples\{id}.res"))
                {
                    output.Append("Rozwiązanie problemu:\n");
                    output.Append(System.IO.File.ReadAllText($@"{directoryPath}\examples\{id}.res"));
                    System.IO.File.Delete($@"{directoryPath}\examples\{id}.res");
                }
                else
                {
                    output.Append("Nie znaleziono rozwiązania dla danego problemu!");
                }
            }
            catch (Exception e)
            {
                output.Append("\nWystąpił błąd! Czy na pewno wysłałeś poprawny plik? Logi:\n");
                output.Append(e.StackTrace);
                Console.Out.WriteLine(e.StackTrace);
            }

            // Creating response:
            var response = new ResponseModel();
            Regex rgx = new Regex(@"(New solution:) (\d+) (.*\n) (.*)");
            var match = rgx.Match(output.ToString());
            int maxWeight = value.Functions.Select(x => x.Weight).Sum();
            int weight = int.Parse(match.Groups[2].Value);
            response.AccomplishementPercentage = (maxWeight - weight) / (double)maxWeight;
            string[] variables = match.Groups[4].Value.Split(" ");
            int counter = 1;
            foreach (string variable in variables)
            {
                int v = int.Parse(variable);
                response.Variables.Add(new Variable() { Name = dict[counter], Value = v });
                counter++;
            }

            return response;
        }
        */
        // POST: api/ProblemLoader
        [HttpPost]
        public ResponseModel Post([FromBody]WCNFModel value)
        {
            string directoryPath = @"";
            //string directoryPath = @"/usr/bin/";
            string fileFullPath;
            Dictionary<int, string> dict;
            Console.WriteLine("Tworze plik dla toulbara");
            (fileFullPath, dict) = CreateWCNFFile(value, directoryPath);
            var output = new StringBuilder();

            try
            {
                Process process = new Process();
                Console.WriteLine("Startuje proces");
                process.StartInfo.FileName = $"toulbar2";
                process.StartInfo.Arguments = $"-s {fileFullPath}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                //process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                process.WaitForExit();
                string error = process.StandardError.ReadToEnd();
                output.Append(process.StandardOutput.ReadToEnd());
                process.Close();
                Console.WriteLine("Koncze proces");
                System.IO.File.Delete(fileFullPath);
                Console.WriteLine("Usuwam plik");
                //return CreateResponse(output.ToString(),dict);
                /*
                if (System.IO.File.Exists($@"{directoryPath}\examples\{id}.res"))
                {
                    output.Append("Rozwiązanie problemu:\n");
                    output.Append(System.IO.File.ReadAllText($@"{directoryPath}\examples\{id}.res"));
                    System.IO.File.Delete($@"{directoryPath}\examples\{id}.res");
                }
                else
                {
                    output.Append("Nie znaleziono rozwiązania dla danego problemu!");
                }*/
            }
            catch (Exception e)
            {
                output.Append("\nWystąpił błąd! Czy na pewno wysłałeś poprawny plik? Logi:\n");
                output.Append(e.StackTrace);
                Console.WriteLine($"Stacktrace: {e.StackTrace}");
                //Console.Out.WriteLine(e.StackTrace);
            }

            // Creating response:
            var response = new ResponseModel();
            Console.WriteLine($"Parsowanie regexpem:");
            Regex rgx = new Regex(@"(New solution:) (\d+) (.*\n) (.*)");
            var match = rgx.Match(output.ToString());
            int maxWeight = value.Functions.Select(x => x.Weight).Sum();
            int weight = int.Parse(match.Groups[2].Value);
            response.AccomplishementPercentage = (maxWeight - weight) / (double)maxWeight;
            string[] variables = match.Groups[4].Value.Split(" ");
            int counter = 1;
            foreach (string variable in variables)
            {
                int v = int.Parse(variable);
                response.Variables.Add(new Variable() { Name = dict[counter], Value = v });
                counter++;
            }

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
