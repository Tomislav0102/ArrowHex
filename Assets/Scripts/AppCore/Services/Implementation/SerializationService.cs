using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine.Scripting;

/// <summary>
/// Service used for JSON serialization/deserialization, implements <see cref="ISerializationService"/>.
/// </summary>
public class SerializationService : ISerializationService
{

    /// <summary>
    /// The default <see cref="JsonSerializerSettings"/>.
    /// </summary>
    private static readonly JsonSerializerSettings DefaultSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto
    };

    [Preserve]
    [UsedImplicitly]
    public SerializationService() { }

    /// <inheritdoc/>
    public T Deserialize<T>(string json, JsonSerializerSettings settings = null)
    {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        settings ??= DefaultSerializerSettings;

        return JsonConvert.DeserializeObject<T>(json, settings);
    }

    /// <inheritdoc/>
    public string Serialize<T>(T value, JsonSerializerSettings settings = null)
    {
        settings ??= DefaultSerializerSettings;

        return JsonConvert.SerializeObject(value, Formatting.Indented, settings);
    }

}

