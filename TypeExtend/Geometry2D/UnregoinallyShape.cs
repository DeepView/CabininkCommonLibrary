using System;
using System.Runtime.InteropServices;
namespace Cabinink.TypeExtend.Geometry2D
{
   /// <summary>
   /// 没有数学区域占用的图形抽象类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public abstract class UnregoinallyShape : Shape2D
   {
      /// <summary>
      /// 获取当前图形是否被允许在Graphics对象中绘制，不过在不存在数学区域的图形中，这个属性的值都为false。
      /// </summary>
      public override bool IsAllowDrawing => !base.IsAllowDrawing;
   }
}
