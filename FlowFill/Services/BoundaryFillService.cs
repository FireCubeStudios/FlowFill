using FlowFill.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace FlowFill.Services
{
    public class BoundaryFillService
    {
        private InkCanvas inkCanvas;
        private int Width;
        private int Height;
        private HashSet<Point> Points;
        private List<Point> FillPoints = new List<Point>();

        public BoundaryFillService(InkCanvas canvas)
        {
            inkCanvas = canvas;
            Width = (int)inkCanvas.Width;
            Height = (int)inkCanvas.Height;
        }
        //switch between varitaions
        public async void Fill(List<Point> BoundaryStroke)
        {
            Points = BoundaryStroke.ToHashSet();
            // OptimizedRecusrivePointFillAsync has scanline style features but originates from a single point if the recursive portion has await used
            // await OptimizedRecursivePointFillAsync(25, 25);
            //  Scanline style
            //  await RecursivePointFillAsync(30, 30);
            await Task.Run(async () =>
            {
                await ParallelRecursivePointFill(30, 30);
            });
            /* await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 CanvasHelper.CreateStroke(FillPoints, inkCanvas);
             });*/
        }
        private async Task RecursivePointFill(double x, double y)
        {

                  var watch = Stopwatch.StartNew();
                  if (await HasStrokeASync(x, y) && x < Width && y < Height && y > 0 && x > 0)
                      return;
                  Points.Add(new Point(x, y));
            // FillPoints.Add(new Point(x, y));
            RecursivePointFill(x + 1, y);
            RecursivePointFill(x - 1, y);
            RecursivePointFill(x, y + 1);
                  RecursivePointFill(x, y - 1);

              watch.Stop();
              await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
              {
                  CanvasHelper.CreateStroke(new Point(x, y), inkCanvas);
              });
              Debug.WriteLine(Points.Count);
              Debug.WriteLine(watch.ElapsedMilliseconds);
        }
        private async Task ParallelRecursivePointFill(double x, double y)
        {
            var watch = Stopwatch.StartNew();
            if (await HasStrokeASync(x, y) && x < Width && y < Height && y > 0 && x > 0)
                return;
            Points.Add(new Point(x, y));
            // FillPoints.Add(new Point(x, y));
            Parallel.Invoke(() => RecursivePointFill(x, y + 1),
                            () => RecursivePointFill(x, y - 1),
                            () => RecursivePointFill(x + 1, y),
                            () => RecursivePointFill(x - 1, y));
            ;
            watch.Stop();
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                CanvasHelper.CreateStroke(new Point(x, y), inkCanvas);
            });
            Debug.WriteLine(Points.Count);
            Debug.WriteLine(watch.ElapsedMilliseconds);
        }

        private async Task RecursivePointFillAsync(double x, double y)
        {
            await Task.Run(async () =>
            {
                if (await HasStroke(x, y) && x < Width && y < Height && y > 0 && x > 0)
                    return;
                Points.Add(new Point(x, y));
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CanvasHelper.CreateStroke(new Point(x, y), inkCanvas);
                });
                 await RecursivePointFillAsync(x + 1, y);
                 await RecursivePointFillAsync(x - 1, y);
                 await RecursivePointFillAsync(x, y + 1);
                 await RecursivePointFillAsync(x, y - 1);
            });
        }

        private async Task OptimizedRecursivePointFillAsync(double x, double y)
        {
            await Task.Run(async () =>
            {
                if (await HasStroke(x, y) && x < Width && y < Height && y > 0 && x > 0)
                    return;
               // Points.Add(new Point(x, y));
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CanvasHelper.CreateStroke(new Point(x, y), inkCanvas);
                });
                await RecursivePointFillAsyncRight(x + 1, y);
                await RecursivePointFillAsyncLeft(x - 1, y);
                await RecursivePointFillAsyncUp(x, y + 1);
                await RecursivePointFillAsyncDown(x, y - 1);
            });
        }

        private async Task RecursivePointFillAsyncDown(double x, double y)
        {
            await Task.Run(async () =>
            {
                if (await HasStroke(x, y) && x < Width && y < Height && y > 0 && x > 0)
                    return;
               // Points.Add(new Point(x, y));
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CanvasHelper.CreateStroke(new Point(x, y), inkCanvas);
                });
                await RecursivePointFillAsyncRight(x + 1, y);
                await RecursivePointFillAsyncLeft(x - 1, y);
                await RecursivePointFillAsyncDown(x, y - 1);
            });
        }

        private async Task RecursivePointFillAsyncUp(double x, double y)
        {
            await Task.Run(async () =>
            {
                if (await HasStroke(x, y) && x < Width && y < Height && y > 0 && x > 0)
                    return;
               // Points.Add(new Point(x, y));
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CanvasHelper.CreateStroke(new Point(x, y), inkCanvas);
                });
                await RecursivePointFillAsyncRight(x + 1, y);
                await RecursivePointFillAsyncLeft(x - 1, y);
                await RecursivePointFillAsyncUp(x, y + 1);
            });
        }

        private async Task RecursivePointFillAsyncLeft(double x, double y)
        {
            await Task.Run(async () =>
            {
                if (await HasStroke(x, y) && x < Width && y < Height && y > 0 && x > 0)
                    return;
                //Points.Add(new Point(x, y));
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CanvasHelper.CreateStroke(new Point(x, y), inkCanvas);
                });
                await RecursivePointFillAsyncLeft(x - 1, y);
                await RecursivePointFillAsyncDown(x, y + 1);
                await RecursivePointFillAsyncUp(x, y - 1);
            });
        }

        private async Task RecursivePointFillAsyncRight(double x, double y)
        {
            await Task.Run(async () =>
            {
                if (await HasStroke(x, y) && x < Width && y < Height && y > 0 && x > 0)
                    return;
              //  Points.Add(new Point(x, y));
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CanvasHelper.CreateStroke(new Point(x, y), inkCanvas);
                });
                await RecursivePointFillAsyncRight(x + 1, y);
                await RecursivePointFillAsyncDown(x, y + 1);
                await RecursivePointFillAsyncUp(x, y - 1);
            });
        }

        public async Task<bool> HasStroke(double x, double y)
        {
            bool HasStroke = false;
            await Task.Run(() =>
            {
                foreach(var point in Points)
                {
                    if (point.X == x && point.Y == y)
                        HasStroke = true;
                   /* else if (Math.Floor(point.X) == x && Math.Floor(point.Y) == y)
                        HasStroke = true;
                    else if (Math.Ceiling(point.X) == x && Math.Ceiling(point.Y) == y)
                        HasStroke = true;*/
                }
             
            });
            return HasStroke;
        }
        public async Task<bool> HasStrokeASync(double x, double y)
        {
            bool HasStroke = false;
            await Task.Run(() =>
            {
                if (Points.Contains(new Point(x, y)))
                    HasStroke = true;
            });
            return HasStroke;
        }
    }

}
