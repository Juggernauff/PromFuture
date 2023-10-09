using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataApiService.Models
{
    public class Command
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("parameter_name1")]
        public string ParameterName1 { get; set; }

        [JsonPropertyName("parameter_name2")]
        public string ParameterName2 { get; set; }

        [JsonPropertyName("parameter_name3")]
        public string ParameterName3 { get; set; }

        [JsonPropertyName("parameter_default_value1")]
        public int? ParameterValue1 { get; set; }

        [JsonPropertyName("parameter_default_value2")]
        public int? ParameterValue2 { get; set; }

        [JsonPropertyName("parameter_default_value3")]
        public int? ParameterValue3 { get; set; }

        [JsonPropertyName("visible")]
        public bool Visible { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
