using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class FileHandler
{
    public static void SaveToJSON<T>(List<T> toSave, string filename)
    {
        var content = JsonHelper.ToJson(toSave.ToArray());
        WriteFile(GetPath(filename), content);
    }

    public static void SaveToJSON<T>(T toSave, string filename)
    {
        var content = JsonUtility.ToJson(toSave);
        WriteFile(GetPath(filename), content);
    }

    public static List<T> ReadListFromJSON<T>(string filename)
    {
        var content = ReadFile(GetPath(filename));

        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return new List<T>();
        }

        var res = JsonHelper.FromJson<T>(content).ToList();
        return res;
    }

    public static T ReadFromJSON<T>(string filename)
    {
        var content = ReadFile(GetPath(filename));

        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return default;
        }

        var res = JsonUtility.FromJson<T>(content);
        return res;
    }

    private static string GetPath(string filename)
    {
        return Application.dataPath + "/" + filename;
    }

    private static void WriteFile(string path, string content)
    {
        var fileStream = new FileStream(path, FileMode.Create);
        using var writer = new StreamWriter(fileStream);
        writer.Write(content);
    }

    private static string ReadFile(string path)
    {
        if (!File.Exists(path)) return "";

        using var reader = new StreamReader(path);
        var content = reader.ReadToEnd();
        return content;
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        var wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.items;
    }

    public static string ToJson<T>(T[] array, bool prettyPrint = true)
    {
        var wrapper = new Wrapper<T>
        {
            items = array
        };

        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}