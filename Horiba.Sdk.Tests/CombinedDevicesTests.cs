using Horiba.Sdk.Enums;

namespace Horiba.Sdk.Tests;

public class CombinedDevicesTests : IClassFixture<CombinedDevicesTestFixture>
{
    private readonly CombinedDevicesTestFixture _fixture;

    public CombinedDevicesTests(CombinedDevicesTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GivenMonoAndCcd_WhenUsingCentralScan_ThenReadsData()
    {
        // Arrange
        var targetWavelength = 230;
        await _fixture.Mono.HomeAsync();
        await _fixture.Mono.WaitForDeviceNotBusy(waitIntervalInMs: 2000);
        await _fixture.Mono.MoveToWavelengthAsync(targetWavelength);
        await _fixture.Mono.WaitForDeviceNotBusy();
        await _fixture.Ccd.SetCenterWavelengthAsync(targetWavelength);
        await _fixture.Ccd.SetXAxisConversionTypeAsync(ConversionType.FromIclSettingsIni);
        await _fixture.Ccd.SetAcquisitionFormatAsync(AcquisitionFormat.Spectra, 1);
        await _fixture.Ccd.SetAcquisitionCountAsync(1);
        await _fixture.Ccd.SetRegionOfInterestAsync(RegionOfInterest.Default);
        await _fixture.Ccd.SetTimerResolutionAsync(TimerResolution.Millisecond);
        await _fixture.Ccd.SetExposureTimeAsync(50);
        await _fixture.Ccd.WaitForDeviceNotBusy();

        // Act
        Dictionary<string, object> data = [];
        if (await _fixture.Ccd.GetAcquisitionReadyAsync())
        {
            await _fixture.Ccd.SetAcquisitionStartAsync(true);
            await _fixture.Ccd.WaitForDeviceNotBusy(TimeSpan.FromSeconds(1));
            data = await _fixture.Ccd.GetAcquisitionDataAsync();
        }
        
        // Assert
        data.Count.Should().BeGreaterThan(1);
    }
}