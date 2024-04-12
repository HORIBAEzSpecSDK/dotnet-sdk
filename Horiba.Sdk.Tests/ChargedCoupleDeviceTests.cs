using Horiba.Sdk.Enums;

namespace Horiba.Sdk.Tests;

public class ChargedCoupleDeviceTests : IClassFixture<ChargedCoupleDeviceTestFixture>
{
    private readonly ChargedCoupleDeviceTestFixture _fixture;

    public ChargedCoupleDeviceTests(ChargedCoupleDeviceTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Theory]
    [InlineData(Gain.HighLight)]
    [InlineData(Gain.BestDynamicRange)]
    [InlineData(Gain.HighSensitivity)]
    public async Task GivenCcd_WhenTriggeringSetGainMethod_ThenGainIsSet(Gain expectedSetting)
    {
        // Act
        await _fixture.Ccd.SetGainAsync(expectedSetting);
        var actualSetting = await _fixture.Ccd.GetGainAsync();

        // Assert
        actualSetting.Should().HaveSameValueAs(expectedSetting);
    }

    [Fact]
    public async Task GivenCcd_WhenTriggeringSetFitParameters_ThenFitParametersAreSet()
    {
        // Arrange
        var expectedParams = "1,1,1,1";

        // Act
        await _fixture.Ccd.SetFitParametersAsync(expectedParams);
        var actual = await _fixture.Ccd.GetFitParametersAsync();

        // Assert
        actual.Should().BeSameAs(expectedParams);
    }

    [Fact]
    public async Task GivenCcd_WhenGettingActualData_ThenReturnsByteData()
    {
        // Arrange
        await _fixture.Ccd.OpenConnectionAsync();
        await _fixture.Ccd.SetAcquisitionCountAsync(1);
        await _fixture.Ccd.SetXAxisConversionTypeAsync(ConversionType.None);
        await _fixture.Ccd.SetExposureTimeAsync(1000);
        await _fixture.Ccd.SetRegionOfInterestAsync(RegionOfInterest.Default);
        
        // Act
        var isReady = await _fixture.Ccd.GetAcquisitionReadyAsync();
        await _fixture.Ccd.SetAcquisitionStartAsync(true);
        await _fixture.Ccd.WaitForDeviceNotBusy();
        var data = await _fixture.Ccd.GetAcquisitionDataAsync();

        // Assert
        data.MatchSnapshot();
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

    [Theory]
    [InlineData(Speed.Slow)]
    [InlineData(Speed.Medium)]
    [InlineData(Speed.Fast)]
    public async Task GivenCcd_WhenSettingSpeed_ThenSpeedIsUpdated(Speed targetSpeed)
    {
        // Act
        await _fixture.Ccd.SetSpeedAsync(targetSpeed);
        var speed = await _fixture.Ccd.GetSpeedAsync();

        // Assert
        speed.Should().Be(targetSpeed);
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

    [Fact]
    public async Task GivenCcd_WhenSettingNumberOfAverages_ThenSetsTheNumberOfAverages()
    {
        // Arrange
        var targetNumber = 5;

        // Act
        await _fixture.Ccd.SetNumberOfAveragesAsync(targetNumber);
        var actual = await _fixture.Ccd.GetNumberOfAveragesAsync();

        // Assert
        actual.Should().Be(targetNumber);
    }

    [Fact]
    public async Task GivenCcd_WhenSettingTimeResolution_ThenSetsTheTimeResolution()
    {
        // Arrange
        var targetResolution = 500;

        // Act
        await _fixture.Ccd.SetTimerResolutionAsync(targetResolution);
        var actual = await _fixture.Ccd.GetTimerResolutionAsync();

        // Assert
        actual.Should().Be(targetResolution);
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
        await _fixture.Ccd.SetCleanCountAsync(5, CleanCountMode.Mode1);
        var actual = await _fixture.Ccd.GetCleanCountAsync();

        // Assert
        actual.MatchSnapshot();
    }

    [Fact]
    public async Task GivenCcd_WhenReadingDeviceConfiguration_ThenReturnsConsistentDeviceConfiguration()
    {
        // Act
        var cofig = await _fixture.Ccd.GetDeviceConfigurationAsync();

        // Assert
        cofig.MatchSnapshot();
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
        temp.Should().BeApproximately(-60, 0.1f);
    }
}