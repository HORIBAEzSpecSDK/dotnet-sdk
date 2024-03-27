using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;
using Snapshooter.Xunit;

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
}