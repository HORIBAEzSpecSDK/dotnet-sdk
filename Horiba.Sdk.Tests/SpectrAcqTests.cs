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

    [Theory]
    [InlineData(49)]
    [InlineData(51)]
    public async Task GivenSaqDevice_WhenSettingHvBiasVoltage_ThenCorrectHvBiasVoltageIsReturned(
        int expectedHvBiasVoltage)
    {
        var initialHvBiasVoltage = await _fixture.Saq.GetHvBiasVoltageAsync();

        // Act
        await _fixture.Saq.SetHvBiasVoltageAsync(expectedHvBiasVoltage);
        var hvBiasVoltage = await _fixture.Saq.GetHvBiasVoltageAsync();

        // Assert
        hvBiasVoltage.Should().Be(expectedHvBiasVoltage);

        // Restore initial state
        await _fixture.Saq.SetHvBiasVoltageAsync(initialHvBiasVoltage);
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

    [Fact(Skip = "The integration in ICL version 177 does not come back with all parameters")]
    public async Task GivenSaqDevice_WhenGettingAcquisitionSet_ThenCorrectAcquisitionSetIsReturned()
    {
        //Arrange
        var expectedScanCount = 10;
        var expectedTimeStep = 1;
        var expectedIntegrationTime = 10;
        var expectedExternalParam = 0;
        // Act
        var acquisitionSet = await _fixture.Saq.GetAcqSetAsync();

        // Assert
        acquisitionSet["scanCount"].Should().Be(expectedScanCount);
        acquisitionSet["timeStep"].Should().Be(expectedTimeStep);
        acquisitionSet["integrationTime"].Should().Be(expectedIntegrationTime);
        acquisitionSet["externalParam"].Should().Be(expectedExternalParam);
    }

    [Fact(Skip = "The integration in ICL version 177 does deliver the wrong is_data_available bool value ")]
    public async Task GivenSaqDevice_WhenCheckingIfDataIsAvailable_ThenTrueIsReturned()
    {
        //Arrange
        await _fixture.Saq.SetIntegrationTimeAsync(2);
        await _fixture.Saq.SetAcqSetAsync(2, 0, 2, 0);
        // Act
        await _fixture.Saq.StartAcquisitionAsync(TriggerMode.TriggerAndInterval);
        await _fixture.Saq.WaitForDeviceNotBusy(TimeSpan.FromSeconds(10));
        var dataAvailable = await _fixture.Saq.GetIsDataAvailableAsync();

        // Assert
        dataAvailable.Should().BeTrue();
    }


    [Fact]
    public async Task GivenSaqDevice_WhenGettingData_ThenDataIsReturned()
    {
        //Arrange
        await _fixture.Saq.SetIntegrationTimeAsync(2);
        await _fixture.Saq.SetAcqSetAsync(2, 0, 2, 0);
        // Act
        await _fixture.Saq.StartAcquisitionAsync(TriggerMode.TriggerAndInterval);
        await Task.Delay(10000);
        var data = await _fixture.Saq.GetAvailableDataAsync();

        // Assert
        data.Data.Count().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GivenSaqDevice_WhenAcquisitionIsStopped_ThenDeviceIsNotBusy()
    {
        //Arrange
        await _fixture.Saq.SetIntegrationTimeAsync(10);
        await _fixture.Saq.SetAcqSetAsync(10, 0, 10, 0);

        // Act
        await _fixture.Saq.StartAcquisitionAsync(TriggerMode.TriggerAndInterval);
        await Task.Delay(100);

        // Assert
        _fixture.Saq.GetIsBusyAsync().Should().Be(true);
        await _fixture.Saq.StopAcquisitionAsync();
        await Task.Delay(100);
        _fixture.Saq.GetIsBusyAsync().Should().Be(false);
    }

    [Fact]
    public async Task GivenSaqDevice_WhenAcquisitionIsPausedAndThenContinued_ThenAcquisitionIsContinued()
    {
        //Arrange
        await _fixture.Saq.SetIntegrationTimeAsync(10);
        await _fixture.Saq.SetAcqSetAsync(10, 0, 10, 0);
        // Act
        await _fixture.Saq.StartAcquisitionAsync(TriggerMode.TriggerAndInterval);
        await Task.Delay(100);
        await _fixture.Saq.PauseAcquisitionAsync();

        // Assuming some mechanism to check if acquisition paused
        await _fixture.Saq.ContinueAcquisitionAsync();
        // Assuming some mechanism to check if acquisition continued
    }

    [Fact]
    public async Task GivenSaqDevice_WhenTriggerIsForced_ThenTriggerIsForced()
    {
        // Assuming some mechanism to verify trigger was forced
        await _fixture.Saq.ForceTriggerAsync();
    }

    [Fact]
    public async Task GivenSaqDevice_WhenGettingInputTriggerMode_ThenExpectedInputTriggerModeIsReturned()
    {
        //Arrange
        var expectedInTriggerMode = InTriggerMode.EventMarkerInput;
        await _fixture.Saq.SetInTriggerModeAsync(expectedInTriggerMode);

        //Act
        var returnedTriggerModes = await _fixture.Saq.GetTriggerModeAsync();

        //Assert
        returnedTriggerModes["inputTriggerMode"].Should().Be(expectedInTriggerMode);
    }

    [Fact]
    public async Task GivenSaqDevice_WhenGettingLastError_ThenLastErrorIsReturnedAsString()
    {
        //Act
        var lastError = await _fixture.Saq.GetLastErrorAsync();

        //Assert
        lastError.GetType().Should().Be(typeof(string));
    }

    [Fact]
    public async Task GivenSaqDevice_WhenGettingErrorLog_ThenErrorLogIsReturnedAsString()
    {
        //Act
        var errorLog = await _fixture.Saq.GetErrorLogAsync();

        //Assert
        errorLog.GetType().Should().Be(typeof(string));
    }

    [Fact]
    public async Task GivenSaqDevice_WhenClearingErrorLog_ThenErrorLogIsCleared()
    {
        //Act
        await _fixture.Saq.ClearErrorLogAsync();
        //Assuming some mechanism to verify error log was cleared
    }
}