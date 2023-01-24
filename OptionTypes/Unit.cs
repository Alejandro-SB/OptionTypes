namespace OptionTypes;

public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
{
    public static readonly Unit Default = new();

    public override int GetHashCode() => 0;

    public override bool Equals(object? obj) => obj is Unit;

    public override string ToString() => "()";

    public bool Equals(Unit other) => true;

    public int CompareTo(Unit other) => 0;

    public static bool operator ==(Unit lhs, Unit rhs) => true;

    public static bool operator !=(Unit lhs, Unit rhs) => false;

    public static bool operator >(Unit lhs, Unit rhs) => false;

    public static bool operator >=(Unit lhs, Unit rhs) => true;

    public static bool operator <(Unit lhs, Unit rhs) => false;

    public static bool operator <=(Unit lhs, Unit rhs) => true;

    public static Unit operator +(Unit a, Unit b) => Default;
}
