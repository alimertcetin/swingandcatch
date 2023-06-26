using UnityEngine;

namespace TheGame.SaveSystems
{
    public static class XIVSerializer
    {
        public static string Serialize(object obj)
        {
            return JsonUtility.ToJson(obj, true);
        }

        public static T Deserialize<T>(string data)
        {
            return JsonUtility.FromJson<T>(data);
        }
        
    }
}