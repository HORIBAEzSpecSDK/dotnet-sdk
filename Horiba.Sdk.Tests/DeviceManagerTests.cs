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
    public async Task GivenNewDeviceManager_WhenGettingIclInfo_ThenReturnsConsistentResult()
    {
        // Arrange
        using var deviceManager = new DeviceManager();
        await deviceManager.StartAsync();
        
        // Act
        var info = await deviceManager.GetIclInfoAsync();

        // Assert
        info.MatchSnapshot();
    }
}