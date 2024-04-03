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
    public async Task GivenCcdDiscovery_WhenExtractingRawDeviceDescription_ThenReturnsCorrectDeviceDescription()
    {
        // Arrange
        var discovery = new ChargedCoupleDeviceDeviceDiscovery(null);
        var rawInput =
            "{\n    \"index0: %2\": \"{ productId: 13, deviceType: HORIBA Scientific Syncerity, serialNumber: Camera SN:  2244}\"\n  }";
        
        // Act
        var description = discovery.ExtractDescription(rawInput);
        
        // Assert
        description.MatchSnapshot();
    }

    [Fact]
    public async Task GivenMonoDiscovery_WhenExtractingRawDeviceDescription_ThenReturnsCorrectDeviceDescription()
    {
        // Arrange
        var discovery = new MonochromatorDeviceDiscovery(null);
        var rawInput =
            "{\n\t\"command\": \"mono_list\",\n\t\"errors\": [],\n\t\"id\": 0,\n\t\"results\": {\n\t\t\"list\": [\n\t\t\t\"0;HORIBA Scientific iHR;1745B-2017-iHR320       \"\n\t\t]\n\t}\n}";
        
        // Act
        var description = discovery.ExtractDescription(rawInput);
        
        // Assert
        description.MatchSnapshot();
    }
}