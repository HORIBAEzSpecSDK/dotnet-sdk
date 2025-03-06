﻿using Horiba.Sdk.Enums;

namespace Horiba.Sdk.Tests;

public class SpectrAcqTests : IClassFixture<SpectrAcqDeviceTestFixture>
{
    private readonly SpectrAcqDeviceTestFixture _fixture;

    public SpectrAcqTests(SpectrAcqDeviceTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task GivenSaqDevice_WhenOpeningConnection_ThenConnectionIsOpened()
    {
        // Act
        var actual = await _fixture.Saq.IsConnectionOpenedAsync();

        // Assert
        actual.Should().BeTrue();
    }
    
    [Fact]
    public async Task GivenSaqDevice_WhenOpeningAndClosingConnection_ThenConnectionIsClosed()
    {
        // Act
        await _fixture.Saq.OpenConnectionAsync();
        await _fixture.Saq.CloseConnectionAsync();
        var actual = await _fixture.Saq.IsConnectionOpenedAsync();

        // Assert
        actual.Should().BeFalse();
    }
    
    [Fact]
    public async Task GivenSaqDevice_WhenGettingSerialNumber_ThenCorrectSerialNumberIsReturned()
    {
        //Arrange
        var expectedSerialNumber = "SNPG18010036";
        
        // Act
        await _fixture.Saq.WaitForDeviceNotBusy();
        var serialNumber = await _fixture.Saq.GetSerialNumberAsync();

        // Assert
        serialNumber.Should().Be(expectedSerialNumber);
    }


}