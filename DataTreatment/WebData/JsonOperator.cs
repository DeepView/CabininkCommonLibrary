using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
namespace Cabinink.DataTreatment.WebData
{
   /// <summary>
   /// Json字符串解析类，用于解析某些Json字符串。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class JsonOperator
   {
      private string _jsonString;//Json字符串。
      /// <summary>
      /// 构造函数，创建一个指定Json字符串的JSON操作实例。
      /// </summary>
      /// <param name="jsonString"></param>
      public JsonOperator(string jsonString) => _jsonString = jsonString;
      /// <summary>
      /// 获取或设置当前实例的Json字符串。
      /// </summary>
      public string JsonString { get => _jsonString; set => _jsonString = value; }
      /// <summary>
      /// 从当前实例包含的Json字符串解析数据并返回T所指定的对象
      /// </summary>
      /// <typeparam name="T">解析数据之后返回的实例。</typeparam>
      /// <returns>如果操作无异常则会返回一个解析数据之后所对应的解析实例。</returns>
      public T Parse<T>()
      {
         using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(JsonString)))
         {
            return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
         }
      }
      /// <summary>
      /// 将指定的对象解析为所对应的Json字符串，并存储到JsonString属性中。
      /// </summary>
      /// <param name="jsonObject">需要被解析的对象。</param>
      public void Stringify(object jsonObject)
      {
         using (var ms = new MemoryStream())
         {
            new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
            JsonString = Encoding.UTF8.GetString(ms.ToArray());
         }
      }
      /// <summary>
      /// 从Json字符串中读取相关信息。
      /// </summary>
      /// <returns>该操作将会返回一个包含JsonToken，ValueType和Value的元组列表集合。</returns>
      public List<(JsonToken, Type, object)> Read()
      {
         List<(JsonToken, Type, object)> result = new List<(JsonToken, Type, object)>();
         JsonReader reader = new JsonTextReader(new StringReader(JsonString));
         while (reader.Read()) result.Add((reader.TokenType, reader.ValueType, reader.Value));
         return result;
      }
      /// <summary>
      /// 将指定的属性和值写入Json字符串中。
      /// </summary>
      /// <param name="writedPropertyString">需要被写入的属性。</param>
      /// <param name="writedValueString">需要被写入的属性值。</param>
      public void Write(string writedPropertyString, string writedValueString)
      {
         StringBuilder sBuilder = new StringBuilder(JsonString);
         StringWriter sWriter = new StringWriter(sBuilder);
         JsonWriter jsonWriter = new JsonTextWriter(sWriter);
         jsonWriter.WriteStartObject();
         jsonWriter.WritePropertyName(writedPropertyString);
         jsonWriter.WriteValue(writedValueString);
         jsonWriter.WriteEndObject();
         jsonWriter.Flush();
      }
   }
}
