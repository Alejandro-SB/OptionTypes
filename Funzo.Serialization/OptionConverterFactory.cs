using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Funzo.Serialization;

/// <summary>
/// Factory to create a JsonConverter for <see cref="Option{T}"/>
/// </summary>
public class OptionConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Option<>);

    /// <inheritdoc />
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert.IsGenericType &&
                typeToConvert.GetGenericTypeDefinition() == typeof(Option<>));

        var elementType = typeToConvert.GetGenericArguments()[0];
        var converter = (JsonConverter)Activator.CreateInstance(
                typeof(OptionConverter<>)
            .MakeGenericType(new[] { elementType }))!;

        return converter;
    }
}

/// <summary>
/// JsonConverter for <see cref="Option{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class OptionConverter<T> : JsonConverter<Option<T>>
{
    /// <inheritdoc />
    public override Option<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var deserializedValue = JsonSerializer.Deserialize<OptionRepresentation<T>>(ref reader, options) ?? throw new JsonException();

        return deserializedValue.HasValue
            ? Option.Some(deserializedValue.Value!)
            : Option<T>.None();
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Option<T> option, JsonSerializerOptions options)
    {
        var hasValue = option.IsSome(out var value);

        object serializedRepresentation = hasValue ?
            new OptionSomeRepresentation<T>
            {
                Value = value!
            }
            : new OptionNoneRepresentation();

        var content = JsonSerializer.Serialize(serializedRepresentation, options);

        writer.WriteRawValue(content);
    }
}