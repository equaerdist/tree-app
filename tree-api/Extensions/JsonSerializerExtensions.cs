using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;


namespace tree_api.Extensions;

internal static class JsonSerializerExtensions
{
    private static readonly JsonSerializerSettings JsonOptions = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        Converters = { new StringEnumConverter() },
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.Indented
    };

    public static string Serialize(this object obj, JsonSerializerSettings? options = null)
    {
        string json = JsonConvert.SerializeObject(obj, options ?? JsonOptions);
        return json;
    }

    public static T Deserialize<T>(this string obj, JsonSerializerSettings? options = null)
    {
        var deserializedData = JsonConvert.DeserializeObject<T>(obj, options ?? JsonOptions) ?? throw new ArgumentNullException(nameof(obj));
        return deserializedData;
    }
}
