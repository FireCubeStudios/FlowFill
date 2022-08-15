using FlowFill.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FlowFill
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class MainPage : Page
    {
        public List<Point> BoundaryStroke = new List<Point>();
        public List<Point> FillStroke = new List<Point>();
        private ScanLineFillService ScanFillService;
        private BoundaryFillService FillService;
        public MainPage()
        {
            this.InitializeComponent();
            ScanFillService = new ScanLineFillService(inkCanvas);
            FillService = new BoundaryFillService(inkCanvas);
            SetDrawingAttributues();
        }

        private void SetDrawingAttributues()
        {
            InkDrawingAttributes ida = inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
            ida.FitToCurve = false;
            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(ida);
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.InkPresenter.StrokeContainer.Clear();
            BoundaryStroke.Clear();
            ScanFillStroke.Clear();
            FillStroke.Clear();
        }
        private void DrawSmallButton_Click(object sender, RoutedEventArgs e)
        {
            CreateSquare(15, 45);
        }
        private void DrawBigButton_Click(object sender, RoutedEventArgs e)
        {
            CreateSquare(150, 350);
        }
        private void DrawSmallBigButton_Click(object sender, RoutedEventArgs e)
        {
            CreateSquare(150, 250);
            CreateSquare(205, 235);
        }
        private void DrawTriangleButton_Click(object sender, RoutedEventArgs e)
        {
            CreateTriangle(150, 350);
        }
        private void PrintButton_Click(object sender, RoutedEventArgs e) => PrintPoints();
        private void PrintFillButton_Click(object sender, RoutedEventArgs e) => PrintFillPoints();
        private void BoundaryFillButton_Click(object sender, RoutedEventArgs e)
        {

            // Task.Factory.StartNew(() => RecursiveBoundaryFill(20, 20));

            FillService.Fill(BoundaryStroke);
        }
        private void BoundaryFillExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            // CreateStroke(FillStroke);     
            FillService.Fill(BoundaryStroke);
        }
        public async void RecursiveBoundaryFill(int x, int y)
        {
            /* await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,() =>
             {
                 Logs.Text = "";
                 Logs.Text += $"\n X: {x}, Y: {y}";
             });*/
            Logs.Text = "";
            Logs.Text += $"\n X: {x}, Y: {y}";
            if (HasStroke(x, y) == true)
            {
                return;
            }
            BoundaryStroke.Add(new Point(x, y));
            FillStroke.Add(new Point(x, y));
            RecursiveBoundaryFill(x + 1, y);
            RecursiveBoundaryFill(x - 1, y);
            RecursiveBoundaryFill(x, y + 1);
            RecursiveBoundaryFill(x, y - 1);
        }
        public bool HasStroke(int x, int y)
        {
             foreach (var point in BoundaryStroke)
             {
                 if (point.X == x && point.Y == y) 
                     return true;
                 else if(Math.Floor(point.X) == x && Math.Floor(point.Y) == y)
                    return true;
                 else if(Math.Ceiling(point.X) == x && Math.Ceiling(point.Y) == y)
                    return true;
            }
             return false;
        }
        public List<Point> GetPoints()
        {
            var targetStrokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();
            List<Point> Points = new List<Point>();
            foreach (var stroke in targetStrokes)
            {
                var points = stroke.GetInkPoints();
                foreach (var point in points)
                {
                    Points.Add(new Point(point.Position.X, point.Position.Y));
                }
            }
            return Points;
        }
        public void PrintFillPoints()
        {
            Logs.Text = "";
            foreach (var point in FillStroke)
            {
                Logs.Text += $"\n X: {point.X}, Y: {point.Y}";
            }
        }
        public void PrintPoints()
        {
            Logs.Text = "";
            foreach (var point in BoundaryStroke)
            {
                Logs.Text += $"\n X: {point.X}, Y: {point.Y}";
            }
        }
        public void CreateSquare(int l, int w)
        {
            List<Point> Points = new List<Point>();
            Points.Add(new Point(l, l));
            Points.Add(new Point(w, l));
            DDA(l, l, w, l);
            Points.Add(new Point(w, w));
            DDA(w, l, w, w);
            Points.Add(new Point(l, w));
            DDA(w, w, l, w);
            Points.Add(new Point(l, l));
            DDA(l, w, l, l);
            CreateStroke(Points);
        }
        public void CreateTriangle(int l, int w)
        {
            List<Point> Points = new List<Point>();
            Points.Add(new Point(30, 15));
            Points.Add(new Point(45, 45));
            DDA(30, 15, 45, 45);
            Points.Add(new Point(15, 45));
            Logs.Text += "\n";
            DDA(45, 45, 15, 45);
            Points.Add(new Point(30, 15));
            Logs.Text += "\n";
            DDA(15, 45, 30, 15);
            CreateStroke(Points);
        }
        public void DDA(double x0, double y0, double x1, double y1)
        {
            double dx = x1 - x0;
            double dy = y1 - y0;

            // Depending upon absolute value of dx & dy
            // choose number of steps to put pixel as
            // steps = abs(dx) > abs(dy) ? abs(dx) : abs(dy)
            double steps = Math.Abs(dx) > Math.Abs(dy) ? Math.Abs(dx) : Math.Abs(dy);

            // calculate increment in x & y for each steps
            double Xinc = dx / (float)steps;
            double Yinc = dy / (float)steps;

            // Put pixel for each step
            double X = x0;
            double Y = y0;
            for (int ii = 0; ii <= steps; ii++)
            {
                BoundaryStroke.Add(new Point(X, Y));
                Logs.Text += $"\n X: {X}, Y: {Y}";
                X += Xinc;
                Y += Yinc;
            }
        }

        public void CreateStroke(List<Point> Points)
        {
            var strokeBuilder = new InkStrokeBuilder();
            strokeBuilder.SetDefaultDrawingAttributes(inkCanvas.InkPresenter.CopyDefaultDrawingAttributes());;
            InkStroke stkA = strokeBuilder.CreateStroke(Points);
            inkCanvas.InkPresenter.StrokeContainer.AddStroke(stkA);
        }

        public void CreateStroke(int x, int y)
        {
            List<Point> Points = new List<Point>();
            Points.Add(new Point(x, y));
            var strokeBuilder = new InkStrokeBuilder();
            strokeBuilder.SetDefaultDrawingAttributes(inkCanvas.InkPresenter.CopyDefaultDrawingAttributes()); ;
            InkStroke stkA = strokeBuilder.CreateStroke(Points);
            inkCanvas.InkPresenter.StrokeContainer.AddStroke(stkA);
        }

        private void ScanFillButton_Click(object sender, RoutedEventArgs e)
        {
           for(int y = 150; y <= 350; y++)
            {
                for (int x = 150; x <= 350; x++)
                {
                    ScanFillStroke.Add(new Tuple<Point, Point>(new Point(x, y), new Point(x, y)));
                }
            }
        }
        private void xScanFillButton_Click(object sender, RoutedEventArgs e)
        {
            for (int y = 150; y <= 350; y++)
            {
                for (int x = 150; x <= 350; x++)
                {
                    ScanFillStroke.Add(new Tuple<Point, Point>(new Point(x, y), new Point(x, y)));
                }
            }
        }
        public List<Tuple<Point, Point>> ScanFillStroke = new List<Tuple<Point,Point>>();

        private async void ScanFillExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            /*  for (int i = 0; i <= ScanFillStroke.Count; i += 2)
              {
                  List<Point> Points = new List<Point>();
                  try
                  {
                      Points.Add(ScanFillStroke[i].Item1);
                      Points.Add(ScanFillStroke[i].Item2);
                  }
                  catch
                  {

                  }
                  CreateStroke(Points);
              }*/
            ScanFillService.Fill(BoundaryStroke);
        }
    }
}
