using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DataApiService.Models
{
    /// <summary>
    /// Аппарат
    /// </summary>
    public class Machine
    {
        [JsonPropertyName("machine_id")]
        public int Machine_id { get; set; }

        [JsonPropertyName("machine_name")]
        public string Machine_name { get; set; }

        [JsonPropertyName("machine_model")]
        public string Machine_model { get; set; }

        [JsonPropertyName("model_id")]
        public int Model_id { get; set; }

        [JsonPropertyName("machine_address")]
        public string Machine_address { get; set; }

        [JsonPropertyName("terminal_id")]
        public int? Terminal_id { get; set; }
        
        [JsonPropertyName("owner_id")]
        public int Owner_id { get; set; }

    }
}
