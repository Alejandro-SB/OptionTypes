namespace Funzo.Serialization;

internal class ResultRepresentation<TOk, TErr>
{
    public bool IsOk { get; set; }
    public TOk? Ok { get; set; }
    public TErr? Err { get; set; }
}

internal class ResultOkRepresentation<TOk>
{
    public static bool IsOk => true;
    public TOk Ok { get; set; }

    public ResultOkRepresentation(TOk ok)
    {
        Ok = ok;
    }
}

internal class ResultErrRepresentation<TErr>
{
    public static bool IsOk => false;
    public TErr Err { get; set; }

    public ResultErrRepresentation(TErr err)
    {
        Err = err;
    }
}