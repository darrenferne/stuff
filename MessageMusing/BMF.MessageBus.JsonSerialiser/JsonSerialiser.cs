using BMF.MessageBus.Core.Interfaces;
using Newtonsoft.Json;
using System;
using System.Text;
using NewtonsoftSerialiser = Newtonsoft.Json.JsonSerializer;
using NewtonsoftSerialiserSettings = Newtonsoft.Json.JsonSerializerSettings;

namespace BMF.MessageBus.Serialisers
{
    public class JsonSerialiser : IMessageBusSerialiser
    {
        NewtonsoftSerialiserSettings _settings;

        public JsonSerialiser()
        {
            _settings = new JsonSerializerSettings();
        }

        public JsonSerialiser(NewtonsoftSerialiserSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            _settings = settings;
        }

        public object Deserialise(byte[] messageData)
        {
            var messageAsString = Encoding.UTF8.GetString(messageData);
            var result = JsonConvert.DeserializeObject(messageAsString, _settings);
            return result;
        }

        public T_message Deserialise<T_message>(byte[] messageData)
        {
            var messageAsString = Encoding.UTF8.GetString(messageData);
            var result = JsonConvert.DeserializeObject<T_message>(messageAsString, _settings);
            return result;
        }

        public byte[] Serialise(object message)
        {
            var messageAsString = JsonConvert.SerializeObject(message, _settings);
            return Encoding.UTF8.GetBytes(messageAsString);
        }

        public byte[] Serialise<T_message>(T_message message)
        {
            var messageAsString = JsonConvert.SerializeObject(message, _settings);
            return Encoding.UTF8.GetBytes(messageAsString);
        }
    }
}
