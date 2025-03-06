namespace Horiba.Sdk.Tests;

public class SpectrAcqTests
{
    private readonly SpectrAcqDeviceTestFixture _fixture;

    public SpectrAcqTests(SpectrAcqDeviceTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task GivenMonoDevice_WhenOpeningConnection_ThenConnectionIsOpened()
    {
        // Act
        var actual = await _fixture.Saq.IsConnectionOpenedAsync();

        // Assert
        actual.Should().BeTrue();
    }
}