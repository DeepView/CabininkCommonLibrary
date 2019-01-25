using System.Collections.Generic;
namespace Cabinink.Windows.Applications
{
   /// <summary>
   /// 用于规范RemoteCall的被调用方的基础访问API的接口。
   /// </summary>
   /// <typeparam name="T">Detail方法需要返回的数据类型或者实例。</typeparam>
   public interface IRemoteCallProtocol<T>
   {
      /// <summary>
      /// 提供给用户最基本的RemoteCall方法，主要用于处理构造函数。
      /// </summary>
      /// <param name="parameters">参数集合，需要绑定到T所对应的实例中每一个字段或者属性。</param>
      /// <returns>该操作将会返回绑定之后的实例，或者返回一个方法执行之后的结果。</returns>
      T Ctor(IDictionary<string, object> parameters);
   }
}
