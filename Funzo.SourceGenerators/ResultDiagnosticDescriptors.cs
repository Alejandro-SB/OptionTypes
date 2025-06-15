using Microsoft.CodeAnalysis;

namespace Funzo;
internal static class ResultDiagnosticDescriptors
{
    public static DiagnosticDescriptor TopLevelError
        => new("FNZ0001",
            "Class must be top level",
            "Class '{0}' using ResultGenerrator must be top level",
            "ResultGenerator",
            DiagnosticSeverity.Error,
            true);

    public static DiagnosticDescriptor WrongBaseType
        => new("FNZ0002", "Result must inherit from Result",
            "Class '{0}' should inherit from Result",
            "ResultGenerator",
            DiagnosticSeverity.Error,
            true);
}
