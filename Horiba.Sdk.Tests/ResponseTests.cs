using Newtonsoft.Json;
using System.Collections.Generic;
namespace Horiba.Sdk.Tests;


public class ResponseTests
{
    [Fact]
    public void ToString_ShouldReturnFormattedJsonString()
    {
        // Arrange
        var response = new Response(
            Id: 1,
            CommandName: "TestCommand",
            Results: new Dictionary<string, object> { { "key1", "value1" }, { "key2", 2 } },
            Errors: new List<string> { "Error1", "Error2" }
        );

        // Act
        var jsonString = response.ToString();

        // Assert
        var expectedJsonString = JsonConvert.SerializeObject(response, Formatting.Indented);
        Assert.Equal(expectedJsonString, jsonString);
    }
}