using System;
using Newtonsoft.Json;

namespace Logicing.Knowing.StratisClient
{
    public class Uint256Converter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var uint256 = (Uint256)value;
            writer.WriteValue(uint256.Value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new Uint256{Value = (string)reader.Value};
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}