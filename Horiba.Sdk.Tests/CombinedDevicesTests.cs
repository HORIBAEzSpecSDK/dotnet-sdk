using Horiba.Sdk.Calculations;
using Horiba.Sdk.Data;

//using Horiba.Sdk.Calculations.Stitching;
using Horiba.Sdk.Enums;
using Newtonsoft.Json;

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
        await _fixture.Ccd.SetCenterWavelengthAsync(0, targetWavelength);
        await _fixture.Ccd.SetXAxisConversionTypeAsync(ConversionType.FromIclSettingsIni);
        await _fixture.Ccd.SetAcquisitionFormatAsync(AcquisitionFormat.Spectra, 1);
        await _fixture.Ccd.SetAcquisitionCountAsync(1);
        await _fixture.Ccd.SetRegionOfInterestAsync(RegionOfInterest.Default);
        await _fixture.Ccd.SetTimerResolutionAsync(TimerResolution.Millisecond);
        await _fixture.Ccd.SetExposureTimeAsync(50);
        await _fixture.Ccd.WaitForDeviceNotBusy();

        // Act
        CcdData data = new CcdData();
        if (await _fixture.Ccd.GetAcquisitionReadyAsync())
        {
            await _fixture.Ccd.AcquisitionStartAsync(true);
            await _fixture.Ccd.WaitForDeviceNotBusy(TimeSpan.FromSeconds(1));
            data = await _fixture.Ccd.GetAcquisitionDataAsync();
        }
        
        // Assert
        data.Acquisition.Count.Should().BeGreaterThan(0);
    }

    //[Fact]
    //public async Task GivenMonoAndCcd_WhenReadingRangeSpectrum_ThenReturnsRangeData()
    //{
    //    // Arrange
    //    var startWavelength = 200;
    //    var endWavelength = 600;
    //    var overlap = 10;


    //    await _fixture.Mono.HomeAsync();
    //    await _fixture.Mono.WaitForDeviceNotBusy();
    //    await _fixture.Mono.SetTurretGratingAsync(Grating.Second);
    //    await _fixture.Mono.WaitForDeviceNotBusy();


    //    await _fixture.Ccd.SetTimerResolutionAsync(TimerResolution.Millisecond);
    //    await _fixture.Ccd.SetExposureTimeAsync(50);
    //    await _fixture.Ccd.SetGainAsync(0);
    //    await _fixture.Ccd.SetSpeedAsync(0);
    //    await _fixture.Ccd.SetXAxisConversionTypeAsync(ConversionType.FromIclSettingsIni);
    //    await _fixture.Ccd.SetAcquisitionFormatAsync(AcquisitionFormat.Image, 1);
    //    await _fixture.Ccd.SetRegionOfInterestAsync(RegionOfInterest.Default);

    //    // Act
    //    var result = await _fixture.Ccd.CalculateRangePositionsAsync(0, startWavelength, endWavelength, overlap);

    //    // Assert
    //    result.MatchSnapshot();
    //}

    //[Fact]
    //public async Task GivenMonoAndCcd_WhenReadingRangeSpectrumAndStitchingData_ThenDataStitchedCorrectly()
    //{
    //    // Arrange
    //    var startWavelength = 200;
    //    var endWavelength = 600;
    //    var overlap = 10;

    //    await _fixture.Mono.HomeAsync();
    //    await _fixture.Mono.WaitForDeviceNotBusy();
    //    await _fixture.Mono.SetTurretGratingAsync(Grating.Second);
    //    await _fixture.Mono.WaitForDeviceNotBusy();

    //    await _fixture.Ccd.SetTimerResolutionAsync(TimerResolution.Millisecond);
    //    await _fixture.Ccd.SetExposureTimeAsync(50);
    //    await _fixture.Ccd.SetGainAsync(SyncerityOEGain.HighLight);
    //    await _fixture.Ccd.SetSpeedAsync(SyncerityOESpeed.MHz1U);
    //    await _fixture.Ccd.SetXAxisConversionTypeAsync(ConversionType.FromIclSettingsIni);
    //    await _fixture.Ccd.SetRegionOfInterestAsync(RegionOfInterest.Default);

    //    // Act
    //    var result = (await _fixture.Ccd.CalculateRangePositionsAsync(0, startWavelength, endWavelength, overlap)).ToArray();
    //    List<AcquisitionDescription>[] data = new List<AcquisitionDescription>[result.Length];

    //    for (var i = 0; i < result.Length; i++)
    //    {
    //        await _fixture.Mono.MoveToWavelengthAsync(result[i].WavelengthValue);
    //        await _fixture.Mono.WaitForDeviceNotBusy();
    //        var wavelength = await _fixture.Mono.GetCurrentWavelengthAsync();

    //        await _fixture.Ccd.WaitForDeviceNotBusy();

    //        await _fixture.Ccd.SetCenterWavelengthAsync(0, result[i].WavelengthValue);

    //        await _fixture.Ccd.WaitForDeviceNotBusy();

    //        if (await _fixture.Ccd.GetAcquisitionReadyAsync())
    //        {
    //            await _fixture.Ccd.AcquisitionStartAsync(true);
    //            await _fixture.Ccd.WaitForDeviceNotBusy();
    //            var acquiredData = await _fixture.Ccd.GetAcquisitionDataAsync();
    //            var acquisition = acquiredData.GetValueOrDefault("acquisition");
    //            data[i] = JsonConvert.DeserializeObject<List<AcquisitionDescription>>(acquisition.ToString());
    //        }
    //    }

    //    var stitch = new LinearSpectraStitch(data.Select(d => d.First().Region.First().Data.Select(xy => new XYData(xy)).ToList()).ToArray());
    //    var stitchedData = stitch.Stitch();

    //    // Assert
    //    stitchedData.MatchSnapshot();
    //}
}