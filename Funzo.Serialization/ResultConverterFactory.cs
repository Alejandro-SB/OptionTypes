using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Funzo.Serialization;

/// <summary>
/// Factory to create a JsonConverter for <see cref="Result{TOk, TErr}"/>
/// </summary>
public class ResultConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Result<,>);

    /// <inheritdoc />
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert.IsGenericType &&
                typeToConvert.GetGenericTypeDefinition() == typeof(Result<,>));

        var genericTypes = typeToConvert.GetGenericArguments();
        var okType = genericTypes[0];
        var errType = genericTypes[1];
        var converter = (JsonConverter)Activator.CreateInstance(
                typeof(ResultConverter<,>)
            .MakeGenericType(new[] { okType, errType }))!;

        return converter;
    }
}

/// <summary>
/// JsonConverter for <see cref="Result{TOk, TErr}"/>
/// </summary>
/// <typeparam name="TOk"></typeparam>
/// <typeparam name="TErr"></typeparam>
public class ResultConverter<TOk, TErr> : JsonConverter<Result<TOk, TErr>>
{
    /// <inheritdoc />
    public override Result<TOk, TErr>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var deserializedValue = JsonSerializer.Deserialize<ResultRepresentation<TOk, TErr>>(ref reader, options) ?? throw new JsonException();

        return deserializedValue.IsOk
            ? Result<TOk, TErr>.Ok(deserializedValue.Ok!)
            : Result<TOk, TErr>.Err(deserializedValue.Err!);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Result<TOk, TErr> result, JsonSerializerOptions options)
    {
        var isErr = result.IsErr(out var ok, out var err);

        object representation = isErr
            ? new ResultErrRepresentation<TErr>(err!)
            : new ResultOkRepresentation<TOk>(ok!);

        var content = JsonSerializer.Serialize(representation, options);

        writer.WriteRawValue(content);
    }
}