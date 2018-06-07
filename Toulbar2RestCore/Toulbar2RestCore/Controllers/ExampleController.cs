using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Toulbar2RestCore.Controllers
{
    [Produces("application/json")]
    [Route("/Example")]
    public class ExampleController : Controller
    {
        // GET: api/Example/5
        public string Get(int id)
        {
            //string directoryPath = @"C:\Users\Krzysiek\Desktop\resttest\";
            string directoryPath = @"/usr/bin/";
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

            return output.ToString();
        }
    }
}
