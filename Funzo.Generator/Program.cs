using System.Reflection;
using System.Text;

const string Namespace = "Funzo";
const string ClassName = "Union";
const string BaseClassName = $"{ClassName}Base";
static string GenericClassName(int ordinality, bool stripTypeNames = false) => $"{ClassName}<{GenerateGenericArguments(ordinality)}>";
static string GenericClassNameForDocs(int ordinality) => GenericClassName(ordinality).Replace("<", "{").Replace(">", "}");
static string GenericName(int n) => $"T{n}";
static string FieldName(int n) => $"_value{n}";

var root = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, @"..\..\..\.."));

var sb = new StringBuilder($@"#nullable enable
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace {Namespace};

/// <summary>
/// Base class for the union types
/// </summary>
public abstract class {BaseClassName}
{{
    internal abstract object GetValue();
}}
");

for (var i = 2; i < 6; i++)
{
    var content = GenerateClass(i);

    sb.AppendLine(content);
}

var filePath = Path.Combine(root, "Funzo", "Union.generated.cs");

File.WriteAllText(filePath, sb.ToString());

static string GenerateClass(int ordinality)
{
    var doc = @$"
/// <summary>
/// Class an entity that can be different types
/// </summary>
{string.Join(Environment.NewLine, (Enumerable.Range(0, ordinality).Select(x => $@"/// <typeparam name=""T{x}""></typeparam>")))}";

    return $@"
{doc}
public class {GenericClassName(ordinality)} : {BaseClassName}, IEquatable<{GenericClassName(ordinality)}>
    {string.Join(Environment.NewLine, Enumerable.Range(0, ordinality).Select(x => $"where {GenericName(x)} : notnull"))}
{{
    /// <summary>
    /// Stores lazily all the types of this instance to easily check if matches another one
    /// </summary>
    private static readonly Lazy<Type[]> UnionTypeDefinitions = new(() => typeof({GenericClassName(ordinality, true)}).GetGenericArguments());

    internal override object GetValue() =>
        _index switch
        {{
            {string.Join(Environment.NewLine, Enumerable.Range(0, ordinality).Select(x => $"{x} => {FieldName(x)}!,"))}

            _ => throw new IndexOutOfRangeException(""Union went out of range"")
        }};

    private readonly int _index;

    {GenerateFields(ordinality)}

    {GenerateConstructors(ordinality)}

    {GenerateImplicitOperators(ordinality)}

    /// <summary>
    /// Checks if this <see cref=""{GenericClassNameForDocs(ordinality)}"" /> instance is of type <typeparamref name=""T"" /> and gets the value if it is
    /// </summary>
    /// <remarks>
    /// NOTE: Please be aware that this method may return unexpected results if this <see cref=""{GenericClassNameForDocs(ordinality)}"" /> instance has two equal generic type parameters
    /// </remarks>
    /// <typeparam name=""T""></typeparam>
    /// <param name=""value"">If this instance is of type <typeparamref name=""T"" />, has its value assigned, <see langword=""default"" /> otherwise</param>
    /// <returns><see langword=""true"" /> if this instance is of type <typeparamref name=""T"" />, <see langword=""false"" /> otherwise</returns>
    public bool Is<T>([NotNullWhen(true)]out T? value)
    {{
        var valueType = GetValue().GetType();
        var isSameType = valueType == typeof(T);

        if (isSameType)
        {{
            value = (T?)GetValue();
        }}
        else
        {{
            value = default;
        }}

        return isSameType;
    }}

    {GenerateSwitches(ordinality)}

    {GenerateMatch(ordinality)}

    /// <inheritdoc />
    public bool Equals({GenericClassName(ordinality)}? other)
    {{
        return other is not null && other._index == _index && other.GetValue().Equals(GetValue());
    }}

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {{
        return obj is {GenericClassName(ordinality)} union && union.Equals(this) || obj is {BaseClassName} u && this.Equals(u);
    }}

    /// <inhericdoc />
    internal bool Equals({BaseClassName} other)
    {{
        return other.GetValue().Equals(GetValue());
    }}

    /// <inheritdoc />
    public override int GetHashCode()
    {{
        #if NETSTANDARD2_0
        unchecked
        {{
            return (GetValue().GetHashCode() * 397) ^ _index;
        }}
#else
        return HashCode.Combine(_index, GetValue().GetHashCode());
#endif
    }}
}}
";
}

static string GenerateGenericArguments(int ordinality, bool stripTypeNames = false)
{
    Func<int, string> mapFunc = stripTypeNames ? x => "" : GenericName;

    return string.Join(",", Enumerable.Range(0, ordinality).Select(mapFunc));
}

static string GenerateFields(int ordinality)
    => string.Join(Environment.NewLine,
        Enumerable.Range(0, ordinality)
        .Select(x => $"private readonly {GenericName(x)} {FieldName(x)} = default!;")
    );

static string GenerateConstructors(int ordinality)
    => string.Join(Environment.NewLine,
        Enumerable.Range(0, ordinality)
            .Select(x => $@"/// <summary>
    /// Create an instance of <see cref=""{GenericClassNameForDocs(ordinality)}"" /> with the type <typeparamref name=""{GenericName(x)}"" />
    /// </summary>
    /// <param name=""value"">The value to initialize this instance</param>""
public {ClassName}({GenericName(x)} value)
{{
    _index = {x};

    {FieldName(x)} = value;
}}
"));

static string GenerateImplicitOperators(int ordinality)
    => string.Join(Environment.NewLine,
        Enumerable.Range(0, ordinality)
            .Select(x => $@"/// <summary>
    /// Implicitly converts <typeparamref name=""{GenericName(x)}"" /> into <see cref=""{GenericClassNameForDocs(ordinality)}"" />
    /// </summary>
    /// <param name=""x"">The value to convert from</param>
public static implicit operator {GenericClassName(ordinality)}({GenericName(x)} x) => new(x);")
    );

static string GenerateSwitches(int ordinality)
{
    var syncSwitch = BaseSwitch(false);
    var asyncSwitch = BaseSwitch(true);

    return $@"{syncSwitch}
{asyncSwitch}";

    static string actionName(int i) => $"action{i}";
    static string caseHandler(int i, bool isAsync) => $@"case {i}:
    {(isAsync ? "await " : "")}{actionName(i)}({FieldName(i)});
    return;";
    string Arguments(bool isAsync) => string.Join(',', Enumerable.Range(0, ordinality).Select(x => isAsync ? $"Func<{GenericName(x)}, Task> {actionName(x)}" : $"Action<{GenericName(x)}> {actionName(x)}"));
    string Handlers(bool isAsync) => string.Join(Environment.NewLine, Enumerable.Range(0, ordinality).Select(x => caseHandler(x, isAsync)));

    string BaseSwitch(bool isAsync)
    {
        return $@"/// <summary>
    /// Executes an action based on the type of this <see cref=""{GenericClassNameForDocs(ordinality)}"" /> instance
    /// </summary>
    {string.Join(Environment.NewLine, Enumerable.Range(0, ordinality).Select(x => $@"/// <param name=""{actionName(x)}"">The action to take if this instance is of type <typeparamref name=""{GenericName(x)}"" /></param>"))}
public {(isAsync ? "async Task" : "void")} Switch{(isAsync ? "Async" : "")}({Arguments(isAsync)})
{{
    switch(_index)
    {{
        {Handlers(isAsync)}
        default:
            throw new IndexOutOfRangeException(""Union went out of range"");
    }}
}}
";
    }
}

static string GenerateMatch(int ordinality)
{
    static string actionName(int i) => $"func{i}";
    var arguments = string.Join(',', Enumerable.Range(0, ordinality).Select(x => $"Func<{GenericName(x)}, TOut> {actionName(x)}"));
    var handlers = string.Join(Environment.NewLine, Enumerable.Range(0, ordinality).Select(x => $"{x} => {actionName(x)}({FieldName(x)}),"));

    return $@"/// <summary>
    /// Matches the value of this instance against a mapping function
    /// </summary>
    /// <typeparam name=""TOut""></typeparam>
    {string.Join(Environment.NewLine, Enumerable.Range(0, ordinality).Select(x => $@"/// <param name=""{actionName(x)}""></param>"))}
    /// <returns></returns>
public TOut Match<TOut>({arguments})
{{
    return _index switch
    {{
        {handlers}
        _ => throw new IndexOutOfRangeException(""Union went out of range"")
    }};
}}
";
}