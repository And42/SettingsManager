using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SettingsManager.ModelProcessors
{
    public class JsonModelProcessor : IModelProcessor
    {
        private static readonly ContractResolver PropertiesResolver = new ContractResolver();

        private class ContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);

                property.PropertyName = FormatPropertyName(property.PropertyName);

                return property;
            }
        }

        public T LoadModelFromString<T>(string input) where T : SettingsModel
        {
            return JsonConvert.DeserializeObject<T>(input, new JsonSerializerSettings{ContractResolver = PropertiesResolver});
        }

        public string SaveModelToString<T>(T model) where T : SettingsModel
        {
            return JsonConvert.SerializeObject(
                model, 
                new JsonSerializerSettings
                {
                    ContractResolver = PropertiesResolver,
                    Formatting = Formatting.Indented
                }
            );
        }

        private static string FormatPropertyName(string propertyName)
        {
            var builder = new StringBuilder(propertyName.Length);

            for (int i = 0; i < propertyName.Length; i++)
            {
                char current = propertyName[i];

                if (char.IsLower(current))
                {
                    builder.Append(current);
                    continue;
                }

                if (i > 0 && propertyName[i - 1] != '_')
                    builder.Append('_');

                builder.Append(char.ToLower(current));
            }

            return builder.ToString();
        }
    }
}
