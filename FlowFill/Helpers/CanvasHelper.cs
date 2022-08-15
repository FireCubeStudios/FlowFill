using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;

namespace FlowFill.Helpers
{
    public class CanvasHelper
    {
        //  Points on Cubic Bezier Curve siehe https://www.cubic.org/docs/bezier.htm
        public static Point lerp(Point a, Point b, float t)
        {
            Point p = new Point()
            {
                X = a.X + (b.X - a.X) * t,
                Y = a.Y + (b.Y - a.Y) * t,

            };
            return p;
        }

        public static Point bezier(Point a, Point b, Point c, Point d, float t)
        {
            Point ab = new Point();
            Point bc = new Point();
            Point cd = new Point();
            Point abbc = new Point();
            Point bccd = new Point();

            Point bezierPt = new Point();

            ab = lerp(a, b, t);           // point between a and b
            bc = lerp(b, c, t);           // point between b and c
            cd = lerp(c, d, t);           // point between c and d
            abbc = lerp(ab, bc, t);       // point between ab and bc
            bccd = lerp(bc, cd, t);       // point between bc and cd
            bezierPt = lerp(abbc, bccd, t);   // point on the bezier-curve

            return bezierPt;
        }

        /// <summary>
        /// Gets all the points in the InkStroke segment
        /// </summary>
        /// <param name="startPt">The first ink point in the InkStroke.</param>
        /// <param name="controlPt1">The first BezierControlPoint of the InkStrokeRenderingSegment.</param>
        /// <param name="controlPt2">The second BezierControlPoint of the InkStrokeRenderingSegment.</param>
        /// <param name="PositionPt">Position of the InkStrokeRenderingSegment.</param>
        /// <returns>Returns a list of Point objects representing every Point in the InkStroke segment./returns>
        public static List<Point> PointsOnSegment(Point startPt, Point controlPt1, Point controlPt2, Point PositionPt)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < 10; i++)
            {
                Point p = new Point();
                float t = (float)(i) / 9.0f;
                p = bezier(startPt, controlPt1, controlPt2, PositionPt, t);
                points.Add(p);
            }
            return points;
        }

        /// <summary>
        /// Gets all the points in the InkStroke
        /// </summary>
        /// <param name="inst">An InkStroke object.</param>
        /// <returns>Returns a list of Point objects representing every Point in the InkStroke./returns>
        public static async Task<List<Point>> GetPointsAsync(InkCanvas inkCanvas)
        {
            var targetStrokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();
            List<Point> Points = new List<Point>();
            await Task.Run(() =>
            {
                foreach (var inst in targetStrokes)
                {
                    List<InkStrokeRenderingSegment> renderingSegments = new List<InkStrokeRenderingSegment>();
                    List<Point> points = new List<Point>();
                    points.Add(new Point() { X = inst.GetInkPoints().First().Position.X, Y = inst.GetInkPoints().First().Position.Y, });

                    foreach (InkStrokeRenderingSegment isrs in inst.GetRenderingSegments())
                    {
                        List<Point> pointsOnSg = new List<Point>();
                        pointsOnSg = PointsOnSegment(points.Last(), isrs.BezierControlPoint1, isrs.BezierControlPoint2, isrs.Position);
                        points.AddRange(pointsOnSg);
                    }
                    foreach(var p in points)
                    {
                        Point pp = new Point(Math.Floor(p.X), Math.Floor(p.Y));
                        if(!!Points.Contains(pp))
                        {
                            Points.Add(pp);
                        }
                    }
                }
            });
            return Points;
        }

        public static async Task<List<Point>> GetInkPointsAsync(InkCanvas inkCanvas)
        {
            var targetStrokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();
            List<Point> Points = new List<Point>();
            await Task.Run(() =>
            {
                foreach (var inst in targetStrokes)
                {
                    foreach (var p in inst.GetInkPoints())
                    {
                        Points.Add(new Point((int)p.Position.X, (int)p.Position.Y));
                    }
                }
            });
            return Points;
        }

        public static void CreateStroke(Point p, InkCanvas inkCanvas)
        {
            var strokeBuilder = new InkStrokeBuilder();
            List<Point> Points = new List<Point>();
            Points.Add(p);
            Points.Add(p);
            strokeBuilder.SetDefaultDrawingAttributes(inkCanvas.InkPresenter.CopyDefaultDrawingAttributes()); ;
            InkStroke stkA = strokeBuilder.CreateStroke(Points);
            inkCanvas.InkPresenter.StrokeContainer.AddStroke(stkA);
        }

        public static void CreateStroke(Point p, Point p2, InkCanvas inkCanvas)
        {
            var strokeBuilder = new InkStrokeBuilder();
            List<Point> Points = new List<Point>();
            Points.Add(p);
            Points.Add(p2);
            strokeBuilder.SetDefaultDrawingAttributes(inkCanvas.InkPresenter.CopyDefaultDrawingAttributes()); ;
            InkStroke stkA = strokeBuilder.CreateStroke(Points);
            inkCanvas.InkPresenter.StrokeContainer.AddStroke(stkA);
        }

        public static void CreateStroke(List<Point> Points, InkCanvas inkCanvas)
        {
                var strokeBuilder = new InkStrokeBuilder();
                strokeBuilder.SetDefaultDrawingAttributes(inkCanvas.InkPresenter.CopyDefaultDrawingAttributes()); ;
                InkStroke stkA = strokeBuilder.CreateStroke(Points);
                inkCanvas.InkPresenter.StrokeContainer.AddStroke(stkA);
        }

        public static void CreateStroke(int x, int y, InkCanvas inkCanvas)
        {
            List<Point> Points = new List<Point>();
            Points.Add(new Point(x, y));
            var strokeBuilder = new InkStrokeBuilder();
            strokeBuilder.SetDefaultDrawingAttributes(inkCanvas.InkPresenter.CopyDefaultDrawingAttributes()); ;
            InkStroke stkA = strokeBuilder.CreateStroke(Points);
            inkCanvas.InkPresenter.StrokeContainer.AddStroke(stkA);
        }
    }
}
