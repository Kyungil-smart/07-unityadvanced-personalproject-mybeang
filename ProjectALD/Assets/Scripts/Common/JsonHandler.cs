using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class JsonHandler
{
    public static JObject Load(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);
        if (textAsset == null)
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다: Resources/{fileName}");
            return null;
        }
        return JObject.Parse(textAsset.text);
    }
}
