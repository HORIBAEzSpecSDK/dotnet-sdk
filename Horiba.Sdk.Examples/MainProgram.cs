using Microsoft.VisualBasic;
using Serilog;
using Serilog.Events;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Horiba.Sdk.Examples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            /// <summary>
            /// Serilog log levels are used to specify the importance and severity of log messages. The available log levels in Serilog are:
            ///�	Verbose: Detailed and potentially high-volume messages that are useful during development.
            ///�	Debug: Diagnostic messages that are useful for debugging.
            ///�	Information: Informational messages that track the general flow of the application.
            ///�	Warning: Messages that indicate potential issues or unexpected situations.
            ///�	Error: Error messages that indicate a failure in the current activity or operation.
            ///�	Fatal: Critical errors that cause the application to crash or terminate.
            ///
            Console.WriteLine("Select a log level - chose Debug/Information for info from the SDK. Choose Warning for ICL output only:");
            Console.WriteLine("1. Verbose");
            Console.WriteLine("2. Debug");
            Console.WriteLine("3. Information");
            Console.WriteLine("4. Warning");
            Console.WriteLine("5. Error");
            Console.WriteLine("6. Fatal");
            Console.Write("Enter your choice: ");


            var logLevelMappings = new Dictionary<string, LogEventLevel>
        {
            { "1", LogEventLevel.Verbose },
            { "2", LogEventLevel.Debug },
            { "3", LogEventLevel.Information },
            { "4", LogEventLevel.Warning },
            { "5", LogEventLevel.Error },
            { "6", LogEventLevel.Fatal }
        };

            var logLevelChoice = Console.ReadLine();

            if (logLevelMappings.TryGetValue(logLevelChoice, out var logLevel))
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Is(logLevel)
                    .WriteTo.Console()
                    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                Log.Information("Logger initialized with {LogLevel} level", logLevel);
            }
            else
            {
                Console.WriteLine("Invalid choice. Defaulting to Information level.");
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();
            }


            Console.WriteLine("Select an example to run:");
            Console.WriteLine("1. CCD Example");
            Console.WriteLine("2. Monochromator Example");
            Console.WriteLine("3. SpectrAcq3 Example");
            Console.WriteLine("4. CCD abort Example");
            Console.WriteLine("5. CCD range scan with stitching Example");
            Console.WriteLine("6. CCD dark count subtraction example");
            Console.WriteLine("7. CCD raman shift example");
            Console.Write("Enter your choice: ");
            var exampleChoice = Console.ReadLine();

            switch (exampleChoice)
            {
                case "1":
                    await CcdProgram.CcdExample();
                    break;
                case "2":
                    await MonoProgram.MonoExample();
                    break;
                case "3":
                    await SpectrAcq3Programm.SpectrAcq3Example();
                    break;
                case "4":
                    await CcdAbortExample.CcdExampleStartAndAbort();
                    break;
                case "5":
                    await CcdRangeScanExample.MainAsync();
                    break;
                case "6":
                    await CcdDarkCountSubtractionExample.MainAsync();
                    break;
                case "7":
                    await CcdRamanShiftExample.MainAsync();
                    break;
                
                
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }
}