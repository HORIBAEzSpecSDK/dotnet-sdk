namespace Horiba.Sdk;

/// <summary>
/// Represents an issue with the communication with the ICL process.
/// </summary>
/// <param name="message">The message reported from the ICL</param>
public class CommunicationException(string message) : Exception(message)
{
}