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
    [Route("Toulbar2REST/File")]
    public class FileController : Controller
    {
        // GET: api/File
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/File/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/File
        public string Post([FromBody]string value)
        {
            //string directoryPath = @"C:\Users\Krzysiek\Desktop\resttest\";
            string directoryPath = @"/usr/bin/";
            Random random = new Random();
            string fileFullPath = $"{directoryPath}{random.Next(10000)}tmp.wcsp";
            System.IO.File.WriteAllText(fileFullPath, value);

            StringBuilder output = new StringBuilder();
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = $"{directoryPath}toulbar2";
                process.StartInfo.Arguments = $"-w=\"{fileFullPath}sol\" {fileFullPath}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                //process.StartInfo.WorkingDirectory = @"C:\Program Files (x86)\toulbar2 0.9.8\bin\";
                //process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                process.WaitForExit();
                output.Append("Alokacja pamięci dla zmiennych:\n");
                output.Append(process.StandardOutput.ReadToEnd());
                string error = process.StandardError.ReadToEnd();
                process.Close();
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
                System.IO.File.Delete(fileFullPath);

            }
            catch (Exception e)
            {
                output.Append("\nWystąpił błąd! Czy na pewno wysłałeś poprawny plik? Logi:\n");
                output.Append(e.StackTrace);
                Console.Out.WriteLine(e.StackTrace);
            }

            return output.ToString();
        }

        // PUT: api/File/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
