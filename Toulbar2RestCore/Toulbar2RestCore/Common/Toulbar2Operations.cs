using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Toulbar2RestCore.Common
{
    public class Toulbar2Operations
    {
        public static string RunToulbar2(string fileFullPath,bool deleteAfter, ILogger logger)
        {
            var output = new StringBuilder();
            try
            {
                Process process = new Process();
                logger.LogInformation(LoggerEvents.Process, "Starting process ...");
                process.StartInfo.FileName = $@"toulbar2";
                process.StartInfo.Arguments = $"-s {fileFullPath}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                process.WaitForExit();
                string error = process.StandardError.ReadToEnd();
                output.Append(process.StandardOutput.ReadToEnd());
                process.Close();
                logger.LogInformation(LoggerEvents.Process, "Process finished");
                if (deleteAfter)
                {
                    System.IO.File.Delete(fileFullPath);
                    logger.LogInformation(LoggerEvents.Process, "File deleted, filepath: {PATH}", fileFullPath);
                }
            }
            catch (Exception e)
            {
                output.Append("\nWystąpił błąd! Czy na pewno wysłałeś poprawny plik? Logi:\n");
                output.Append(e.StackTrace);
                logger.LogError(LoggerEvents.ProblemError, e, "An exception ocurred");
            }
            return output.ToString();
        }

        public static int CalcualteMaxWeightFromFile(string file, string problemType)
        {
            int maxWeight = 0;
            var lines = file.Split('\n');
            switch (problemType)
            {
                case "wcsp":
                    var rgx = new Regex(@"(\d+)$");
                    lines.Skip(2).ToList().ForEach(x =>
                    {
                        var match = rgx.Match(x);
                        int result;
                        if (int.TryParse(match.Groups[1].Value, out result))
                            maxWeight += result;
                    });
                    break;

                case "wcnf":
                    rgx = new Regex(@"^(\d+)");
                    foreach (var line in lines)
                    {
                        if (line[0] == 'c' || line[0] == 'p')
                            continue;
                        else
                        {
                            var match = rgx.Match(line);
                            int result;
                            if(int.TryParse(match.Groups[1].Value,out result))
                                maxWeight += result;
                        }
                    }
                    break;
            }
            return maxWeight;
        }
    }
}

