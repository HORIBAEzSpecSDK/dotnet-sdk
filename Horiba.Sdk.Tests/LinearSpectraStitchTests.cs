using Horiba.Sdk.Stitching;

namespace Horiba.Sdk.Tests;

public class LinearSpectraStitchTests
{
    [Fact]
    public void GivenTwoSpectrums_WhenStitch_DataStitchedCorrectly()
    {
        // Arrange
        var data = new List<XYData>[]
        {
            [
                new(1, 5),
                new(2, 6),
                new(3, 7),
            ],
            [
                new(2, 10),
                new(3, 11),
                new(4, 12),
            ]
        };

        // Act
        var stitchedData = new LinearSpectraStitch(data).Stitch();

        // Assert
        var expectedData = new List<XYData>
        {
            new(1, 5),
            new(2, 8),
            new(3, 9),
            new(4, 12),
        };

        stitchedData.Should().BeEquivalentTo(expectedData);
    }

    [Fact]
    public void GivenFourSpectrums_WhenStitch_DataStitchedCorrectly()
    {

        // Arrange
        var data = new List<XYData>[]
        {
            [
                new(1, 5),
                new(2, 6),
                new(3, 7),
            ],
            [
                new(2, 7),
                new(3, 8),
                new(4, 9),
            ],
            [
                new(3, 10),
                new(4, 11),
                new(5, 12),
            ],
            [
                new(4, 13),
                new(5, 14),
                new(6, 15),
            ]
        };

        // Act
        var stitchedData = new LinearSpectraStitch(data).Stitch();

        // Assert
        var expectedData = new List<XYData>
        {
            new(1, 5),
            new(2, 6.5f),
            new(3, 8.75f),
            new(4, 11.5f),
            new(5, 13),
            new(6, 15)
        };

        stitchedData.Should().BeEquivalentTo(expectedData);
    }

    [Fact]
    public void GivenFourSpectrumsWithAllDataOverlapping_WhenStitch_DataStitchedCorrectly()
    {

        // Arrange
        var data = new List<XYData>[]
        {
            [
                new(1, 5),
                new(2, 6),
                new(3, 7),
            ],
            [
                new(1, 5),
                new(2, 6),
                new(3, 7),
            ],
            [
                new(1, 5),
                new(2, 6),
                new(3, 7),
            ],
            [
                new(1, 5),
                new(2, 6),
                new(3, 7),
            ]
        };

        // Act
        var stitchedData = new LinearSpectraStitch(data).Stitch();

        // Assert
        var expectedData = new List<XYData>
        {
            new(1, 5),
            new(2, 6),
            new(3, 7),
        };

        stitchedData.Should().BeEquivalentTo(expectedData);
    }
}
