using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Util
{
    class JsonUtil
    {
        public static string SerialObject<T>(T obj)
        {
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            ser.WriteObject(stream1, obj);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            return sr.ReadToEnd();
        }

        public static T DeserialObject<T>(byte[] data)
        {
            MemoryStream stream1 = new MemoryStream();
            stream1.Write(data, 0, data.Length);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            stream1.Position = 0;
            return (T)ser.ReadObject(stream1);
        }

        public static T DeserialObject<T>(string data)
        {
            return DeserialObject<T>(System.Text.Encoding.Default.GetBytes(data));
        }
    }
}
