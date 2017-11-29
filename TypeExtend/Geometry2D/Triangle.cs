using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Cabinink.TypeExtend.Geometry2D
{
   /// <summary>
   /// 平面三角形数学描述类。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public class Triangle : RegionallyShape, IVertexToArray, IInteriorAngles, IEquatable<Triangle>
   {
      private (ExPoint2D, ExPoint2D, ExPoint2D) _vertexes;//三角形的三个顶点。
      private const double TRIANGLE_INTERIOR_ANGLES_SIZE_SUM = 180;//常量，三角形内角和定义。
      /// <summary>
      /// 构造函数，创建一个指定顶点的三角形数学描述实例。
      /// </summary>
      /// <param name="vertexes">指定的有效顶点集合。</param>
      /// <exception cref="NonStandardGraphicalException">当定点定义不符合该实例的数学标准时，则将会抛出这个异常。</exception>
      public Triangle((ExPoint2D, ExPoint2D, ExPoint2D) vertexes)
      {
         bool inLine01 = new StraightLine2D(vertexes.Item1, vertexes.Item2).IsMemberPoint(vertexes.Item3);
         bool inLine02 = new StraightLine2D(vertexes.Item1, vertexes.Item3).IsMemberPoint(vertexes.Item2);
         bool inLine03 = new StraightLine2D(vertexes.Item2, vertexes.Item3).IsMemberPoint(vertexes.Item1);
         if (inLine01 || inLine02 || inLine03) throw new NonStandardGraphicalException();
         else _vertexes = vertexes;
      }
      /// <summary>
      /// 获取当前实例所表示的三角形的面积。
      /// </summary>
      public override double MeasureOfArea
      {
         get
         {
            StraightLine2D lineAC = GetCorrespondingEdge(Vertexes.Item2);
            StraightLine2D lineBC = GetCorrespondingEdge(Vertexes.Item1);
            Angle angle = new Angle(EAngleMeteringMode.Radian) { Size = lineAC.GetIntersectionLineAngle(lineBC) };
            return 0.5 * lineAC.GetDistanceWith() * lineBC.GetDistanceWith() * angle.GetSinusoidal();
         }
      }
      /// <summary>
      /// 获取当前实例所表示的三角形的周长。
      /// </summary>
      public override double Perimeter
      {
         get
         {
            StraightLine2D lineAB = GetCorrespondingEdge(Vertexes.Item3);
            StraightLine2D lineAC = GetCorrespondingEdge(Vertexes.Item2);
            StraightLine2D lineBC = GetCorrespondingEdge(Vertexes.Item1);
            return lineAB.GetDistanceWith() + lineAC.GetDistanceWith() + lineBC.GetDistanceWith();
         }
      }
      /// <summary>
      /// 获取或设置当前实例所表示的三角形的顶点集合。
      /// </summary>
      /// <exception cref="NonStandardGraphicalException">当定点定义不符合该实例的数学标准时，则将会抛出这个异常。</exception>
      public (ExPoint2D, ExPoint2D, ExPoint2D) Vertexes
      {
         get => _vertexes;
         set
         {
            bool inLine01 = new StraightLine2D(value.Item1, value.Item2).IsMemberPoint(value.Item3);
            bool inLine02 = new StraightLine2D(value.Item1, value.Item3).IsMemberPoint(value.Item2);
            bool inLine03 = new StraightLine2D(value.Item2, value.Item3).IsMemberPoint(value.Item1);
            if (inLine01 || inLine02 || inLine03) throw new NonStandardGraphicalException();
            else _vertexes = value;
         }
      }
      /// <summary>
      /// 将当前图形在横轴上平移指定的长度。
      /// </summary>
      /// <param name="offset">偏移量，即平移的长度。</param>
      public override void AbscissaTranslation(double offset)
      {
         ExPoint2D[] ps = new ExPoint2D[] { Vertexes.Item1, Vertexes.Item2, Vertexes.Item3 };
         for (int i = 0; i < 3; i++) ps[i].AbscissaTranslation(offset);
      }
      /// <summary>
      /// 将当前图形在纵轴上平移指定的长度。
      /// </summary>
      /// <param name="offset">偏移量，即平移的长度。</param>
      public override void OrdinateTranslation(double offset)
      {
         ExPoint2D[] ps = new ExPoint2D[] { Vertexes.Item1, Vertexes.Item2, Vertexes.Item3 };
         for (int i = 0; i < 3; i++) ps[i].OrdinateTranslation(offset);
      }
      /// <summary>
      /// 将当前的图形围绕指定点旋转指定角度。
      /// </summary>
      /// <param name="axisPoint">二维点旋转的中心点。</param>
      /// <param name="angle">逆时针旋转的角度。</param>
      /// <returns>这个操作会返回一个通过逆时针旋转后获取到的新的图形实例。</returns>
      public override Shape2D Rotate(ExPoint2D axisPoint, Angle angle)
      {
         ExPoint2D[] ps = new ExPoint2D[] { Vertexes.Item1, Vertexes.Item2, Vertexes.Item3 };
         ExPoint2D[] psnew = ps;
         for (int i = 0; i < 3; i++) psnew[i] = (ExPoint2D)ps[i].Rotate(axisPoint, angle);
         return new Triangle((psnew[0], psnew[1], psnew[2]));
      }
      /// <summary>
      /// 将整个图形进行坐标平移。
      /// </summary>
      /// <param name="offsetPoint">偏移量，即在横轴和纵轴上平移的长度。</param>
      public override void Translation(ExPoint2D offsetPoint)
      {
         for (int i = 0; i < 3; i++) VertexesToArray()[i].Translation(offsetPoint);
      }
      /// <summary>
      /// 判断指定的ExPoint2D实例是否为当前三角形的顶点。
      /// </summary>
      /// <param name="point">用于作判断的ExPoint2D实例。</param>
      /// <returns>如果当前判断操作返回true，则表示参数point指定的点是当前三角形数学描述实例中的顶点，否则结果相反。</returns>
      public bool IsVertex(ExPoint2D point)
      {
         bool isVertex = false;
         List<ExPoint2D> vertexes = VertexesToArray().ToList();
         for (int i = 0; i < vertexes.Count; i++)
         {
            if (point.Equals(vertexes[i]))
            {
               isVertex = true;
               break;
            }
         }
         return isVertex;
      }
      /// <summary>
      /// 获取三角形指定顶点所对应的边，比如顶点A对应的边为a=BC。
      /// </summary>
      /// <param name="vertex">指定的顶点，如果顶点不存在则将会抛出异常。</param>
      /// <returns>该操作会返回指定定点所对应的那一条边，不过严格来说，该方法返回的是对应边所在的直线，如果要获取这条边的长度，请访问StraightLine2D.GetDistanceWith方法。</returns>
      /// <exception cref="ArgumentException">当参数vertex提供的顶点无效时，则会抛出这个异常。</exception>
      public StraightLine2D GetCorrespondingEdge(ExPoint2D vertex)
      {
         StraightLine2D line = new StraightLine2D((0, 0), (100, 100));
         if (!IsVertex(vertex)) throw new ArgumentException("参数提供的顶点无效！", "vertex");
         else
         {
            if (vertex.Equals(Vertexes.Item1)) line = new StraightLine2D(Vertexes.Item2, Vertexes.Item3);
            if (vertex.Equals(Vertexes.Item2)) line = new StraightLine2D(Vertexes.Item1, Vertexes.Item3);
            if (vertex.Equals(Vertexes.Item3)) line = new StraightLine2D(Vertexes.Item1, Vertexes.Item2);
         }
         return line;
      }
      /// <summary>
      /// 获取指定顶点所在内角的大小，以角度制作为计量依据。
      /// </summary>
      /// <param name="vertex">指定的顶点，如果顶点不存在则将会抛出异常。</param>
      /// <returns>该操作会返回指定顶点所在内角的角度尺寸，并以角度制作为计量依据，如果要使用弧度制作为计量依据，请调用Angle.Convert方法。</returns>
      /// <exception cref="ArgumentException">当参数vertex提供的顶点无效时，则会抛出这个异常。</exception>
      public double GetInteriorAngleSize(ExPoint2D vertex)
      {
         Angle interiorAngle = new Angle(EAngleMeteringMode.Radian);
         StraightLine2D intersectStraightLine = new StraightLine2D((0, 0), (100, 100));
         StraightLine2D anotherIntersectStraightLine = new StraightLine2D((0, 0), (100, 100));
         if (!IsVertex(vertex)) throw new ArgumentException("参数提供的顶点无效！", "vertex");
         else
         {
            if (vertex.Equals(Vertexes.Item1))
            {
               intersectStraightLine = new StraightLine2D(Vertexes.Item1, Vertexes.Item2);
               anotherIntersectStraightLine = new StraightLine2D(Vertexes.Item1, Vertexes.Item3);
            }
            if (vertex.Equals(Vertexes.Item2))
            {
               intersectStraightLine = new StraightLine2D(Vertexes.Item1, Vertexes.Item2);
               anotherIntersectStraightLine = new StraightLine2D(Vertexes.Item2, Vertexes.Item3);
            }
            if (vertex.Equals(Vertexes.Item3))
            {
               intersectStraightLine = new StraightLine2D(Vertexes.Item2, Vertexes.Item3);
               anotherIntersectStraightLine = new StraightLine2D(Vertexes.Item1, Vertexes.Item3);
            }
            interiorAngle.Size = intersectStraightLine.GetIntersectionLineAngle(anotherIntersectStraightLine);
         }
         interiorAngle.Convert();
         return interiorAngle.Size;
      }
      /// <summary>
      /// 获取当前图形的内角和。
      /// </summary>
      /// <returns>该操作将会返回当前图形的内角和，并以角度制作为计量依据，如果要使用弧度制作为计量依据，请调用Angle.Convert方法。</returns>
      public double GetInteriorAnglesSizeSum() => TRIANGLE_INTERIOR_ANGLES_SIZE_SUM;
      /// <summary>
      /// 将当前图形的所有顶点装入到ExPoint2D数组之中。
      /// </summary>
      /// <returns>该操作会返回一个ExPoint2D数组，这个数组包含了当前图形所有的顶点。</returns>
      public ExPoint2D[] VertexesToArray() => new ExPoint2D[] { Vertexes.Item1, Vertexes.Item2, Vertexes.Item3 };
      /// <summary>
      /// 检查当前实例从数学层面上来看，是否与指定的实例相同。
      /// </summary>
      /// <param name="other">指定需要比较的实例。</param>
      /// <returns>如果该操作证明源实例与目标实例在数学层面上相同，则会返回true，否则将会返回false。</returns>
      public bool Equals(Triangle other)
      {
         bool isSame = true;
         for (int i = 0; i < 3; i++)
         {
            for(int j = 0;j < 3;j++)
            {
               if (!other.VertexesToArray()[i].Equals(VertexesToArray()[j]))
               {
                  isSame = false;
                  break;
               }
               if (!isSame) break;
            }
         }
         return isSame;
      }
   }
}
