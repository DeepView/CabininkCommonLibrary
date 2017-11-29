namespace Cabinink.TypeExtend.Geometry2D
{
   /// <summary>
   /// 顶点集合转换接口，用于实现顶点集合转换为数组的特性。
   /// </summary>
   public interface IVertexToArray
   {
      /// <summary>
      /// 将当前图形的所有顶点装入到ExPoint2D数组之中。
      /// </summary>
      /// <returns>该操作会返回一个ExPoint2D数组，这个数组包含了当前图形所有的顶点。</returns>
      ExPoint2D[] VertexesToArray();
   }
}