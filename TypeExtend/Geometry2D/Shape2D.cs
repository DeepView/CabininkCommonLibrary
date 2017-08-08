using System;
using System.Runtime.InteropServices;
namespace Cabinink.TypeExtend.Geometry2D
{
   /// <summary>
   /// 平面图形基础类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public abstract class Shape2D
   {
      /// <summary>
      /// 获取当前图形是否被允许在Graphics对象中绘制，通常而言，这个属性的值都为true。
      /// </summary>
      public virtual bool IsAllowDrawing => true;
      /// <summary>
      /// 获取当前实例的字符串表达形式。
      /// </summary>
      /// <returns>该操作将会返回一个字符串值，这个值表示当前实例包含命名空间的完整名称。</returns>
      public override string ToString() => @"Cabinink.TypeExtend.Geometry2D.Shape2D";
   }
}
