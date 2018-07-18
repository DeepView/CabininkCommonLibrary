using System;
using Cabinink.TypeExtend;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using Cabinink.DataTreatment.Database;
using Cabinink.TypeExtend.Collections;
namespace Cabinink.DataTreatment.ORMapping
{
   /// <summary>
   /// SQL语句生成实例，用于生成受CCL支持的数据库系统的SQL语句。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class SQLGenerator
   {
      private object _operatedObject;//需要生成对应成员SQL语句的对象。
      private string _sqlSentence;//存储生成的SQL语句。
      private ESupportedDbSystem _supportedDbSystem;//CCL支持的DBS的名称枚举。
      private EDateTimeConvertMode _dateTimeConvertMode;//日期类型转换存储的模式。
      /// <summary>
      /// 构造函数，创建一个适用于SQLite DBS的空System.Object实例的SQL语句生成实例。
      /// </summary>
      public SQLGenerator()
      {
         _operatedObject = new object();
         _sqlSentence = string.Empty;
         _supportedDbSystem = ESupportedDbSystem.SQLite;
         _dateTimeConvertMode = EDateTimeConvertMode.ToString;
      }
      /// <summary>
      /// 构造函数，创建一个适用于SQLite DBS的SQL语句生成实例。
      /// </summary>
      /// <param name="operatedObject">需要生成对应成员SQL语句的对象。</param>
      /// <exception cref="NotSupportedTypeException">当参数operatedObject指定的对象不受支持时，则会抛出这个异常。</exception>
      public SQLGenerator(object operatedObject)
      {
         if (operatedObject.GetType().FullName == @"Cabinink.DataTreatment.ORMapping.SQLGenerator")
         {
            throw new NotSupportedTypeException();
         }
         _operatedObject = operatedObject;
         _sqlSentence = string.Empty;
         _supportedDbSystem = ESupportedDbSystem.SQLite;
         _dateTimeConvertMode = EDateTimeConvertMode.ToString;
      }
      /// <summary>
      /// 构造函数，创建一个指定DBS的空System.Object实例的SQL语句生成实例。
      /// </summary>
      /// <param name="supportedDbSystem">受CCL支持的DBS。</param>
      public SQLGenerator(ESupportedDbSystem supportedDbSystem)
      {
         _operatedObject = new object();
         _sqlSentence = string.Empty;
         _supportedDbSystem = supportedDbSystem;
         _dateTimeConvertMode = EDateTimeConvertMode.ToString;
      }
      /// <summary>
      /// 构造函数，创建一个指定操作对象SQL语句生成实例。
      /// </summary>
      /// <param name="operatedObject">需要生成对应成员SQL语句的对象。</param>
      /// <param name="supportedDbSystem">受CCL支持的DBS。</param>
      /// <exception cref="NotSupportedTypeException">当参数operatedObject指定的对象不受支持时，则会抛出这个异常。</exception>
      public SQLGenerator(object operatedObject, ESupportedDbSystem supportedDbSystem)
      {
         if (operatedObject.GetType().FullName == @"Cabinink.DataTreatment.ORMapping.SQLGenerator")
         {
            throw new NotSupportedTypeException();
         }
         _operatedObject = operatedObject;
         _sqlSentence = string.Empty;
         _supportedDbSystem = supportedDbSystem;
         _dateTimeConvertMode = EDateTimeConvertMode.ToString;
      }
      /// <summary>
      /// 构造函数，创建一个指定操作对象SQL语句生成实例。
      /// </summary>
      /// <param name="operatedObject">需要生成对应成员SQL语句的对象。</param>
      /// <param name="supportedDbSystem">受CCL支持的DBS。</param>
      /// <param name="dateTimeConvertMode">指定日期类型转换存储的模式。</param>
      /// <exception cref="NotSupportedTypeException">当参数operatedObject指定的对象不受支持时，则会抛出这个异常。</exception>
      public SQLGenerator(object operatedObject, ESupportedDbSystem supportedDbSystem, EDateTimeConvertMode dateTimeConvertMode)
      {
         if (operatedObject.GetType().FullName == @"Cabinink.DataTreatment.ORMapping.SQLGenerator")
         {
            throw new NotSupportedTypeException();
         }
         _operatedObject = operatedObject;
         _sqlSentence = string.Empty;
         _supportedDbSystem = supportedDbSystem;
         _dateTimeConvertMode = dateTimeConvertMode;
      }
      /// <summary>
      /// 获取或设置当前实例需要生成对应成员SQL语句的对象。
      /// </summary>
      /// <exception cref="NotSupportedTypeException">当参数operatedObject指定的对象不受支持时，则会抛出这个异常。</exception>
      public object OperatedObject
      {
         get => _operatedObject;
         set
         {
            if (value.GetType().FullName == @"Cabinink.DataTreatment.ORMapping.SQLGenerator")
            {
               throw new NotSupportedTypeException();
            }
            _operatedObject = value;
         }
      }
      /// <summary>
      /// 获取或设置当前实例生成的SQL语句。
      /// </summary>
      public string SqlSentence { get => _sqlSentence; set => _sqlSentence = value; }
      /// <summary>
      /// 获取或设置当前实例受支持的DBS。
      /// </summary>
      public ESupportedDbSystem SupportedDbSystem { get => _supportedDbSystem; set => _supportedDbSystem = value; }
      /// <summary>
      /// 获取或设置当前实例的日期类型转换存储的模式。
      /// </summary>
      public EDateTimeConvertMode DateTimeConvertMode { get => _dateTimeConvertMode; set => _dateTimeConvertMode = value; }
      /// <summary>
      /// 获取当前被操作实例的属性信息集合，这个集合的每一个元素都以(propertyName, typeName)形式的元组呈现。
      /// </summary>
      public List<(string, string)> PropertiesInfo
      {
         get
         {
            ObjectMemberGetter getter = new ObjectMemberGetter(OperatedObject);
            List<(string, string)> result = new List<(string, string)>();
            List<string> memberTypes = getter.GetPropertyTypes();
            List<string> memberNames = getter.GetPropertyNames();
            for (int i = 0; i < memberNames.Count; i++)
            {
               if (memberTypes[i] == @"System.DateTime") result.Add((memberNames[i], new TypeMapping(0).DateTimeTypeMapping(true)));
               else result.Add((memberNames[i], new TypeMapping(0)
               {
                  SupportedDbSystem = SupportedDbSystem
               }.CTSMapping(memberTypes[i])));
            }
            return result;
         }
      }
      /// <summary>
      /// 生成创建一个指定名称，路径和日志记录文件路径的数据库的SQL语句，目前这个方法暂时支持SQLServer数据库的创建。
      /// </summary>
      /// <param name="databaseName">数据库的名称。</param>
      /// <param name="databaseFileUrl">数据库文件的物理地址。</param>
      /// <param name="logFileUrl">数据库日志文件的物理地址。</param>
      /// <exception cref="EmptySqlCommandLineException">如果用户选择SQLite和MSAccess数据库的SQL创建方式时，则需要抛出这个异常，原因是目前暂时不支持这两种数据库文件的SQL语句方式创建。</exception>
      public void GenerateSqlForCreateDatabase(string databaseName, string databaseFileUrl, string logFileUrl)
      {
         switch (SupportedDbSystem)
         {
            case ESupportedDbSystem.SQLServer:
               SqlSentence = @"create databse " + databaseName + " on(name=" + databaseName +
                  ",filename=" + databaseFileUrl + ",size=5MB,maxsize=20MB,filegrowth=20MB)log on(name=" + databaseName +
                  ",filename=" + logFileUrl + ",size=2MB,maxsize=10MB,filegrowth=1MB)";
               break;
            case ESupportedDbSystem.SQLite:
            case ESupportedDbSystem.MSAccess2003:
            default:
               throw new EmptySqlCommandLineException("目前暂时不支持SQLite和MSAccess数据库的SQL方式创建！");
         }
      }
      /// <summary>
      /// 生成创建数据表的SQL语句，数据库系统类型由SupportedDbSystem属性指定。
      /// </summary>
      /// <param name="tableName">数据表的名称。</param>
      /// <param name="primaryKey">指定的主键字段，如果指定的字段在PropertiesInfo属性中无法被检索到，则会默认指定第一个字段为主键。</param>
      /// <param name="isNullField">一个列表集合，用于存储所对应字段是否允许为空字段，如果某个索引上的值为true，则表示这个这个索引所对应的字段允许为空，否则不允许为空。</param>
      /// <exception cref="OverflowException">当isNullField参数的Coun属性小于当前实例的PropertiesInfo.Count属性时，则将会抛出这个异常。</exception>
      public void GenerateSqlForCreateTable(string tableName, string primaryKey, List<bool> isNullField)
      {
         ExString baseSql = @"create table " + tableName + "( ";
         List<(string, string)> propInfos = PropertiesInfo;
         List<bool> isnulfd = isNullField;
         if (isnulfd == null)
         {
            isnulfd = new List<bool>(propInfos.Count);
            for (int i = 0; i < isnulfd.Count; i++) isnulfd.Add(true);
         }
         if (isNullField.Count < propInfos.Count)
         {
            throw new OverflowException("isNullField参数的Coun属性不能小于当前实例的PropertiesInfo.Count属性！");
         }
         for (int i = 0; i < propInfos.Count; i++)
         {
            if (propInfos[i].Item1 == primaryKey)
            {
               baseSql = baseSql + propInfos[i].Item1 + " " + propInfos[i].Item2 + " primary key not null, ";
            }
            else
            {
               string notNullString = isNullField[i] ? string.Empty : " not null, ";
               baseSql = baseSql + propInfos[i].Item1 + " " + propInfos[i].Item2 + notNullString;
            }
         }
         SqlSentence = baseSql.SubString(0, baseSql.Length - 2) + " );";
      }
      /// <summary>
      /// 生成删除数据表的SQL语句。
      /// </summary>
      /// <param name="tableName">指定需要删除的数据表。</param>
      public void GenerateSqlForDeleteTable(string tableName) => SqlSentence = @"drop table " + tableName + ";";
      /// <summary>
      /// 生成用于列举不同值的SQL语句。
      /// </summary>
      /// <param name="tableName">指定需要操作的数据表。</param>
      /// <param name="fieldName">指定需要被列举的字段。</param>
      public void GenerateSqlForDistinct(string tableName, string fieldName) => SqlSentence = @"select distinct " + fieldName + " from " + tableName + ";";
      /// <summary>
      /// 通过当前实例包含的对象来生成插入新值的SQL语句。
      /// </summary>
      /// <param name="tableName">指定需要操作的数据表。</param>
      /// <remarks>这个操作生成的Insert SQL语句，其中需要插入的值由当前实例的OperatedObject属性中的公共属性的值来决定。</remarks>
      public void GenerateSqlForInsert(string tableName) => GenerateSqlForInsert(tableName, OperatedObject);
      /// <summary>
      /// 通过指定的对象来生成插入新值的SQL语句。
      /// </summary>
      /// <param name="tableName">指定需要操作的数据表。</param>
      /// <param name="instanceWithIncludedValues">需要插入的值所在的对象或者实例。</param>
      /// <remarks>这个操作生成的Insert SQL语句，其中需要插入的值由instanceWithIncludedValues参数中的公共属性的值来决定。</remarks>
      /// <exception cref="TypeIsNotMatchException">当源数据类型与指定数据类型不匹配时，则将会抛出这个异常。</exception>
      public void GenerateSqlForInsert(string tableName, object instanceWithIncludedValues)
      {
         if (instanceWithIncludedValues.GetType().FullName != OperatedObject.GetType().FullName)
         {
            throw new TypeIsNotMatchException();
         }
         string baseSql = @"insert into " + tableName + " values(";
         string valuesCsvStr = string.Empty;
         string sqlInsertedValue = string.Empty;
         BiDirectionalLinkedList<object> values = new ObjectMemberGetter(OperatedObject).GetProperityValues();
         for (int i = 0; i < values.Count; i++)
         {
            if (values[i].Element.GetType().FullName == @"System.DateTime")
            {
               switch (DateTimeConvertMode)
               {
                  case EDateTimeConvertMode.ToTicks:
                     sqlInsertedValue = ((DateTime)(values[i].Element)).Ticks.ToString() + ",";
                     break;
                  case EDateTimeConvertMode.ToDbsDateTime:
                     string shortDateStr = ((DateTime)(values[i].Element)).ToShortDateString();
                     string longTimeStr = ((DateTime)(values[i].Element)).ToLongTimeString();
                     sqlInsertedValue = "'" + shortDateStr + " " + longTimeStr + "',";
                     break;
                  case EDateTimeConvertMode.ToString:
                  default:
                     sqlInsertedValue = "'" + values[i].Element.ToString() + "',";
                     break;
               }
            }
            else sqlInsertedValue = "'" + values[i].Element.ToString() + "',";
            valuesCsvStr += sqlInsertedValue;
         }
         valuesCsvStr = valuesCsvStr.Substring(0, valuesCsvStr.Length - 1) + ");";
         SqlSentence = baseSql + valuesCsvStr;
      }
      /// <summary>
      /// 生成显示指定数据表所有记录的SQL语句。
      /// </summary>
      /// <param name="tableName">指定需要操作的数据表。</param>
      /// <remarks>该方法所生成的SQL语句为select * from table.</remarks>
      public void GenerateSqlForShowTable(string tableName) => SqlSentence = @"select * from " + tableName + ";";
   }
   /// <summary>
   /// 日期时间类型的转换模式的枚举。
   /// </summary>
   public enum EDateTimeConvertMode : int
   {
      /// <summary>
      /// 通过DateTime实例的ToString方法直接转换为字符串。
      /// </summary>
      [EnumerationDescription("以ToString方法转换")]
      ToString = 0x0000,
      /// <summary>
      /// 转换为Ticks，即转换为long数据类型。
      /// </summary>
      [EnumerationDescription("转换为Ticks")]
      ToTicks = 0x0001,
      /// <summary>
      /// 转换为DBS兼容的日期时间类型。
      /// </summary>
      [EnumerationDescription("转换为DBS兼容的日期时间类型")]
      ToDbsDateTime = 0x0002
   }
   /// <summary>
   /// 数据类型不匹配时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class TypeIsNotMatchException : Exception
   {
      public TypeIsNotMatchException() : base("源数据类型与指定数据类型不匹配！") { }
      public TypeIsNotMatchException(string message) : base(message) { }
      public TypeIsNotMatchException(string message, Exception inner) : base(message, inner) { }
      protected TypeIsNotMatchException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
