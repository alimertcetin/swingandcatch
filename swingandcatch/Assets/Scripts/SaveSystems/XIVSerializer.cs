using Newtonsoft.Json;

namespace TheGame.SaveSystems
{
    public static class XIVSerializer
    {
        static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
        };
        
        public static string Serialize(object deserializedData)
        {
            return JsonConvert.SerializeObject(deserializedData, Formatting.Indented, jsonSerializerSettings);
        }

        public static object Deserialize(string serializedData)
        {
            return JsonConvert.DeserializeObject(serializedData, jsonSerializerSettings);
        }

        public static T Deserialize<T>(string serializedData)
        {
            return JsonConvert.DeserializeObject<T>(serializedData, jsonSerializerSettings);
        }
        
    }
}