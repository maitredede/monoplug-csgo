using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug.Managed
{
    public sealed class NativeEventJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(NativeEventArgs).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            NativeEventArgs e = new NativeEventArgs();
            JObject jsonObject = JObject.Load(reader);
            e.Name = jsonObject.GetValue("name").Value<string>();
            e.Type = (NativeEventArgType)jsonObject.GetValue("type").Value<int>();
            switch (e.Type)
            {
                case NativeEventArgType.Int:
                    e.IntVal = jsonObject.GetValue("value").Value<short>();
                    break;
                case NativeEventArgType.String:
                    e.StrVal = jsonObject.GetValue("value").Value<string>();
                    break;
                case NativeEventArgType.Bool:
                    e.BoolVal = jsonObject.GetValue("value").Value<bool>();
                    break;
                case NativeEventArgType.Long:
                    e.LongVal = jsonObject.GetValue("value").Value<UInt64>();
                    break;
                case NativeEventArgType.Float:
                    e.FloatVal = jsonObject.GetValue("value").Value<float>();
                    break;
                default:
                    throw new NotSupportedException();
            }
            return e;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            NativeEventArgs e = (NativeEventArgs)value;
            writer.WriteStartObject();
            writer.WritePropertyName("name");
            serializer.Serialize(writer, e.Name);
            writer.WritePropertyName("type");
            serializer.Serialize(writer, (int)e.Type);
            writer.WritePropertyName("value");
            switch (e.Type)
            {
                case NativeEventArgType.Int:
                    serializer.Serialize(writer, e.IntVal);
                    break;
                case NativeEventArgType.String:
                    serializer.Serialize(writer, e.StrVal);
                    break;
                case NativeEventArgType.Bool:
                    serializer.Serialize(writer, e.BoolVal);
                    break;
                case NativeEventArgType.Long:
                    serializer.Serialize(writer, e.LongVal);
                    break;
                case NativeEventArgType.Float:
                    serializer.Serialize(writer, e.FloatVal);
                    break;
                default:
                    throw new NotSupportedException();
            }
            writer.WriteEndObject();
        }
    }
}
