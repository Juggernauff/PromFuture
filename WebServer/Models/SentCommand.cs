using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebServer.Models
{
    public class SentCommand
    {
        [JsonPropertyName("command_id")]
        public int CommandId { get; set; }

        [JsonPropertyName("terminal_ids")]
        public List<int> TerminalIds { get; set; }

        [JsonPropertyName("name")]
        public string CommandName { get; set; }

        [JsonPropertyName("parameter1")]
        public int CommandParameterValue1 { get; set; }

        [JsonPropertyName("parameter2")]
        public int CommandParameterValue2 { get; set; }

        [JsonPropertyName("parameter3")]
        public int CommandParameterValue3 { get; set; }

        [JsonPropertyName("state_name")]
        public string StateName { get; set; }

        [JsonPropertyName("time_created")]
        public string TimeCreated { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
