using System.ComponentModel;
using Horiba.Sdk.Enums;

namespace Horiba.Sdk.Tests;

public class DeviceManagerTests
{
    [Fact]
    public async Task GivenNewDeviceManager_WhenGettingDevices_ThenReturnsAvailableDevices()
    {
        // Arrange
        using var deviceManager = new DeviceManager();

        // Act
        await deviceManager.StartAsync();

        // Assert
        deviceManager.Monochromators.Should().NotBeNullOrEmpty();
        deviceManager.ChargedCoupledDevices.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GivenNewDeviceManager_WhenStartingIclProcess_ThenStartsIclProcess()
    {
        // Arrange
        using var manager = new DeviceManager();

        // Act
        await manager.StartAsync();

        // Assert
        manager.IclProcess.StartTime.Should().BeWithin(TimeSpan.FromMilliseconds(200));
    }

    [Fact]
    public async Task GivenNewDeviceManager_WhenStartingAndStoppingIclProcess_ThenIclIsStopped()
    {
        // Arrange
        using var manager = new DeviceManager();

        // Act
        await manager.StartAsync();
        await manager.StopAsync();

        // Assert
        manager.IclProcess.ExitTime.Should().BeWithin(TimeSpan.FromMilliseconds(200));
    }

    [Fact]
    public async Task GivenDeviceManagerWithNonExistingIclInstallation_WhenStartingDeviceManager_ThenExceptionIsThrown()
    {
        // Arrange
        using var manager = new DeviceManager("c:/notExistingIclPath/icl.exe");
        Win32Exception? expectedException = null;

        // Act
        try
        {
            await manager.StartAsync();
        }
        catch (Win32Exception? e)
        {
            expectedException = e;
        }

        // Assert
        expectedException.Should().NotBeNull();
    }

    [Fact]
    public async Task GivenMonoDevice_WhenOpeningConnection_ThenConnectionIsOpened()
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var mono = dm.Monochromators.First();
        
        // Act
        await mono.OpenConnectionAsync();
        var actual = await mono.IsConnectionOpenedAsync();

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public async Task GivenMonoDevice_WhenOpeningAndClosingConnection_ThenConnectionIsClosed()
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var mono = dm.Monochromators.First();
        
        // Act
        await mono.OpenConnectionAsync();
        await mono.CloseConnectionAsync();
        var actual = await mono.IsConnectionOpenedAsync();

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public async Task GivenMonoDevice_WhenMovingToPosition_ThenPositionIsUpdated()
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var mono = dm.Monochromators.First();
        await mono.OpenConnectionAsync();
        var target = 390;
        
        // Act
        await mono.MoveToWavelengthAsync(target);
        var actual = await mono.GetCurrentWavelengthAsync();

        // Assert
        actual.Should().Be(target);
    }

    [Theory]
    [InlineData(Grating.First)]
    [InlineData(Grating.Second)]
    [InlineData(Grating.Third)]
    public async Task GivenMonoDevice_WhenMovingToGratingPosition_ThenGratingPositionIsUpdated(Grating target)
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var mono = dm.Monochromators.First();
        await mono.OpenConnectionAsync();
        
        // Act
        await mono.SetTurretGratingAsync(target);
        var actual = await mono.GetTurretGratingAsync();

        // Assert
        actual.Should().Be(target);
    }

    [Theory]
    [InlineData(FilterWheelPosition.Red)]
    [InlineData(FilterWheelPosition.Green)]
    [InlineData(FilterWheelPosition.Blue)]
    [InlineData(FilterWheelPosition.Yellow)]
    public async Task GivenMonoDevice_WhenMovingFilterPosition_ThenFilterPositionIsUpdated(FilterWheelPosition target)
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var mono = dm.Monochromators.First();
        await mono.OpenConnectionAsync();
        
        // Act
        await mono.SetFilterWheelPositionAsync(target);
        var actual = await mono.GetFilterWheelPositionAsync();

        // Assert
        actual.Should().Be(target);
    }

    [Theory]
    [InlineData(Mirror.First, MirrorPosition.Axial)]
    [InlineData(Mirror.First, MirrorPosition.Literal)]
    [InlineData(Mirror.Second, MirrorPosition.Axial)]
    [InlineData(Mirror.Second, MirrorPosition.Literal)]
    public async Task GivenMonoDevice_WhenMovingMirrorToPosition_ThenMirrorPositionIsChanged(Mirror mirror, MirrorPosition target)
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var mono = dm.Monochromators.First();
        await mono.OpenConnectionAsync();
        
        // Act
        await mono.SetMirrorPositionAsync(mirror, target);
        var actual = await mono.GetMirrorPosition(mirror);

        // Assert
        actual.Should().Be(target);
    }

    [Theory]
    [InlineData(Slit.A, 5.2)]
    [InlineData(Slit.A, 8.7)]
    [InlineData(Slit.B, 3.7)]
    [InlineData(Slit.B, 6.2)]
    [InlineData(Slit.C, 5.2)]
    [InlineData(Slit.C, 4.9)]
    [InlineData(Slit.D, 6.3)]
    [InlineData(Slit.D, 7.4)]
    public async Task GivenMonoDevice_WhenMovingSlitPosition_ThenSlitPositionIsUpdated(Slit slit, float targetPosition)
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var mono = dm.Monochromators.First();
        await mono.OpenConnectionAsync();
        
        // Act
        await mono.SetSlitPositionAsync(slit, targetPosition);
        var actual = await mono.GetSlitPositionInMMAsync(slit);

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
    [InlineData(Slit.C, SlitStepPosition.A)]
    [InlineData(Slit.C, SlitStepPosition.B)]
    [InlineData(Slit.C, SlitStepPosition.C)]
    [InlineData(Slit.C, SlitStepPosition.D)]
    [InlineData(Slit.D, SlitStepPosition.A)]
    [InlineData(Slit.D, SlitStepPosition.B)]
    [InlineData(Slit.D, SlitStepPosition.C)]
    [InlineData(Slit.D, SlitStepPosition.D)]
    public async Task GivenMonoDevice_WhenMovingSlitStepPosition_ThenSlitStepPositionIsUpdated(Slit slit, SlitStepPosition targetPosition)
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var mono = dm.Monochromators.First();
        await mono.OpenConnectionAsync();
        
        // Act
        await mono.SetSlitStepPositionAsync(slit, targetPosition);
        var actual = await mono.GetSlitStepPositionAsync(slit);

        // Assert
        actual.Should().Be(targetPosition);
    }

    [Fact]
    public async Task GivenMonoDevice_WhenOpeningShutter_ThenShutterIsOpened()
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var mono = dm.Monochromators.First();
        await mono.OpenConnectionAsync();
        
        // Act
        await mono.CloseShutterAsync();
        await mono.OpenShutterAsync();
        var actual = await mono.GetShutterPositionAsync();
        
        // Assert
        actual.Should().Be(ShutterPosition.Opened);
    }

    [Fact]
    public async Task GivenMonoDevice_WhenClosingShutter_ThenShutterIsClosed()
    {
        // Arrange
        var dm = new DeviceManager();
        await dm.StartAsync();
        var mono = dm.Monochromators.First();
        await mono.OpenConnectionAsync();
        
        // Act
        await mono.OpenShutterAsync();
        await mono.CloseShutterAsync();
        var actual = await mono.GetShutterPositionAsync();
        
        // Assert
        actual.Should().Be(ShutterPosition.Closed);
    }
}