using System.ComponentModel;

namespace Horiba.Sdk.Tests;

public class DeviceManagerTests
{
    [Fact]
    public async Task GivenNewDeviceManager_WhenGettingDevices_ThenReturnsAvailableDevices()
    {
        // Arrange
        using var deviceManager = new DeviceManager();
        
        // Act
        await deviceManager.DiscoverDevicesAsync();
        
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
        await manager.StartAsync(true);

        // Assert
        manager.IclProcess.StartTime.Should().BeWithin(TimeSpan.FromMilliseconds(200));
    }

    [Fact]
    public async Task GivenNewDeviceManager_WhenStartingAndStoppingIclProcess_ThenIclIsStopped()
    {
        // Arrange
        using var manager = new DeviceManager();

        // Act
        await manager.StartAsync(true, false);
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
            await manager.StartAsync(true);
        }
        catch (Win32Exception? e)
        {
            expectedException = e;
        }

        // Assert
        expectedException.Should().NotBeNull();
    }
}