using Horiba.Sdk.Enums;

namespace Horiba.Sdk.Tests;

public class MonochromatorTests : IClassFixture<MonochromatorTestFixture>
{
    private readonly MonochromatorTestFixture _fixture;

    public MonochromatorTests(MonochromatorTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task GivenMonoDevice_WhenOpeningConnection_ThenConnectionIsOpened()
    {
        // Act
        var actual = await _fixture.Mono.IsConnectionOpenedAsync();

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public async Task GivenMonoDevice_WhenOpeningAndClosingConnection_ThenConnectionIsClosed()
    {
        // Act
        await _fixture.Mono.WaitForDeviceNotBusy();
        await _fixture.Mono.CloseConnectionAsync();
        await _fixture.Mono.WaitForDeviceNotBusy();
        var actual = await _fixture.Mono.IsConnectionOpenedAsync();

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public async Task GivenMonoDevice_WhenMovingToPosition_ThenPositionIsUpdated()
    {
        // Arrange
        float target = 390;
        
        // Act
        await _fixture.Mono.MoveToWavelengthAsync(target);
        await _fixture.Mono.WaitForDeviceNotBusy();
        var actual = await _fixture.Mono.GetCurrentWavelengthAsync();

        // Assert
        actual.Should().BeApproximately(target, 1f);
    }

    [Theory]
    [InlineData(Grating.First)]
    [InlineData(Grating.Second)]
    [InlineData(Grating.Third)]
    public async Task GivenMonoDevice_WhenMovingToGratingPosition_ThenGratingPositionIsUpdated(Grating target)
    {
        // Act
        await _fixture.Mono.SetTurretGratingAsync(target);
        // NOTE: The timeout the device needs is different depending on the combination of the
        // current position and the target position
        await _fixture.Mono.WaitForDeviceNotBusy(120_000); // 2 min
        var actual = await _fixture.Mono.GetTurretGratingAsync();

        // Assert
        actual.Should().Be(target);
    }

    [Theory(Skip = "The available device does not have a wheel installed. Hardware feature is missing")]
    [InlineData(FilterWheelPosition.Red)]
    [InlineData(FilterWheelPosition.Green)]
    [InlineData(FilterWheelPosition.Blue)]
    [InlineData(FilterWheelPosition.Yellow)]
    public async Task GivenMonoDevice_WhenMovingFilterPosition_ThenFilterPositionIsUpdated(FilterWheelPosition target)
    {
        // Act
        await _fixture.Mono.SetFilterWheelPositionAsync(target);
        var actual = await _fixture.Mono.GetFilterWheelPositionAsync();

        // Assert
        actual.Should().Be(target);
    }

    [Theory]
    [InlineData(Mirror.First, MirrorPosition.Axial)]
    [InlineData(Mirror.First, MirrorPosition.Lateral)]
    [InlineData(Mirror.Second, MirrorPosition.Axial)]
    [InlineData(Mirror.Second, MirrorPosition.Lateral)]
    public async Task GivenMonoDevice_WhenMovingMirrorToPosition_ThenMirrorPositionIsChanged(Mirror mirror, MirrorPosition target)
    {
        // NOTE: needed just to have some delay between sequential test runs
        Task.Delay(TimeSpan.FromMilliseconds(1)).Wait();
        
        // Act
        var initialPosition = await _fixture.Mono.GetMirrorPosition(mirror);
        await _fixture.Mono.SetMirrorPositionAsync(mirror, target);
        await _fixture.Mono.WaitForDeviceNotBusy(5000);
        var finalPosition = await _fixture.Mono.GetMirrorPosition(mirror);

        // Assert
        finalPosition.Should().Be(target);
    }

    [Theory]
    [InlineData(Slit.A, 5.2)]
    [InlineData(Slit.A, 1)]
    [InlineData(Slit.A, 8.7)]
    [InlineData(Slit.B, 0)]
    [InlineData(Slit.B, 1.7)]
    [InlineData(Slit.B, 6.2)]
    [InlineData(Slit.C, 5.2, Skip = "This slit is missing on the device")]
    [InlineData(Slit.C, 4.9, Skip = "This slit is missing on the device")]
    [InlineData(Slit.D, 6.3)]
    [InlineData(Slit.D, 7.4)]
    public async Task GivenMonoDevice_WhenMovingSlitPosition_ThenSlitPositionIsUpdated(Slit slit, float targetPosition)
    {
        Task.Delay(TimeSpan.FromMilliseconds(600)).Wait();
        
        // Act
        await _fixture.Mono.SetSlitPositionAsync(slit, targetPosition);
        await _fixture.Mono.WaitForDeviceNotBusy(7000);
        var actual = await _fixture.Mono.GetSlitPositionInMMAsync(slit);

        // Assert
        actual.Should().Be(targetPosition);
    }

    [Theory]
    [InlineData(Slit.A, SlitStepPosition.A)]
    [InlineData(Slit.A, SlitStepPosition.B)]
    [InlineData(Slit.A, SlitStepPosition.C)]
    [InlineData(Slit.A, SlitStepPosition.D)]
    [InlineData(Slit.B, SlitStepPosition.A)]
    [InlineData(Slit.B, SlitStepPosition.B)]
    [InlineData(Slit.B, SlitStepPosition.C)]
    [InlineData(Slit.B, SlitStepPosition.D)]
    [InlineData(Slit.C, SlitStepPosition.A, Skip = "This slit is missing on the device")]
    [InlineData(Slit.C, SlitStepPosition.B, Skip = "This slit is missing on the device")]
    [InlineData(Slit.C, SlitStepPosition.C, Skip = "This slit is missing on the device")]
    [InlineData(Slit.C, SlitStepPosition.D, Skip = "This slit is missing on the device")]
    [InlineData(Slit.D, SlitStepPosition.A)]
    [InlineData(Slit.D, SlitStepPosition.B)]
    [InlineData(Slit.D, SlitStepPosition.C)]
    [InlineData(Slit.D, SlitStepPosition.D)]
    public async Task GivenMonoDevice_WhenMovingSlitStepPosition_ThenSlitStepPositionIsUpdated(Slit slit, SlitStepPosition targetPosition)
    {
        Task.Delay(TimeSpan.FromMicroseconds(100)).Wait();
        
        // Act
        await _fixture.Mono.SetSlitStepPositionAsync(slit, targetPosition);
        await _fixture.Mono.WaitForDeviceNotBusy(7000);
        var actual = await _fixture.Mono.GetSlitStepPositionAsync(slit);

        // Assert
        actual.Should().Be(targetPosition);
    }

    [Fact(Skip = "Expected error [E];-519;Mono must be configured for internal shutter mode")]
    public async Task GivenMonoDevice_WhenOpeningShutter_ThenShutterIsOpened()
    {
        // Act
        await _fixture.Mono.CloseShutterAsync();
        await _fixture.Mono.OpenShutterAsync();
        var actual = await _fixture.Mono.GetShutterPositionAsync();
        
        // Assert
        actual.Should().Be(ShutterPosition.Opened);
    }

    [Fact(Skip = "Expected error: [E];-519;Mono must be configured for internal shutter mode")]
    public async Task GivenMonoDevice_WhenClosingShutter_ThenShutterIsClosed()
    {
        // Act
        await _fixture.Mono.OpenShutterAsync();
        await _fixture.Mono.WaitForDeviceNotBusy();
        await _fixture.Mono.CloseShutterAsync();
        await _fixture.Mono.WaitForDeviceNotBusy();
        var actual = await _fixture.Mono.GetShutterPositionAsync();
        
        // Assert
        actual.Should().Be(ShutterPosition.Closed);
    }

    [Fact]
    public async Task GivenMono_WhenReadingConfiguration_ThenReturnsConsistentConfiguration()
    {
        // Act
        var config = await _fixture.Mono.GetDeviceConfigurationAsync();

        // Assert
        config.MatchSnapshot();
    }
}