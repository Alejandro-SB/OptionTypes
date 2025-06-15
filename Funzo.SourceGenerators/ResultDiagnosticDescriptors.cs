using Microsoft.CodeAnalysis;

namespace Funzo;
internal static class ResultDiagnosticDescriptors
{
    public static DiagnosticDescriptor WrongBaseType
        => new("FNZ0001", "Result must inherit from Result",
            "Class '{0}' should inherit from Result",
            "ResultGenerator",
            DiagnosticSeverity.Error,
            true);
}
