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
        // POST: api/File
        [HttpOptions]
        public string Post([FromBody]RawTextFileModel file)
        { 
            this._logger.LogInformation(LoggerEvents.RequestPassed, "Processing request");  
            //string directoryPath = @"C:\Users\Krzysiek\Desktop\resttest\";
            string directoryPath = @"";
            Random random = new Random();
            string fileFullPath = $"{directoryPath}{random.Next(10000)}tmp.{file.Format}";
            System.IO.File.WriteAllText(fileFullPath, file.Data);

            StringBuilder output = new StringBuilder();
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = $"{directoryPath}toulbar2";
                process.StartInfo.Arguments = $"-s {fileFullPath}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                //process.StartInfo.WorkingDirectory = @"C:\Program Files (x86)\toulbar2 0.9.8\bin\";
                //process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                //process.WaitForExit();
                output.Append("Alokacja pamięci dla zmiennych:\n");
                output.Append(process.StandardOutput.ReadToEnd());
                string error = process.StandardError.ReadToEnd();
                process.Close();
                /*
                if (System.IO.File.Exists($"{fileFullPath}sol"))
                {
                    output.Append("Rozwiązanie problemu:\n");
                    output.Append(System.IO.File.ReadAllText($"{fileFullPath}sol"));
                    System.IO.File.Delete($"{fileFullPath}sol");
                }
                else
                {
                    output.Append("Nie znaleziono rozwiązania dla danego problemu!");
                }
                */
                System.IO.File.Delete(fileFullPath);
                this._logger.LogInformation(LoggerEvents.FileLoaded, "File succesfully loaded");

            }
            catch (Exception e)
            {
                output.Append("\nWystąpił błąd! Czy na pewno wysłałeś poprawny plik? Logi:\n");
                output.Append(e.StackTrace);
                Console.Out.WriteLine(e.StackTrace);
                this._logger.LogError(LoggerEvents.FileError, e, "An exception occured");
            }

            return output.ToString();
        }

    }
}
