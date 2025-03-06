using Horiba.Sdk.Enums;

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
}