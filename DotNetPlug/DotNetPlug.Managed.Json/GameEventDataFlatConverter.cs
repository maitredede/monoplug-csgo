using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public sealed class GameEventDataFlatConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(GameEventData).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            GameEventData e = new GameEventData();
            JObject jsonObject = JObject.Load(reader);
            e.Event = (GameEvent)Enum.Parse(typeof(GameEvent), jsonObject.GetValue("name").Value<string>());
            //e.Type = (NativeEventArgType)jsonObject.GetValue("type").Value<int>();
            //switch (e.Type)
            //{
            //    case NativeEventArgType.Int:
            //        e.IntVal = jsonObject.GetValue("value").Value<short>();
            //        break;
            //    case NativeEventArgType.String:
            //        e.StrVal = jsonObject.GetValue("value").Value<string>();
            //        break;
            //    case NativeEventArgType.Bool:
            //        e.BoolVal = jsonObject.GetValue("value").Value<bool>();
            //        break;
            //    case NativeEventArgType.Long:
            //        e.LongVal = jsonObject.GetValue("value").Value<UInt64>();
            //        break;
            //    case NativeEventArgType.Float:
            //        e.FloatVal = jsonObject.GetValue("value").Value<float>();
            //        break;
            //    default:
            //        throw new NotSupportedException();
            //}
            return e;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            GameEventData e = (GameEventData)value;

            writer.WriteStartObject();
            writer.WritePropertyName("name");
            writer.WriteValue(e.Event.ToString());

            foreach (GameEventArg arg in e.Args)
            {
                writer.WritePropertyName(arg.Name);
                switch (arg.Type)
                {
                    case ArgumentValueType.Int:
                        writer.WriteValue(arg.ValueInt);
                        break;
                    case ArgumentValueType.String:
                        writer.WriteValue(arg.ValueString);
                        break;
                    case ArgumentValueType.Bool:
                        writer.WriteValue(arg.ValueBool);
                        break;
                    case ArgumentValueType.Long:
                        writer.WriteValue(arg.ValueLong);
                        break;
                    case ArgumentValueType.Float:
                        writer.WriteValue(arg.ValueFloat);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            writer.WriteEndObject();
        }
    }
}
