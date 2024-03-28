namespace Horiba.Sdk.Tests;

public class CommandsTests
{
    [Fact]
    public async Task GivenIclInfoCommand_WhenSerializing_ThenCommandIdAndNameAreCorrect()
    {
        // Arrange
        var iclInfoCommand = new IclInfoCommand();
        
        // Act
        var jsonCommand = iclInfoCommand.ToJson();
        
        // Assert
        jsonCommand.MatchSnapshot();
    }
    
    [Fact]
    public async Task GivenDeviceInfoCommand_WhenSendingUsingCommunicator_ThenReceivesExpectedResponse()
    {
        // Arrange
        var communicator = new WebSocketCommunicator();
        await communicator.OpenConnectionAsync();
        var command = new IclInfoCommand();

        // Act
        var response = await communicator.SendAsync(command);

        // Assert
        response.MatchSnapshot();
    }
}