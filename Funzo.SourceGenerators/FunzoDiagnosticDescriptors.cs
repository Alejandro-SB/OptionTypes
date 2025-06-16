using Microsoft.CodeAnalysis;

namespace Funzo;
internal static class FunzoDiagnosticDescriptors
{
    public static class Result
    {
        public static DiagnosticDescriptor TopLevelError
            => new("FNZ0001",
                "Class must be top level",
                "Class '{0}' using ResultGenerator must be top level",
                "ResultGenerator",
                DiagnosticSeverity.Error,
                true);

        public static DiagnosticDescriptor WrongBaseType
            => new("FNZ0002", "Result must inherit from Result",
                "Class '{0}' should inherit from Result",
                "ResultGenerator",
                DiagnosticSeverity.Error,
                true);

        public static DiagnosticDescriptor ObjectNotValidType
            => new("FNZ0003", "Object is not a valid type parameter",
                "Defined conversions to or from a base type are not allowed for class '{0}'",
                "ResultGenerator",
                DiagnosticSeverity.Error,
                true);
    }

    public static class Union
    {
        public static DiagnosticDescriptor TopLevelError
            => new("FNZ0004",
                "Class must be top level",
                "Class '{0}' using UnionGenerator must be top level",
                "ResultGenerator",
                DiagnosticSeverity.Error,
                true);

        public static DiagnosticDescriptor WrongBaseType
            => new("FNZ0005", "Unions must inherit from Union",
                "Class '{0}' should inherit from Union",
                "ResultGenerator",
                DiagnosticSeverity.Error,
                true);

        public static DiagnosticDescriptor ObjectNotValidType
            => new("FNZ0006", "Object is not a valid type parameter",
                "Defined conversions to or from a base type are not allowed for class '{0}'",
                "UnionGenerator",
                DiagnosticSeverity.Error,
                true);

        public static DiagnosticDescriptor InterfaceNotValidType
            => new("FNZ0007", "user-defined conversions to or from an interface are not allowed",
                "user-defined conversions to or from an interface are not allowed",
                "UnionGenerator",
                DiagnosticSeverity.Error,
                true);
    }
}
