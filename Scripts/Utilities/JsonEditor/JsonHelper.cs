using UnityEngine;

namespace Magia.Utilities.Json
{
    public static class JsonHelper<T>
    {
        public static T LoadData(string fileName)
        {
            TextAsset jsonTextFile = Resources.Load<TextAsset>(fileName);
            T data = JsonUtility.FromJson<T>(jsonTextFile.text);

            return data;
        }
    }
}
