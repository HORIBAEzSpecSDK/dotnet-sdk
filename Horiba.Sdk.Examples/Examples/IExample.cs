namespace Horiba.Sdk.Examples;

public interface IExample
{
    Task MainAsync(bool showIclConsoleOutput = false);
}