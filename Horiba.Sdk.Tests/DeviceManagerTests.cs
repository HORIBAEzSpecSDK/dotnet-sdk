using FluentAssertions;
using Horiba.Sdk.Commands;
using Horiba.Sdk.Communication;
using Horiba.Sdk.Devices;
using Snapshooter.Xunit;

namespace Horiba.Sdk.Tests;

public class UnitTest1
{
    [Fact]
    public async Task GivenNewDeviceManager_WhenGettingDevices_ThenReturnsAvailableDevices()
    {
        // Arrange
        var deviceManager = new DeviceManager();
        
        // Act
        await deviceManager.DiscoverDevicesAsync();
        
        // Assert
        deviceManager.Monochromators.Should().NotBeNullOrEmpty();
        deviceManager.ChargedCoupledDevices.Should().NotBeNullOrEmpty();
    }
}