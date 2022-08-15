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
    public class ScanLineFillService
    {
        private InkCanvas inkCanvas;
        private int Width;
        private int Height;
        private List<Point> Points;

        public ScanLineFillService(InkCanvas canvas)
        {
            inkCanvas = canvas;
            Width = (int)inkCanvas.Width;
            Height = (int)inkCanvas.Height;
        }
        //switch between varitaions
        public async void Fill(List<Point> BoundaryStroke)
        {
            Points = await CanvasHelper.GetPointsAsync(inkCanvas);
            var watch = Stopwatch.StartNew();
            await ParallelLineFillAsync(BoundaryStroke);
            watch.Stop();
            Debug.WriteLine(watch.ElapsedMilliseconds);
        }

        private async Task ParallelLineFillAsync(List<Point> BoundaryStroke)
        {
            await Task.Run(() =>
            {
                Parallel.For(0, Height, async y => {
                    bool IsDrawing = false; //bool to determine if drawing
                    Point A;
                    Point B;
                    for (int x = 0; x < Width; x++)
                    {
                        foreach (var point in BoundaryStroke)
                        {
                            if (point.X == x && point.Y == y)
                            {
                                if (IsDrawing == true)
                                {
                                    B = new Point(x, y);
                                    IsDrawing = false;
                                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                    {
                                        CanvasHelper.CreateStroke(A, B, inkCanvas);
                                    });
                                }
                                else
                                {
                                    A = new Point(x, y);
                                    IsDrawing = true;
                                }
                            }
                        }
                    }
                });
            });
        }

        private async Task ParallelPointFillAsync(List<Point> BoundaryStroke)
        {
            await Task.Run(() =>
            {
                Parallel.For(0, Height, async y => {
                    bool IsDrawing = false; 
                    for (int x = 0; x < Width; x++)
                    {
                        foreach (var point in BoundaryStroke)
                        {
                            if (point.X == x && point.Y == y)
                            {
                                IsDrawing = !IsDrawing; //Odd - even - odd rule, the point will be filled or not
                            }
                        }
                        if (IsDrawing)
                        {
                            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                CanvasHelper.CreateStroke(new Point(x, y), inkCanvas);
                            });
                        }
                    }
                });
            });
        }

        // draws line by line instead of per point
        private async Task LineFillAsync(List<Point> BoundaryStroke)
        {
            await Task.Run(async () =>
            {
                for (double y = 0; y < Height; y += 0.5)
                {
                    bool IsDrawing = false; //bool to determine if drawing
                    Point A;
                    Point B;
                    for (double x = 0; x < Width; x += 0.5)
                    {
                        foreach (var point in BoundaryStroke)
                        {
                            if (point.X == x && point.Y == y)
                            {
                                if(IsDrawing == true)
                                {
                                    B = new Point(x, y);
                                    IsDrawing = false;
                                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                    {
                                        CanvasHelper.CreateStroke(A, B, inkCanvas);
                                    });
                                }
                                else
                                {
                                    A = new Point(x, y);
                                    IsDrawing = true;
                                }
                            }
                        }
                    }
                }
            });
        }

        private async Task PointFillAsync(List<Point> BoundaryStroke)
        {
            await Task.Run(async ()=> 
            { 
                //  Points = await CanvasHelper.GetPointsAsync(inkCanvas);
                for (int y = 0; y < Height; y++)
                {
                    bool IsDrawing = false; //bool to determine if point should be filled
                    // Loop through each row of points in canvas
                    for (int x = 0; x < Width; x++)
                    {
                        /* if (y == 15 || y > 30)
                             break;*/
                        foreach (var point in BoundaryStroke)
                        {
                            // Debug.WriteLine("p: " + point.X + " py: " + point.Y + " x: " + x + " y: " + y);
                            if (point.X == x && point.Y == y)
                            {
                                IsDrawing = !IsDrawing; //Odd - even - odd rule, the point will be filled or not
                            }
                        }
                        if (IsDrawing)
                        {
                            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                CanvasHelper.CreateStroke(new Point(x, y), inkCanvas);
                            });
                        }
                    }
                }
            });
        }

        private async void PointFill(List<Point> BoundaryStroke)
        {
            //  Points = await CanvasHelper.GetPointsAsync(inkCanvas);
            for (int y = 0; y < Height; y++)
            {
                bool IsDrawing = false; //bool to determine if point should be filled
                // Loop through each row of points in canvas
                for (int x = 0; x < Width; x++)
                {
                   /* if (y == 15 || y > 30)
                        break;*/
                   foreach(var point in BoundaryStroke)
                    {
                       // Debug.WriteLine("p: " + point.X + " py: " + point.Y + " x: " + x + " y: " + y);
                        if (point.X == x && point.Y == y)
                        {
                            IsDrawing = !IsDrawing; //Odd - even - odd rule, the point will be filled or not
                        }
                    }
                    if(IsDrawing)
                    {
                        CanvasHelper.CreateStroke(new Point(x, y), inkCanvas);
                    }
                }
            }
        }
    }
}