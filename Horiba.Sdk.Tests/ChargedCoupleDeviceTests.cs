using Horiba.Sdk.Enums;

namespace Horiba.Sdk.Tests;

public class ChargedCoupleDeviceTests
{
    [Fact]
    public async Task GivenCcd_WhenTriggeringAllGetOperations_ThenReturnsConsistentResults()
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var ccd = dm.ChargedCoupledDevices[0];
        await ccd.OpenConnectionAsync();

        // Act
        var list = new List<object>
        {
            await ccd.GetChipSizeAsync(),
            await ccd.GetGainAsync(),
            await ccd.GetSpeedAsync(),
            await ccd.GetChipTemperatureAsync(),
            await ccd.GetCleanCountAsync(),
            await ccd.GetDeviceConfigurationAsync(),
            await ccd.GetExposureTimeAsync(),
            await ccd.GetFitParametersAsync(),
            await ccd.GetTimerResolutionAsync(),
            await ccd.GetAcquisitionBusyAsync(),
            await ccd.GetAcquisitionCountAsync(),
            // await ccd.GetAcquisitionDataAsync(), // currently no data
            await ccd.GetAcquisitionReadyAsync(),
            await ccd.GetDataSizeAsync(),
            // await ccd.GetNumberOfAveragesAsync(), // maybe device does not support this?
            await ccd.GetXAxisConversionTypeAsync()
        };

        // Assert
        list.MatchSnapshot();
    }

    [Theory]
    [InlineData(Gain.HighLight)]
    [InlineData(Gain.BestDynamicRange)]
    [InlineData(Gain.HighSensitivity)]
    public async Task GivenCcd_WhenTriggeringSetGainMethod_ThenGainIsSet(Gain expectedSetting)
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var ccd = dm.ChargedCoupledDevices.First();
        await ccd.OpenConnectionAsync();

        // Act
        await ccd.SetGainAsync(expectedSetting);
        var actualSetting = await ccd.GetGainAsync();

        // Assert
        actualSetting.Should().HaveSameValueAs(expectedSetting);
    }

    [Fact]
    public async Task GivenCcd_WhenTriggeringSetFitParameters_ThenFitParametersAreSet()
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var ccd = dm.ChargedCoupledDevices.First();
        await ccd.OpenConnectionAsync();
        var expectedParams = "1,1,1,1";

        // Act
        await ccd.SetFitParametersAsync(expectedParams);
        var actual = await ccd.GetFitParametersAsync();

        // Assert
        actual.Should().BeSameAs(expectedParams);
    }
    
    //TODO test SET/GET command pairs

    [Fact]
    public async Task GivenCcd_WhenGettingActualData_ThenReturnsByteData()
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var ccd = dm.ChargedCoupledDevices.First();
        await ccd.OpenConnectionAsync();
        await ccd.SetAcquisitionCountAsync(1);
        await ccd.SetXAxisConversionTypeAsync(ConversionType.None);
        await ccd.SetExposureTimeAsync(1000);
        await ccd.SetRegionOfInterestAsync(RegionOfInterest.Default);
        
        // Act
        var isReady = await ccd.GetAcquisitionReadyAsync();
        await ccd.SetAcquisitionStartAsync(true);
        await ccd.WaitForDeviceBusy();
        var data = await ccd.GetAcquisitionDataAsync();

        // Assert
        data.MatchSnapshot();
    }
    
    // TODO test procedure get picture, move mono, get picture again

    [Fact]
    public async Task GivenCcd_WhenOpeningConnection_ThenConnectionIsOpened()
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var ccd = dm.ChargedCoupledDevices.First();
        
        // Act
        await ccd.OpenConnectionAsync();
        var actualStatus = await ccd.IsConnectionOpenedAsync();

        // Assert
        actualStatus.Should().BeTrue();
    }

    [Fact]
    public async Task GivenCcd_WhenOpeningAndClosingConnection_ThenConnectionIsClosed()
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var ccd = dm.ChargedCoupledDevices.First();
        
        // Act
        await ccd.OpenConnectionAsync();
        await ccd.CloseConnectionAsync();
        var actualStatus = await ccd.IsConnectionOpenedAsync();

        // Assert
        actualStatus.Should().BeFalse();
    }

    [Fact]
    public async Task GivenCcd_WhenGettingSpeed_ThenReturnsSpeed()
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var ccd = dm.ChargedCoupledDevices.First();
        await ccd.OpenConnectionAsync();
        
        // Act
        var speed = await ccd.GetSpeedAsync();

        // Assert
        speed.MatchSnapshot();
    }

    [Fact]
    public async Task GivenCcd_WhenSettingExposureTime_ThenSetsExposureTimeCorrectly()
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var ccd = dm.ChargedCoupledDevices.First();
        await ccd.OpenConnectionAsync();
        var time = 1234;
        
        // Act
        await ccd.SetExposureTimeAsync(time);
        var actualTime = await ccd.GetExposureTimeAsync();

        // Assert
        actualTime.Should().Be(time);
    }

    [Fact]
    public async Task GivenCcd_WhenSettingXAxisConversionType_ThenSetsXAxisConversionType()
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var ccd = dm.ChargedCoupledDevices.First();
        await ccd.OpenConnectionAsync();
        var targetConversionType = ConversionType.FromCcdFirmware;

        // Act
        await ccd.SetXAxisConversionTypeAsync(targetConversionType);
        var actual = await ccd.GetXAxisConversionTypeAsync();
        
        // Assert
        actual.Should().Be(targetConversionType);
    }

    [Fact]
    public async Task GivenCcd_WhenSettingNumberOfAverages_ThenSetsTheNumberOfAverages()
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var ccd = dm.ChargedCoupledDevices.First();
        await ccd.OpenConnectionAsync();
        var targetNumber = 5;

        // Act
        await ccd.SetNumberOfAveragesAsync(targetNumber);
        var actual = await ccd.GetNumberOfAveragesAsync();

        // Assert
        actual.Should().Be(targetNumber);
    }

    [Fact]
    public async Task GivenCcd_WhenSettingTimeResolution_ThenSetsTheTimeResolution()
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var ccd = dm.ChargedCoupledDevices.First();
        await ccd.OpenConnectionAsync();
        var targetResolution = 5;

        // Act
        await ccd.SetTimerResolutionAsync(targetResolution);
        var actual = await ccd.GetTimerResolutionAsync();

        // Assert
        actual.Should().Be(targetResolution);
    }

    [Fact]
    public async Task GivenCcd_WhenSettingAcquisitionCount_ThenSetsTheAcquisitionCount()
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var ccd = dm.ChargedCoupledDevices.First();
        await ccd.OpenConnectionAsync();
        var targetCount = 5;

        // Act
        await ccd.SetAcquisitionCountAsync(targetCount);
        var actual = await ccd.GetAcquisitionCountAsync();

        // Assert
        actual.Should().Be(targetCount);
    }

    [Fact]
    public async Task GivenCcd_WhenSettingCleanCount_ThenSetsCleanCount()
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var ccd = dm.ChargedCoupledDevices.First();
        await ccd.OpenConnectionAsync();

        // Act
        await ccd.SetCleanCountAsync(5, CleanCountMode.Mode1);
        var actual = await ccd.GetCleanCountAsync();

        // Assert
        actual.MatchSnapshot();
    }
}