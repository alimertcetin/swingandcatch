using Newtonsoft.Json;

namespace TheGame.SaveSystems
{
    public static class XIVSerializer
    {
        public static string Serialize(object deserializedData)
        {
            return JsonConvert.SerializeObject(deserializedData, Formatting.Indented);
        }

        public static object Deserialize(string serializedData)
        {
            return JsonConvert.DeserializeObject(serializedData);
        }

        public static T Deserialize<T>(string serializedData)
        {
            return JsonConvert.DeserializeObject<T>(serializedData);
        }
        
    }
}