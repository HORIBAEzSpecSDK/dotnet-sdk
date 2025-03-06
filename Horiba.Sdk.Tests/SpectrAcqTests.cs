using Horiba.Sdk.Enums;

namespace Horiba.Sdk.Tests;

public class SpectrAcqTests : IClassFixture<SpectrAcqDeviceTestFixture>
{
    private readonly SpectrAcqDeviceTestFixture _fixture;

    public SpectrAcqTests(SpectrAcqDeviceTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GivenSaqDevice_WhenOpeningConnection_ThenConnectionIsOpened()
    {
        // Act
        var actual = await _fixture.Saq.IsConnectionOpenedAsync();

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public async Task GivenSaqDevice_WhenOpeningAndClosingConnection_ThenConnectionIsClosed()
    {
        // Act
        await _fixture.Saq.OpenConnectionAsync();
        await _fixture.Saq.CloseConnectionAsync();
        var actual = await _fixture.Saq.IsConnectionOpenedAsync();

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public async Task GivenSaqDevice_WhenGettingSerialNumber_ThenCorrectSerialNumberIsReturned()
    {
        //Arrange
        var expectedSerialNumber = "SNPG18010036";

        // Act
        var serialNumber = await _fixture.Saq.GetSerialNumberAsync();

        // Assert
        serialNumber.Should().Be(expectedSerialNumber);
    }

    [Fact]
    public async Task GivenSaqDevice_WhenGettingFirmwareVersion_ThenCorrectFirmwareVersionIsReturned()
    {
        //Arrange
        var expectedFirmwareVersion = "O1.37 Mar 15 2018 05:10:52";

        // Act
        var firmwareVersion = await _fixture.Saq.GetFirmwareVersionAsync();

        // Assert
        firmwareVersion.Should().Be(expectedFirmwareVersion);
    }

    [Fact]
    public async Task GivenSaqDevice_WhenGettingFpgaVersion_ThenCorrectFpgaVersionIsReturned()
    {
        //Arrange
        var expectedFpgaVersion = "-:0.5";

        // Act
        var fpgaVersion = await _fixture.Saq.GetFpgaVersionAsync();

        // Assert
        fpgaVersion.Should().Be(expectedFpgaVersion);
    }

    [Fact]
    public async Task GivenSaqDevice_WhenGettingBoardVersion_ThenCorrectBoardVersionIsReturned()
    {
        //Arrange
        var expectedBoardRevision = "B";

        // Act
        var boardRevision = await _fixture.Saq.GetBordRevisionAsync();

        // Assert
        boardRevision.Should().Be(expectedBoardRevision);
    }

    [Fact(Skip = "The integration in ICL version 177 does come back with type integrationTIme")]
    public async Task GivenSaqDevice_WhenGettingIntegrationTime_ThenCorrectIntegrationTimeIsReturned()
    {
        //Arrange
        var expectedIntegrationTime = 10;

        // Act
        await _fixture.Saq.SetIntegrationTimeAsync(expectedIntegrationTime);
        var integrationTime = await _fixture.Saq.GetIntegrationTimeAsync();

        // Assert
        integrationTime.Should().Be(expectedIntegrationTime);
    }
    
    [Fact]
    public async Task GivenSaqDevice_WhenGettingMaxHvVoltageAllowed_ThenCorrectMaxHvVoltageAllowedIsReturned()
    {
        //Arrange
        var expectedMaxHvVoltageAllowed = 900;

        // Act
        var maxHvVoltageAllowed = await _fixture.Saq.GetMaxHvVoltageAllowedAsync();

        // Assert
        maxHvVoltageAllowed.Should().Be(expectedMaxHvVoltageAllowed);
    }
    
    
}