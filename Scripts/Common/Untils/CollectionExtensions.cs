using System.Collections.Generic;

public static class CollectionExtensions
{
    public static List<TValue> AsList<TKey,TValue>(this Dictionary<TKey,TValue> dict)
    {
        List<TValue> r = new List<TValue>();
        foreach (var item in dict.Values)
        {
            r.Add(item);
        }
        return r;
    }

    public static TValue GetValue<TKey,TValue>(this Dictionary<TKey,TValue> dict, TKey key)
    {
        TValue val;
        dict.TryGetValue(key, out val);

        return val;
    }
}
