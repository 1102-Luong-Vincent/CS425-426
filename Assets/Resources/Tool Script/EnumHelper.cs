using UnityEngine;

public class EnumHelper
{
    public static void ChangeEnumValue<T>(ref T current, int delta) where T : System.Enum
    {
        T[] values = (T[])System.Enum.GetValues(typeof(T));
        int index = System.Array.IndexOf(values, current);
        index = (index + delta + values.Length) % values.Length;
        current = values[index];
    }

    public static T GetEnumByOffset<T>(T current, int offset) where T : System.Enum
    {
        T[] values = (T[])System.Enum.GetValues(typeof(T));
        int index = System.Array.IndexOf(values, current);
        index = (index + offset + values.Length) % values.Length;
        return values[index];
    }

}
