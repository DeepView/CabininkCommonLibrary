namespace Cabinink.TypeExtend.Geometry2D
{
   /// <summary>
   /// 多边形内角和相关接口。
   /// </summary>
   interface IInteriorAngles
   {
      /// <summary>
      /// 获取指定顶点所在内角的大小，以角度制作为计量依据。
      /// </summary>
      /// <param name="vertex">指定的顶点，如果顶点不存在则将会抛出异常。</param>
      /// <returns>该操作会返回指定顶点所在内角的角度尺寸，并以角度制作为计量依据，如果要使用弧度制作为计量依据，请调用Angle.Convert方法。</returns>
      double GetInteriorAngleSize(ExPoint2D vertex);
      /// <summary>
      /// 获取当前图形的内角和。
      /// </summary>
      /// <returns>该操作将会返回当前图形的内角和，并以角度制作为计量依据，如果要使用弧度制作为计量依据，请调用Angle.Convert方法。</returns>
      double GetInteriorAnglesSizeSum();
   }
}
