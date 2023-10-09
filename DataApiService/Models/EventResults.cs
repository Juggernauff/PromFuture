using System.Text.Json.Serialization;

namespace DataApiService.Models
{
    /// <summary>
    /// Событие (Event)
    /// </summary>
    public class EventResults
    {
        /// <summary>
        /// Идентификатор аппарата
        /// </summary>
        [JsonPropertyName("machine_id")]
        public int Machine_id { get; set; }

        /// <summary>
        /// Название аппарата
        /// </summary>
        [JsonPropertyName("machine_name")]
        public string Machine_name { get; set; }

        /// <summary>
        /// Расположение аппарата
        /// </summary>
        [JsonPropertyName("machine_address")]
        public string Machine_address { get; set; }

        /// <summary>
        /// Модель аппарата
        /// </summary>
        [JsonPropertyName("machine_model")]
        public string Machine_model { get; set; }

        /// <summary>
        /// Идентификатор события
        /// </summary>
        [JsonPropertyName("event_id")]
        public int Event_id { get; set; }

        /// <summary>
        /// Название события
        /// </summary>
        [JsonPropertyName("event_name")]
        public string Event_name { get; set; }

       
        /// <summary>
        /// Название продукта
        /// </summary>
        [JsonPropertyName("product_name")]
        public string Product_name { get; set; }

        /// <summary>
        /// Сумма покупки
        /// </summary>
        [JsonPropertyName("sum")]
        public long Sum { get; set; }

        /// <summary>
        /// Дата события
        /// </summary>
        [JsonPropertyName("date")]
        public string Date { get; set; }

        /// <summary>
        /// Идентификатор даты ??
        /// </summary>
        [JsonPropertyName("dateID")]
        public decimal DateId { get; set; }
    }
}
