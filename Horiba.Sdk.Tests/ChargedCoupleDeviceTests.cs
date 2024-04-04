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
        var list = new List<object>()
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
}