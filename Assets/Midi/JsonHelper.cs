using System;
using UnityEngine;

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] items)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = items;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] items, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = items;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
