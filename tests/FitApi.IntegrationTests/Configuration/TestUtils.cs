using System.Text.Json;
using System.Text.Json.Serialization;

namespace FitApi.IntegrationTests.Configuration;

public static class TestUtils
{
    public static JsonSerializerOptions SerializationOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        
        return options;
    }
}