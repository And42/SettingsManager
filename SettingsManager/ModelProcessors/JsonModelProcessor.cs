using Newtonsoft.Json;

namespace SettingsManager.ModelProcessors
{
    public class JsonModelProcessor : IModelProcessor
    {
        public T LoadModelFromString<T>(string input) where T : SettingsModel
        {
            return JsonConvert.DeserializeObject<T>(input);
        }

        public string SaveModelToString<T>(T model) where T : SettingsModel
        {
            return JsonConvert.SerializeObject(model, Formatting.Indented);
        }
    }
}
