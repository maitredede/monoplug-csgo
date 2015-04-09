using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public sealed class NativeEventDataFlatConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(GameEventEventArgs).IsAssignableFrom(objectType);
        }

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //NativeEventArgs e = new NativeEventArgs();
            //JObject jsonObject = JObject.Load(reader);
            //e.Name = jsonObject.GetValue("name").Value<string>();
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
            //return e;
            throw new NotSupportedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            GameEventEventArgs e = (GameEventEventArgs)value;
            writer.WriteStartObject();
            writer.WritePropertyName("name");
            serializer.Serialize(writer, e.Event.ToString());
            foreach (GameEventArgument arg in e.Args)
            {
                writer.WritePropertyName(arg.Name);

                //writer.WritePropertyName("type");
                //serializer.Serialize(writer, (int)e.Type);
                //writer.WritePropertyName("value");
                switch (arg.Type)
                {
                    case ArgumentValueType.Int:
                        serializer.Serialize(writer, arg.ValueInt);
                        break;
                    case ArgumentValueType.String:
                        serializer.Serialize(writer, arg.ValueString);
                        break;
                    case ArgumentValueType.Bool:
                        serializer.Serialize(writer, arg.ValueBool);
                        break;
                    case ArgumentValueType.Long:
                        serializer.Serialize(writer, arg.ValueLong);
                        break;
                    case ArgumentValueType.Float:
                        serializer.Serialize(writer, arg.ValueFloat);
                        break;
                    default:
                        throw new NotSupportedException();
                }
                writer.WriteEndObject();
            }
        }
    }
}
