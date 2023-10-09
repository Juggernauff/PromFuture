using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataApiService.Models
{
    public class ResponseObject<T>
    {
        [JsonPropertyName("items")]
        public List<T> Items { get; set; }

        [JsonPropertyName("item")]
        public T Item { get; set; }
    }
}
