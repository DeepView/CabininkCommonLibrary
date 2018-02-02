using System;
using System.Runtime.InteropServices;
namespace Cabinink.DataTreatment.ORMapping
{
   /// <summary>
   /// 数据类型映射类，可以将受CTS和非托管代码系统支持的类型映射为指定数据库系统可支持的数据类型。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class TypeMapping
   {
      private object _beforeObject;//进行类型映射之前的实例。
      private ESupportedDbSystem _supportedDbSystem;//CCL支持的数据库系统。
      /// <summary>
      /// 构造函数，创建一个SQLite数据库系统类型映射的空数据类型映射实例。
      /// </summary>
      public TypeMapping()
      {
         _beforeObject = null;
         _supportedDbSystem = ESupportedDbSystem.SQLite;
      }
      /// <summary>
      /// 构造函数，创建一个指定受CCL（Cabinink Common Library）支持的数据库系统类型映射的空数据类型映射实例。
      /// </summary>
      /// <param name="supportedDbSystem">CCL支持的数据库系统类型映射的数据库系统。</param>
      public TypeMapping(ESupportedDbSystem supportedDbSystem)
      {
         _beforeObject = null;
         _supportedDbSystem = supportedDbSystem;
      }
      /// <summary>
      /// 构造函数，创建一个默认为SQLite数据库系统类型映射的数据类型映射实例。
      /// </summary>
      /// <param name="mappingObject">需要被映射的数据类型或者实例。</param>
      /// <exception cref="NotSupportedTypeException">当参数mappingObject的子类实例不支持时，则会抛出这个异常。</exception>
      public TypeMapping(object mappingObject)
      {
         if (mappingObject.GetType().ToString() == @"TypeMapping" || mappingObject.GetType().ToString() == @"ObjectMemberGetter")
         {
            throw new NotSupportedTypeException();
         }
         else _beforeObject = mappingObject;
         _supportedDbSystem = ESupportedDbSystem.SQLite;
      }
      /// <summary>
      /// 构造函数，创建一个指定受CCL（Cabinink Common Library）支持的数据库系统类型映射的数据类型映射实例。
      /// </summary>
      /// <param name="mappingObject">需要被映射的数据类型或者实例。</param>
      /// <param name="supportedDbSystem">CCL支持的数据库系统类型映射的数据库系统。</param>
      /// <exception cref="NotSupportedTypeException">当参数mappingObject的子类实例不支持时，则会抛出这个异常。</exception>
      public TypeMapping(object mappingObject, ESupportedDbSystem supportedDbSystem)
      {
         if (mappingObject.GetType().ToString() == @"TypeMapping" || mappingObject.GetType().ToString() == @"ObjectMemberGetter")
         {
            throw new NotSupportedTypeException();
         }
         else
         {
            _beforeObject = mappingObject;
            _supportedDbSystem = supportedDbSystem;
         }
      }
      /// <summary>
      /// 获取或设置当前实例在执行映射操作之前的实例。
      /// </summary>
      public object BeforeManagedObject { get => _beforeObject; set => _beforeObject = value; }
      /// <summary>
      /// 获取或设置当前实例受支持的数据库系统。
      /// </summary>
      public ESupportedDbSystem SupportedDbSystem { get => _supportedDbSystem; set => _supportedDbSystem = value; }
      //public List<(string,string)> ReuniteMapping()
      //{
      //}
      /// <summary>
      /// 获取通用类型系统的数据类型实例映射到当前实例指定数据库系统所对应的数据类型名称字符串。
      /// </summary>
      /// <param name="ctsObject">需要被映射到当前实例指定数据库系统的CTS数据类型实例。</param>
      /// <param name="isThrowedException">决定该方法是否需要抛出异常。</param>
      /// <returns>该操作会返回当前实例所指定的DBS所对应的数据类型的名称字符串。</returns>
      /// <exception cref="NotSupportedTypeException">当参数ctsType指定的数据类型无法被映射到目标数据库系统的数据类型时，则会抛出这个异常。</exception>
      public string CTSMapping(object ctsObject, bool isThrowedException)
      {
         string result = string.Empty;
         if (isThrowedException)
         {
            if (!IsCtsDefinedType(ctsObject)) throw new NotSupportedTypeException();
            else result = CTSMapping(ctsObject.GetType().FullName);
         }
         else
         {
            try
            {
               result = CTSMapping(ctsObject.GetType().FullName);
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
            }
         }
         return result;
      }
      /// <summary>
      /// 获取通用类型系统的数据类型字符串映射到当前实例指定数据库系统所对应的数据类型名称字符串。
      /// </summary>
      /// <param name="ctsTypeString">需要被映射到当前实例指定数据库系统的CTS数据类型字符串。</param>
      /// <returns>该操作会返回当前实例所指定的DBS所对应的数据类型的名称字符串。</returns>
      /// <exception cref="NotSupportedTypeException">当参数ctsType指定的数据类型无法被映射到目标数据库系统的数据类型时，则会抛出这个异常。</exception>
      public string CTSMapping(string ctsTypeString)
      {
         string result = string.Empty;
         switch (ctsTypeString)
         {
            case @"System.Byte":
               switch (SupportedDbSystem)
               {
                  case ESupportedDbSystem.SQLServer:
                     result = "tinyint";
                     break;
                  case ESupportedDbSystem.SQLite:
                     result = "TINYINT";
                     break;
                  case ESupportedDbSystem.MSAccess2003:
                     result = "UNSIGNEDTINTINT";
                     break;
                  default:
                     throw new NotSupportedTypeException();
               }
               break;
            case @"System.SByte":
               switch (SupportedDbSystem)
               {
                  case ESupportedDbSystem.SQLServer:
                     throw new NotSupportedTypeException();
                  case ESupportedDbSystem.SQLite:
                     result = "TINYSINT";
                     break;
                  case ESupportedDbSystem.MSAccess2003:
                  default:
                     throw new NotSupportedTypeException();
               }
               break;
            case @"System.Int16":
               switch (SupportedDbSystem)
               {
                  case ESupportedDbSystem.SQLServer:
                     result = "smallint";
                     break;
                  case ESupportedDbSystem.SQLite:
                  case ESupportedDbSystem.MSAccess2003:
                     result = "SMALLINT";
                     break;
                  default:
                     throw new NotSupportedTypeException();
               }
               break;
            case @"System.Int32":
               switch (SupportedDbSystem)
               {
                  case ESupportedDbSystem.SQLServer:
                     result = "int";
                     break;
                  case ESupportedDbSystem.SQLite:
                     result = "INT";
                     break;
                  case ESupportedDbSystem.MSAccess2003:
                     result = "INTEGER";
                     break;
                  default:
                     throw new NotSupportedTypeException();
               }
               break;
            case @"System.Int64":
               switch (SupportedDbSystem)
               {
                  case ESupportedDbSystem.SQLServer:
                     result = "bigint";
                     break;
                  case ESupportedDbSystem.SQLite:
                     result = "INTEGER";
                     break;
                  case ESupportedDbSystem.MSAccess2003:
                     result = "LONG";
                     break;
                  default:
                     throw new NotSupportedTypeException();
               }
               break;
            case @"System.UInt16":
               switch (SupportedDbSystem)
               {
                  case ESupportedDbSystem.SQLServer:
                     throw new NotSupportedTypeException();
                  case ESupportedDbSystem.SQLite:
                     result = "SMALLUINT";
                     break;
                  case ESupportedDbSystem.MSAccess2003:
                  default:
                     throw new NotSupportedTypeException();
               }
               break;
            case @"System.UInt32":
               switch (SupportedDbSystem)
               {
                  case ESupportedDbSystem.SQLServer:
                     throw new NotSupportedTypeException();
                  case ESupportedDbSystem.SQLite:
                     result = "UINT";
                     break;
                  case ESupportedDbSystem.MSAccess2003:
                  default:
                     throw new NotSupportedTypeException();
               }
               break;
            case @"System.UInt64":
               switch (SupportedDbSystem)
               {
                  case ESupportedDbSystem.SQLServer:
                     throw new NotSupportedTypeException();
                  case ESupportedDbSystem.SQLite:
                     result = "UNSIGNEDINTEGER";
                     break;
                  case ESupportedDbSystem.MSAccess2003:
                  default:
                     throw new NotSupportedTypeException();
               }
               break;
            case @"System.Single":
               switch (SupportedDbSystem)
               {
                  case ESupportedDbSystem.SQLServer:
                     result = "real";
                     break;
                  case ESupportedDbSystem.SQLite:
                  case ESupportedDbSystem.MSAccess2003:
                     result = "SINGLE";
                     break;
                  default:
                     throw new NotSupportedTypeException();
               }
               break;
            case @"System.Double":
               switch (SupportedDbSystem)
               {
                  case ESupportedDbSystem.SQLServer:
                     result = "float";
                     break;
                  case ESupportedDbSystem.SQLite:
                     result = "REAL";
                     break;
                  case ESupportedDbSystem.MSAccess2003:
                     result = "DOUBLE";
                     break;
                  default:
                     throw new NotSupportedTypeException();
               }
               break;
            case @"System.Boolean":
               switch (SupportedDbSystem)
               {
                  case ESupportedDbSystem.SQLServer:
                     result = "bit";
                     break;
                  case ESupportedDbSystem.SQLite:
                  case ESupportedDbSystem.MSAccess2003:
                     result = "BIT";
                     break;
                  default:
                     throw new NotSupportedTypeException();
               }
               break;
            case @"System.Char":
               switch (SupportedDbSystem)
               {
                  case ESupportedDbSystem.SQLServer:
                     result = "char(1)";
                     break;
                  case ESupportedDbSystem.SQLite:
                     result = "TEXT";
                     break;
                  case ESupportedDbSystem.MSAccess2003:
                  default:
                     throw new NotSupportedTypeException();
               }
               break;
            case @"System.Decimal":
               switch (SupportedDbSystem)
               {
                  case ESupportedDbSystem.SQLServer:
                     result = "decimal";
                     break;
                  case ESupportedDbSystem.SQLite:
                     result = "DECIMAL";
                     break;
                  case ESupportedDbSystem.MSAccess2003:
                     result = "CURRENCY";
                     break;
                  default:
                     throw new NotSupportedTypeException();
               }
               break;
            case @"System.Object":
               switch (SupportedDbSystem)
               {
                  case ESupportedDbSystem.SQLServer:
                     result = "sql_variant";
                     break;
                  case ESupportedDbSystem.SQLite:
                  case ESupportedDbSystem.MSAccess2003:
                  default:
                     throw new NotSupportedTypeException();
               }
               break;
            case @"System.String":
               switch (SupportedDbSystem)
               {
                  case ESupportedDbSystem.SQLServer:
                     result = "text";
                     break;
                  case ESupportedDbSystem.SQLite:
                     result = "NVARCHAR";
                     break;
                  case ESupportedDbSystem.MSAccess2003:
                     result = "MEMO";
                     break;
                  default:
                     throw new NotSupportedTypeException();
               }
               break;
            case @"System.IntPtr":
            case @"System.UIntPtr":
            default:
               throw new NotSupportedTypeException();
         }
         return result;
      }
      /// <summary>
      /// 获取可映射的非托管数据类型实例映射到当前实例指定数据库系统所对应的数据类型名称字符串。
      /// </summary>
      /// <param name="unmanagedTypeName">非托管数据类型的名称字符串。</param>
      /// <returns>该操作会返回当前实例所指定的DBS所对应的非托管数据类型的名称字符串。</returns>
      /// <exception cref="NotSupportedTypeException">当参数unmanagedTypeName指定的非托管数据类型无法被映射到目标数据库系统的数据类型时，则会抛出这个异常。</exception>
      public string UnmanagedTypeMapping(string unmanagedTypeName)
      {
         string result = string.Empty;
         switch (unmanagedTypeName)
         {
            case @"BOOL":
            case @"BOOLEAN":
            case @"INT":
            case @"INT32":
            case @"LONG":
            case @"LONG32":
               result = CTSMapping(0, true);
               break;
            case @"BYTE":
            case @"USHORT":
            case @"WORD":
               result = CTSMapping((ushort)0, true);
               break;
            case @"CHAR":
            case @"SHORT":
               result = CTSMapping((short)0, true);
               break;
            case @"COLORREF":
            case @"DWORD":
            case @"DWORD32":
            case @"LPCVOID":
            case @"LPHANDLE":
            case @"LPVOID":
            case @"PCWCH":
            case @"PCWSTR":
            case @"PHANDLE":
            case @"PHKEY":
            case @"PLCID":
            case @"PLUID":
            case @"PVOID":
            case @"REGSAM":
            case @"SIZE_T":
            case @"SSIZE_":
            case @"UINT":
            case @"UINT32":
            case @"ULONG":
            case @"ULONG32":
               result = CTSMapping((uint)0, true);
               break;
            case @"DWORD64":
            case @"UINT64":
            case @"ULONG64":
            case @"ULONGLONG":
               result = CTSMapping((ulong)0, true);
               break;
            case @"FLOAT":
               result = CTSMapping((float)0, true);
               break;

            case @"INT64":
            case @"LONG64":
            case @"LONGLONG":
               result = CTSMapping((long)0, true);
               break;
            case @"LPCSTR":
            case @"LPCTSTR":
            case @"LPCWSTR":
            case @"LPSTR":
            case @"LPTSTR":
            case @"LPWSTR":
            case @"PCSTR":
            case @"PCTSTR":
            case @"PSTR":
            case @"PTSTR":
            case @"PWSTR":
               result = CTSMapping(string.Empty);
               break;
            case @"TBYTE":
            case @"TCHAR":
               result = CTSMapping('\x0', true);
               break;
            case @"UCHAR":
               result = CTSMapping((byte)0, true);
               break;
            default:
               throw new NotSupportedTypeException();
         }
         return result;
      }
      /// <summary>
      /// 指示指定的数据类型实例是否是受通用类型系统（CTS）支持的数据类型。
      /// </summary>
      /// <param name="obj">用于判断的数据类型实例。</param>
      /// <returns>如果指定的数据类型是CTS支持的数据类型，则会返回true，否则返回false。</returns>
      public bool IsCtsDefinedType(object obj)
      {
         bool isCtsDefinedType;
         switch (obj.GetType().FullName)
         {
            case @"System.Byte":
            case @"System.SByte":
            case @"System.Int16":
            case @"System.Int32":
            case @"System.Int64":
            case @"System.UInt16":
            case @"System.UInt32":
            case @"System.UInt64":
            case @"System.Single":
            case @"System.Double":
            case @"System.Boolean":
            case @"System.Char":
            case @"System.Decimal":
            case @"System.IntPtr":
            case @"System.UIntPtr":
            case @"System.Object":
            case @"System.String":
            case @"System.DateTime":
               isCtsDefinedType = true;
               break;
            default:
               isCtsDefinedType = false;
               break;
         }
         return isCtsDefinedType;
      }
      /// <summary>
      /// 指示指定的类型是否是除结构和枚举外的值类型。
      /// </summary>
      /// <param name="obj">需要被判断的类型。</param>
      /// <returns>如果指定的数据类型是除结构和枚举外的值类型，则会返回true，否则返回false。</returns>
      public bool IsNonStructOrEnumValueType(object obj)
      {
         bool isNonStructOrEnumCtsDefinedType;
         switch (obj.GetType().FullName)
         {
            case @"System.Byte":
            case @"System.SByte":
            case @"System.Int16":
            case @"System.Int32":
            case @"System.Int64":
            case @"System.UInt16":
            case @"System.UInt32":
            case @"System.UInt64":
            case @"System.Single":
            case @"System.Double":
            case @"System.Boolean":
            case @"System.Char":
            case @"System.Decimal":
            case @"System.IntPtr":
            case @"System.UIntPtr":
            case @"System.DateTime":
               isNonStructOrEnumCtsDefinedType = true;
               break;
            default:
               isNonStructOrEnumCtsDefinedType = false;
               break;
         }
         return isNonStructOrEnumCtsDefinedType;
      }
      /// <summary>
      /// 获取日期时间类型映射到当前实例指定的数据库系统的数据类型的名称字符串。
      /// </summary>
      /// <param name="isConvertToTicks">是否作为Ticks（时间对应的毫秒计数）映射。</param>
      /// <returns>该操作会返回一个日期时间类型映射到当前实例指定DBS类型的数据类型的名称字符串。</returns>
      /// <exception cref="NotSupportedTypeException">当日期时间数据类型无法被映射到目标数据库系统的日期时间数据类型时，则会抛出这个异常。</exception>
      public string DateTimeTypeMapping(bool isConvertToTicks)
      {
         string result = string.Empty;
         switch (SupportedDbSystem)
         {
            case ESupportedDbSystem.SQLServer:
               if (isConvertToTicks) result = "bigint"; else result = "datetime2";
               break;
            case ESupportedDbSystem.SQLite:
               if (isConvertToTicks) result = "BIGINT"; else throw new NotSupportedTypeException();
               break;
            case ESupportedDbSystem.MSAccess2003:
               if (isConvertToTicks) result = "LONG"; else result = "DATETIME";
               break;
            default:
               throw new NotSupportedTypeException();
         }
         return result;
      }
   }
   /// <summary>
   /// Cabinink Common Library（CCL）支持的数据库系统的名称枚举。
   /// </summary>
   public enum ESupportedDbSystem : int
   {
      /// <summary>
      /// SQL Server数据库系统。
      /// </summary>
      [EnumerationDescription("Microsoft SQL Server")]
      SQLServer = 0x0000,
      /// <summary>
      /// SQLite数据库系统。
      /// </summary>
      [EnumerationDescription("SQLite")]
      SQLite = 0x0001,
      /// <summary>
      /// Microsoft Access 97-2003数据库系统。
      /// </summary>
      [EnumerationDescription("Microsoft Access 97-2003")]
      MSAccess2003 = 0x0002
   }
}
