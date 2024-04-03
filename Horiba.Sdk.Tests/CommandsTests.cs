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
        jsonCommand.MatchSnapshot(options => options.IgnoreField("id"));
    }

    [Fact]
    public async Task GivenDeviceInfoCommand_WhenSendingUsingCommunicator_ThenReceivesExpectedResponse()
    {
        // Arrange
        using var manager = new DeviceManager();
        await manager.StartAsync(true, false);

        // Act
        var response = await manager.Communicator.SendWithResponseAsync(new IclInfoCommand());

        // Assert
        response.MatchSnapshot(options => options.IgnoreField("id")
            .IgnoreField("**.nodeBuilt")
            .IgnoreField("**.nodeVersion"));
    }

    [Fact]
    public async Task GivenCommand_WhenSerializing_ThenHasCorrectParameter()
    {
        // Arrange
        var command = new IclBinaryModeAllCommand();

        // Act
        var json = command.ToJson();

        // Assert
        json.MatchSnapshot(options => options.IgnoreField("id"));
    }
}