using System;
using Newtonsoft.Json;

namespace Logicing.Knowing.StratisClient
{
    public class MoneyConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var money = (Money)value;
            writer.WriteValue(money.Satoshi);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new Money {Satoshi = (long?) reader.Value};
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Money);
        }
    }
}