using System.ComponentModel;
using Horiba.Sdk.Enums;
using Newtonsoft.Json;

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

    [Fact]
    public async Task GivenDeviceManager_WhenGetCountOfCcd_ThenReturnsCountAfterDiscovery()
    {
        // Arrange
        using var deviceManager = new DeviceManager();
        await deviceManager.StartAsync();
        
        // Act
        var ccdListCount = await deviceManager.GetCcdCountAsync();

        // Assert
        ccdListCount.Should().BeGreaterOrEqualTo(1);
    }

    [Fact]
    public async Task GivenDeviceManager_WhenGetCountOfMonochromators_ThenReturnsCountAfterDiscovery()
    {
        // Arrange
        using var deviceManager = new DeviceManager();
        await deviceManager.StartAsync();
        
        // Act
        var monoListCount = await deviceManager.GetMonochromatorCountAsync();

        // Assert
        monoListCount.Should().BeGreaterOrEqualTo(1);
    }

    [Fact]
    public void GivenAcquisitionData_WhenDeserializing_ThenHasCorrectClassStructure()
    {
        // Arrange
        var result =
            "[\n  {\n    \"acqIndex\": 1,\n    \"roi\": [\n      {\n        \"roiIndex\": 1,\n        \"xBinning\": 1,\n        \"xOrigin\": 0,\n        \"xSize\": 1024,\n        \"xyData\": [\n          [\n            0,\n            610\n          ],\n          [\n            1,\n            607\n          ],\n          [\n            2,\n            606\n          ],\n          [\n            3,\n            607\n          ],\n        \"yBinning\": 256,\n        \"yOrigin\": 0,\n        \"ySize\": 256\n      }\n    ]\n  }\n]";
        
        //Act
        var obj = JsonConvert.DeserializeObject<List<AcquisitionDescription>>(result);
            
        // Assert
        obj.Should().NotBeNull();
    }
}