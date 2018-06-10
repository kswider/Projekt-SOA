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
    [Route("/Example")]
    public class ExampleController : Controller
    {
        private readonly ILogger _logger;
        public ExampleController(ILoggerFactory logger){
            this._logger = logger.CreateLogger("Toulbar2RestCore.Controllers.ExampleController");
        }
        // GET: api/Example/5
        [HttpGet]
        public string Get(int id)
        {
            
            this._logger.LogInformation(LoggerEvents.RequestPassed, "Processing request ...");
            //string directoryPath = @"C:\Users\Krzysiek\Desktop\resttest\";
            string directoryPath = @"";
            //            string trailer;
            //           if (id == 2)
            //             trailer = ".wcnf";
            //          else
            string trailer = ".wcsp";
            StringBuilder output = new StringBuilder();
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = $"{directoryPath}toulbar2";
                process.StartInfo.Arguments = $"-w=\"{directoryPath}\\examples\\{id}.res\" {directoryPath}\\examples\\{id}{trailer}";
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
                if (System.IO.File.Exists($@"{directoryPath}\examples\{id}.res"))
                {
                    output.Append("Rozwiązanie problemu:\n");
                    output.Append(System.IO.File.ReadAllText($@"{directoryPath}\examples\{id}.res"));
                    System.IO.File.Delete($@"{directoryPath}\examples\{id}.res");

                    this._logger.LogInformation(LoggerEvents.ExampleFound, "Succesfully found solution");
                }
                else
                {
                    output.Append("Nie znaleziono rozwiązania dla danego problemu!");
                    this._logger.LogWarning(LoggerEvents.ExampleNotFound, "Solution has not been found");
                }
            }
            catch (Exception e)
            {
                output.Append("\nWystąpił błąd! Czy na pewno wysłałeś poprawny plik? Logi:\n");
                output.Append(e.StackTrace);
                Console.Out.WriteLine(e.StackTrace);
                this._logger.LogError(LoggerEvents.ExampleFileError, e, "An exception occured");
            }
            this._logger.LogCritical(LoggerEvents.ExampleFileError, new Exception(), "###############################################################");
            return output.ToString();
        }
    }
}
