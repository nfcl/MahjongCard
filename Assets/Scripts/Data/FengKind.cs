using Data;

namespace Data
{

    public enum FengKind
    {
        Dong = 0,
        Nan = 1,
        Xi = 2,
        Bei = 3,
    }
}

public static class FengKindExtension
{
    public static string[] FengString = { "东", "南", "西", "北" };

    public static string toString(this FengKind kind)
    {
        return FengString[(int)kind];
    }
}