using Microsoft.VisualBasic;
using Serilog;
using Serilog.Events;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Horiba.Sdk.Examples
{
    public class MainProgram
    {
        private static async Task Main(string[] args)
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


            var rootNamespace = "Horiba.Sdk.Examples";
            var namespaces = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace != null && t.Namespace.StartsWith(rootNamespace) && t.Namespace != rootNamespace)
                .Select(t => t.Namespace)
                .Distinct()
                .Select(ns => ns.Split('.').Last())
                .ToList();

            Console.WriteLine("Select a namespace:");
            for (var i = 0; i < namespaces.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {namespaces[i]}");
            }
            Console.Write("Enter your choice: ");
            var namespaceChoice = int.Parse(Console.ReadLine()) - 1;
            var selectedNamespace = namespaces[namespaceChoice];

            var classes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace == $"{rootNamespace}.{selectedNamespace}" && t.Assembly == Assembly.GetExecutingAssembly() && !t.Name.StartsWith("<"))
                .ToList();

            Console.WriteLine($"Classes in namespace {selectedNamespace}:");
            for (var i = 0; i < classes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {classes[i].Name}");
            }
            Console.Write("Enter your choice: ");
            var classChoice = int.Parse(Console.ReadLine()) - 1;
            var selectedClass = classes[classChoice];

            var method = selectedClass.GetMethod("MainAsync", BindingFlags.Public | BindingFlags.Static);
            if (method != null)
            {
                await (Task)method.Invoke(null, null);
            }
            else
            {
                Console.WriteLine("MainAsync method not found in the selected class.");
            }
            }
        }
    }