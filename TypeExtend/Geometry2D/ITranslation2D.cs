namespace Cabinink.TypeExtend.Geometry2D
{
   /// <summary>
   /// 平面几何图形坐标平移接口
   /// </summary>
   public interface ITranslation2D
   {
      /// <summary>
      /// 将当前图形在横轴上平移指定的长度。
      /// </summary>
      /// <param name="offset">偏移量，即平移的长度。</param>
      void AbscissaTranslation(double offset);
      /// <summary>
      /// 将当前图形在纵轴上平移指定的长度。
      /// </summary>
      /// <param name="offset">偏移量，即平移的长度。</param>
      void OrdinateTranslation(double offset);
      /// <summary>
      /// 将整个图形进行坐标平移。
      /// </summary>
      /// <param name="offsetPoint">偏移量，即在横轴和纵轴上平移的长度。</param>
      void Translation(ExPoint2D offsetPoint);
   }
}