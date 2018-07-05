using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using Cabinink.TypeExtend.Collections;
namespace Cabinink.DataTreatment.ORMapping
{
   /// <summary>
   /// 对象成员或者字段提取类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class ObjectMemberGetter
   {
      private object _operatedObject;//被用于提取成员信息的对象。
      /// <summary>
      /// 构造函数，给当前实例传递一个被用于提取成员信息的对象。
      /// </summary>
      /// <param name="operatedObject">需要传递的对象，这个对象可以是任何直接或者间接继承自System.Object的对象。</param>
      /// <exception cref="NotSupportedTypeException">当传递的对象不支持时，则会抛出这个异常，在这里，这个不支持的对象即当前构造函数所在的类或者类的实例。</exception>
      public ObjectMemberGetter(object operatedObject)
      {
         if (operatedObject.GetType().ToString() == @"ObjectMemberGetter") throw new NotSupportedTypeException();
         else _operatedObject = operatedObject;
      }
      /// <summary>
      /// 获取或设置当前实例的被用于操作的对象。
      /// </summary>
      /// <exception cref="NotSupportedTypeException">当传递的对象不支持时，则会抛出这个异常，在这里，这个不支持的对象即当前构造函数所在的类或者类的实例。</exception>
      public object OperatedObject
      {
         get => _operatedObject;
         set
         {
            if (value.GetType().ToString() == @"ObjectMemberGetter") throw new NotSupportedException();
            _operatedObject = value;
         }
      }
      /// <summary>
      /// 获取当前被操作对象所包含的所有属性的属性名称。
      /// </summary>
      /// <returns>这个操作会返回一个List列表，这个列表存储了被操作对象的所有的属性的属性名称。</returns>
      public List<string> GetPropertyNames()
      {
         List<string> pNames = new List<string>();
         List<PropertyInfo> pInfos = GetPropertyInfoCollection().ToList();
         for (int i = 0; i < pInfos.Count; i++) pNames.Add(pInfos[i].Name);
         return pNames;
      }
      /// <summary>
      /// 获取当前被操作对象所包含的所有属性的属性类型字符串。
      /// </summary>
      /// <returns>这个操作会返回一个List列表，这个列表存储了被操作对象的所有的属性的属性类型字符串。</returns>
      public List<string> GetPropertyTypes()
      {
         List<string> pTypes = new List<string>();
         List<PropertyInfo> pInfos = GetPropertyInfoCollection().ToList();
         for (int i = 0; i < pInfos.Count; i++) pTypes.Add(pInfos[i].PropertyType.ToString());
         return pTypes;
      }
      /// <summary>
      /// 获取当前被操作对象所包含的所有字段的字段名称。
      /// </summary>
      /// <returns>这个操作会返回一个List列表，这个列表存储了被操作对象的所有的字段的字段名称。</returns>
      public List<string> GetFieldNames()
      {
         List<string> fNames = new List<string>();
         List<FieldInfo> fInfos = GetFieldInfoCollection().ToList();
         for (int i = 0; i < fInfos.Count; i++) fNames.Add(fInfos[i].Name);
         return fNames;
      }
      /// <summary>
      /// 获取当前被操作对象所包含的所有字段的字段类型字符串。
      /// </summary>
      /// <returns>这个操作会返回一个List列表，这个列表存储了被操作对象的所有的字段的字段类型字符串。</returns>
      public List<string> GetFieldTypes()
      {
         List<string> fTypes = new List<string>();
         List<FieldInfo> fInfos = GetFieldInfoCollection().ToList();
         for (int i = 0; i < fInfos.Count; i++) fTypes.Add(fInfos[i].FieldType.ToString());
         return fTypes;
      }
      /// <summary>
      /// 根据成员访问权限来获取当前被操作对象所包含的所有字段的字段名称。
      /// </summary>
      /// <param name="isOvertField">用于决定获取哪种访问权限的字段名称，如果这个参数值为true，则只获取public权限的字段，否则获取其他权限的字段，比如说internal或者private。</param>
      /// <returns>这个操作会返回一个List列表，这个列表存储了被操作对象的指定权限类型的字段的字段名称。</returns>
      public List<string> GetFieldNamesWithJurisdiction(bool isOvertField)
      {
         List<string> fNames = new List<string>();
         List<FieldInfo> fInfos = GetFieldInfoCollection().ToList();
         if (isOvertField)
         {
            for (int i = 0; i < fInfos.Count; i++) if (fInfos[i].IsPublic) fNames.Add(fInfos[i].Name);
         }
         else for (int i = 0; i < fInfos.Count; i++) if (!fInfos[i].IsPublic) fNames.Add(fInfos[i].Name);
         return fNames;
      }
      /// <summary>
      /// 根据成员访问权限来获取当前被操作对象所包含的所有字段的字段类型字符串。
      /// </summary>
      /// <param name="isOvertField">用于决定获取哪种访问权限的字段类型字符串，如果这个参数值为true，则只获取public权限的字段，否则获取其他权限的字段，比如说internal或者private。</param>
      /// <returns>这个操作会返回一个List列表，这个列表存储了被操作对象的指定权限类型的字段的字段类型字符串。</returns>
      public List<string> GetFieldTypesWithJurisdiction(bool isOvertField)
      {
         List<string> fTypes = new List<string>();
         List<FieldInfo> fInfos = GetFieldInfoCollection().ToList();
         if (isOvertField)
         {
            for (int i = 0; i < fInfos.Count; i++) if (fInfos[i].IsPublic) fTypes.Add(fInfos[i].FieldType.ToString());
         }
         else for (int i = 0; i < fInfos.Count; i++) if (!fInfos[i].IsPublic) fTypes.Add(fInfos[i].FieldType.ToString());
         return fTypes;
      }
      /// <summary>
      /// 获取指定变量的名称。
      /// </summary>
      /// <typeparam name="T">表示变量名称的数据类型。</typeparam>
      /// <param name="expression">用于获取变量名称的Lambda表达式。</param>
      /// <returns>该操作将会返回参数memberExpression传递的表达式中包含的变量的变量名称。</returns>
      /// <example>
      /// 下面是一个简单的C#调用范例。
      /// <code>
      /// string myStr="I love China!";
      /// string nameOfVariable = MemberInfoGetting.GetMemberName(() => myStr);
      /// Console.Write("Variable Name = " + nameOfVariable);
      /// </code>
      /// 上面的代码将会在控制台输出如下结果：
      /// Variable Name = myStr
      /// </example>
      public static string GetExampleFieldName<T>(Expression<Func<T>> expression)
      {
         MemberExpression expressionBody = (MemberExpression)expression.Body;
         return expressionBody.Member.Name;
      }
      /// <summary>
      /// 获取当前实例包含的对象的公共属性的属性值。
      /// </summary>
      /// <returns>这个操作将会返回一个双向链表，考虑到属性值的顺序安全性，因此并不会采用列表或者数组，如有这个需求，请调用BiDirectionalLinkedList.ToList()或者BiDirectionalLinkedList.ToArray()方法。</returns>
      public BiDirectionalLinkedList<object> GetProperityValues()
      {
         BiDirectionalLinkedList<object> values = new BiDirectionalLinkedList<object>();
         PropertyInfo[] pInfos = GetPropertyInfoCollection();
         foreach (PropertyInfo pInfo in pInfos)
         {
            values.Add(pInfo.GetValue(OperatedObject));
         }
         return values;
      }
      /// <summary>
      /// 获取当前被操作对象的所有属性的信息。
      /// </summary>
      /// <returns>该操作将会返回所有属性的信息集合。</returns>
      private PropertyInfo[] GetPropertyInfoCollection() => OperatedObject.GetType().GetProperties();
      /// <summary>
      /// 获取当前被操作对象的所有字段的信息。
      /// </summary>
      /// <returns>该操作将会返回所有字段的信息集合。</returns>
      private FieldInfo[] GetFieldInfoCollection() => OperatedObject.GetType().GetFields();
   }
   /// <summary>
   /// 当所属类型不支持时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class NotSupportedTypeException : Exception
   {
      public NotSupportedTypeException() : base("不支持的类型或者当前类型没有访问与修改的意义！") { }
      public NotSupportedTypeException(string message) : base(message) { }
      public NotSupportedTypeException(string message, Exception inner) : base(message, inner) { }
      protected NotSupportedTypeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
