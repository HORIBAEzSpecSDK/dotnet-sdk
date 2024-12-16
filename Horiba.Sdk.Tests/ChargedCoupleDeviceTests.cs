using Horiba.Sdk.Calculations;
using Horiba.Sdk.Calculations.DarkCountSubtraction;
using Horiba.Sdk.Data;
using Horiba.Sdk.Enums;
using Horiba.Sdk.Data;
using Newtonsoft.Json;
using Serilog;

namespace Horiba.Sdk.Tests;

public class ChargedCoupleDeviceTests : IClassFixture<ChargedCoupleDeviceTestFixture>
{
    private readonly ChargedCoupleDeviceTestFixture _fixture;

    public ChargedCoupleDeviceTests(ChargedCoupleDeviceTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GivenCcd_WhenSettingGain_ThenGainIsSet()
    {
        // Arrange
        int expectedGainTokenBefore = 0;
        int expectedGainTokenAfter = 1;

        // Act
        await _fixture.Ccd.SetGainAsync(expectedGainTokenBefore);
        int returnedGainTokenBefore = await _fixture.Ccd.GetGainAsync();
        await _fixture.Ccd.SetGainAsync(expectedGainTokenAfter);
        int returnedGainTokenAfter = await _fixture.Ccd.GetGainAsync();

        // Assert
        returnedGainTokenBefore.Should().Be(expectedGainTokenBefore);
        returnedGainTokenAfter.Should().Be(expectedGainTokenAfter);
    }

    [Fact(Skip = "Description of the parameters is missing. Not sure how to test this")]
    public async Task GivenCcd_WhenTriggeringSetFitParameters_ThenFitParametersAreSet()
    {
        // Arrange
        var expectedParams = "1,1,1,1,1";

        // Act
        await _fixture.Ccd.SetFitParametersAsync(expectedParams);
        var actual = await _fixture.Ccd.GetFitParametersAsync();

        // Assert
        actual.Should().BeSameAs(expectedParams);
    }

    [Fact(Skip = "It should be tested separately")]
    public async Task GivenCcd_WhenOpeningConnection_ThenConnectionIsOpened()
    {
        // Act
        await _fixture.Ccd.OpenConnectionAsync();
        var actualStatus = await _fixture.Ccd.IsConnectionOpenedAsync();

        // Assert
        actualStatus.Should().BeTrue();
    }

    [Fact(Skip = "It should be tested separately")]
    public async Task GivenCcd_WhenOpeningAndClosingConnection_ThenConnectionIsClosed()
    {
        // Act
        await _fixture.Ccd.OpenConnectionAsync();
        await _fixture.Ccd.CloseConnectionAsync();
        var actualStatus = await _fixture.Ccd.IsConnectionOpenedAsync();

        // Assert
        actualStatus.Should().BeFalse();
    }

    [Fact]
    public async Task GivenCcd_WhenSettingSpeed_ThenSpeedIsUpdated()
    {
        // Arrange
        int expectedSpeedTokenBefore = 0;
        int expectedSpeedTokenAfter = 1;

        // Act
        await _fixture.Ccd.SetSpeedAsync(expectedSpeedTokenBefore);
        int returnedSpeedTokenBefore = await _fixture.Ccd.GetSpeedAsync();
        await _fixture.Ccd.SetSpeedAsync(expectedSpeedTokenAfter);
        int returnedSpeedTokenAfter = await _fixture.Ccd.GetSpeedAsync();

        // Assert
        returnedSpeedTokenBefore.Should().Be(expectedSpeedTokenBefore);
        returnedSpeedTokenAfter.Should().Be(expectedSpeedTokenAfter);
    }

    [Fact]
    public async Task GivenCcd_WhenSettingExposureTime_ThenSetsExposureTimeCorrectly()
    {
        // Arrange
        var time = 1234;
        
        // Act
        await _fixture.Ccd.SetExposureTimeAsync(time);
        var actualTime = await _fixture.Ccd.GetExposureTimeAsync();

        // Assert
        actualTime.Should().Be(time);
    }

    [Fact]
    public async Task GivenCcd_WhenSettingXAxisConversionType_ThenSetsXAxisConversionType()
    {
        // Arrange
        var targetConversionType = ConversionType.FromCcdFirmware;

        // Act
        await _fixture.Ccd.SetXAxisConversionTypeAsync(targetConversionType);
        var actual = await _fixture.Ccd.GetXAxisConversionTypeAsync();
        
        // Assert
        actual.Should().Be(targetConversionType);
    }

    [Theory]
    [InlineData(TimerResolution.Millisecond, 0)]
    [InlineData(TimerResolution.Microsecond, 1, Skip = "Not all hardware supports this setting")]
    public async Task GivenCcd_WhenSettingTimeResolution_ThenSetsTheTimeResolution(TimerResolution targetResolution, int expectedMicroseconds)
    {
        // Act
        await _fixture.Ccd.SetTimerResolutionAsync(targetResolution);
        var actual = await _fixture.Ccd.GetTimerResolutionAsync();

        // Assert
        actual.Should().Be(expectedMicroseconds);
    }

    [Fact]
    public async Task GivenCcd_WhenSettingAcquisitionCount_ThenSetsTheAcquisitionCount()
    {
        // Arrange
        var targetCount = 5;

        // Act
        await _fixture.Ccd.SetAcquisitionCountAsync(targetCount);
        var actual = await _fixture.Ccd.GetAcquisitionCountAsync();

        // Assert
        actual.Should().Be(targetCount);
    }

    [Fact]
    public async Task GivenCcd_WhenSettingCleanCount_ThenSetsCleanCount()
    {
        // Act
        await _fixture.Ccd.SetCleanCountAsync(5, CleanCountMode.Never);
        var actual = await _fixture.Ccd.GetCleanCountAsync();

        // Assert
        actual.MatchSnapshot();
    }

    [Fact]
    public async Task GivenCcd_WhenReadingDeviceConfiguration_ThenReturnsConsistentDeviceConfiguration()
    {
        // Act
        var config = await _fixture.Ccd.GetDeviceConfigurationAsync();

        // Assert
        Assert.True(config.Count > 1);
    }

    [Fact]
    public async Task GivenCcd_WhenGettingChipSize_ThenReturnsConsistentSize()
    {
        // Act
        var size = await _fixture.Ccd.GetChipSizeAsync();

        // Assert
        size.MatchSnapshot();
    }

    [Fact]
    public async Task GivenCcd_WhenGettingChipTemperature_ThenReturnsConsistentTemperature()
    {
        // Act
        var temp = await _fixture.Ccd.GetChipTemperatureAsync();

        // Assert
        temp.Should().BeLessThan(-40);
    }

    [Fact]
    public async Task GivenCcd_WhenGettingDataSize_ThenDataSizeGetsReturned()
    {
        // Act
        var size = await _fixture.Ccd.GetDataSizeAsync();
        
        // Assert
        size.Should().Be(1024);
    }

    [Fact]
    public async Task GivenCcd_WhenReadingDataFromDevice_ThenRetrievingDataWorksAsConfigured()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
        // Arrange
        RegionOfInterest myRegion = new RegionOfInterest(1,0,0,16,4,1,4);
        await _fixture.Ccd.SetAcquisitionCountAsync(1);
        await _fixture.Ccd.SetExposureTimeAsync(100);
        await _fixture.Ccd.SetRegionOfInterestAsync(myRegion);
        await _fixture.Ccd.SetXAxisConversionTypeAsync(ConversionType.None);
        List<List<float>> fixedYdata = new List<List<float>>
        {
            new List<float> { 1, 2, 3, 4 },
        };

        // Act
        CcdData returnedCcDData = null;
        if (await _fixture.Ccd.GetAcquisitionReadyAsync())
        {
            await _fixture.Ccd.AcquisitionStartAsync(true);
            await _fixture.Ccd.WaitForDeviceNotBusy(TimeSpan.FromSeconds(1));
            returnedCcDData = await _fixture.Ccd.GetAcquisitionDataAsync();

        }

        returnedCcDData.Acquisition[0].Region[0].YData = fixedYdata;
        returnedCcDData.Timestamp = "replacedWithStaticString";
        returnedCcDData.MatchSnapshot();
        
        // Assert
        returnedCcDData.Should().NotBeNull();
        returnedCcDData.Acquisition.Count.Should().Be(1);
        returnedCcDData.Acquisition[0].Region.Count.Should().Be(1);
        returnedCcDData.Acquisition[0].Region[0].XData.Count.Should().Be(16);
        returnedCcDData.Acquisition[0].Region[0].Index.Should().Be(1);
        returnedCcDData.Acquisition[0].Region[0].XBinning.Should().Be(1);
        returnedCcDData.Acquisition[0].Region[0].XOrigin.Should().Be(0);
        returnedCcDData.Acquisition[0].Region[0].XSize.Should().Be(16);
        returnedCcDData.Acquisition[0].Region[0].YBinning.Should().Be(4);
        returnedCcDData.Acquisition[0].Region[0].YOrigin.Should().Be(0);
        returnedCcDData.Acquisition[0].Region[0].YSize.Should().Be(4);
    }

    [Fact]
    public async Task GivenCcd_WhenSettingSpecificTrigger_ThenTriggerIsProperlySet()
    {
        // Arrange
        var initial = await _fixture.Ccd.GetTriggerInAsync();
        var target = new Trigger(TriggerAddress.Input, TriggerEvent.Once, TriggerSignalType.FallingEdge);
        
        // Act
        await _fixture.Ccd.SetTriggerInAsync(target, true);
        await _fixture.Ccd.WaitForDeviceNotBusy(TimeSpan.FromMilliseconds(350));
        var actual = await _fixture.Ccd.GetTriggerInAsync();
        
        // Assert
        actual.Should().Be(target);

        // Cleanup & resetted state for triggering
        await _fixture.Ccd.RestartDeviceAsync();
        await Task.Delay(10000);
        var final = await _fixture.Ccd.GetTriggerInAsync();
        final.Should().Be(initial);  
    }

    [Fact]
    public async Task GivenCcd_WhenSettingSpecificSignal_ThenSignalIsProperlySet()
    {
        // Arrange
        var target = new Signal(SignalAddress.Output, SignalEvent.StartExperiment, SignalType.ActiveHigh);
        
        // Act
        await _fixture.Ccd.SetSignalOutAsync(target, true);
        await _fixture.Ccd.WaitForDeviceNotBusy(TimeSpan.FromMilliseconds(350));
        var actual = await _fixture.Ccd.GetSignalOutAsync();
        
        // Assert
        actual.Should().Be(target);
    }

    //Commented out this test because the ccd at Zuehlke doesn't have any parallel speed tokens in the configs
    //[Fact]
    //public async Task GivenCcd_WhenSettingParallelSpeed_ThenParallelSpeedIsProperlySet()
    //{
    //    // Arrange
    //    int expectedParallelSpeedTokenBefore = 0;
    //    int expectedParallelSpeedTokenAfter = 1;

    //    // Act
    //    await _fixture.Ccd.SetParallelSpeedAsync(expectedParallelSpeedTokenBefore);
    //    int returnedParallelSpeedTokenBefore = await _fixture.Ccd.GetParallelSpeedAsync();
    //    await _fixture.Ccd.SetParallelSpeedAsync(expectedParallelSpeedTokenAfter);
    //    int returnedParallelSpeedTokenAfter = await _fixture.Ccd.GetParallelSpeedAsync();

    //    // Assert
    //    returnedParallelSpeedTokenBefore.Should().Be(expectedParallelSpeedTokenBefore);
    //    returnedParallelSpeedTokenAfter.Should().Be(expectedParallelSpeedTokenAfter);
    //}

    [Fact]
    public async Task GivenCcd_WhenRemovingDarkCount_ThenRetrievingDataWorksAsConfigured()
    {
        // Arrange
        await _fixture.Ccd.SetAcquisitionCountAsync(1);
        await _fixture.Ccd.SetExposureTimeAsync(5);
        await _fixture.Ccd.SetRegionOfInterestAsync(RegionOfInterest.Default);
        await _fixture.Ccd.SetXAxisConversionTypeAsync(ConversionType.None);

        // Act
        // Fetching data with closed shuther
        List<List<float>> darkData = new List<List<float>>();
        if (await _fixture.Ccd.GetAcquisitionReadyAsync())
        {
            await _fixture.Ccd.AcquisitionStartAsync(false);
            await _fixture.Ccd.WaitForDeviceNotBusy(TimeSpan.FromSeconds(1));
            CcdData acquiredData = await _fixture.Ccd.GetAcquisitionDataAsync();
            darkData = acquiredData.Acquisition[0].Region[0].YData;
        }

        // Fetching normal data
        List<List<float>> normalData = new List<List<float>>();
        if (await _fixture.Ccd.GetAcquisitionReadyAsync())
        {
            await _fixture.Ccd.AcquisitionStartAsync(true);
            await _fixture.Ccd.WaitForDeviceNotBusy(TimeSpan.FromSeconds(1));
            CcdData acquiredData = await _fixture.Ccd.GetAcquisitionDataAsync();
            normalData = acquiredData.Acquisition[0].Region[0].YData;
        }

        // Calculating difference
        var diffData = new DarkCountSubstraction().SubtractData(normalData, darkData);

        diffData.Count.Should().Be(normalData.Count);
        diffData.Count.Should().Be(darkData.Count);
        
        for (var i = 0; i < diffData.Count; i++)
        {
            for (int j = 0; j < normalData[i].Count; j++)
            {
                diffData[i][j].Should().Be(normalData[i][j] - darkData[i][j]);
            }
        }
    }
    [Fact]
    public void GivenJsonString_WhenDeserializing_ThenReturnsCcdDataObject()
    {
        // Arrange
        var jsonString = "{\"acquisition\":[{\"acqIndex\":1,\"roi\":[{\"roiIndex\":1,\"xBinning\":1,\"xData\":[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15],\"xOrigin\":0,\"xSize\":16,\"yBinning\":4,\"yData\":[[602,601,602,604,602,599,602,603,603,602,600,602,602,601,602,602]],\"yOrigin\":0,\"ySize\":4}]}],\"timestamp\":\"2024.12.16 11:32:10.033\"}";

        // Act
        var ccdData = JsonConvert.DeserializeObject<CcdData>(jsonString);

        // Assert
        Assert.NotNull(ccdData);
        Assert.Equal("2024.12.16 11:32:10.033", ccdData.Timestamp);
        Assert.Single(ccdData.Acquisition);
        var acquisition = ccdData.Acquisition.First();
        Assert.Equal("1", acquisition.Index);
        Assert.Single(acquisition.Region);
        var region = acquisition.Region.First();
        Assert.Equal(1, region.Index);
        Assert.Equal(1, region.XBinning);
        Assert.Equal(0, region.XOrigin);
        Assert.Equal(16, region.XSize);
        Assert.Equal(4, region.YBinning);
        Assert.Equal(0, region.YOrigin);
        Assert.Equal(4, region.YSize);
        Assert.Equal(16, region.XData.Count);
        Assert.Equal(16, region.YData.First().Count);
    }
}