using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MapWpfMVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MapWpfMVVM.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private IGraph GetGraph;
        Point scrollMousePoint = new Point();
        double hOff = 1;
        double vOff = 1;
        private int heightMin = 750;
        private int heightMax = 8033;
        private int widthMax = 14705;
        private int widthMin = 1388;
        private double zoom = 1.1;
        private string txbAtcAddressBeginText;
        private string txbAtcAddressEndText;
        private string txblDistanceText;
        private string txblTimeWalkText;
        private string txblTimeCarText;
        private ICommand loadedCommand;
        private ICommand previewMouseLeftButtonUpScrollRootCommand;
        private ICommand previewMouseLeftButtonDownScrollRootCommand;
        private ICommand previewMouseMoveScrollRootCommand;
        private ICommand previewMouseWheelScrollRootCommand;
        private ICommand sizeChangedGridRoot;
        private ICommand clickBtnFind;
        private ICommand selectionChangedTxbAtcAddressBeginCommand;
        private ICommand selectionChangedTxbAtcAddressEndCommand;

        private double thickness = 10.0;
        private ObservableCollection<string> listAddress;
        public ObservableCollection<string> ListAddress { get => listAddress; set => listAddress = value; }
        public int HeightMin { get => heightMin; set => heightMin = value; }
        public int WidthMax { get => widthMax; set => widthMax = value; }
        public int WidthMin { get => widthMin; set => widthMin = value; }
        public int HeightMax { get => heightMax; set => heightMax = value; }
        public string TxbAtcAddressBeginText { get => txbAtcAddressBeginText; set => txbAtcAddressBeginText = value; }
        public string TxbAtcAddressEndText { get => txbAtcAddressEndText; set => txbAtcAddressEndText = value; }
        public string TxblDistanceText { get => txblDistanceText; set => txblDistanceText = value; }
        public string TxblTimeWalkText { get => txblTimeWalkText; set => txblTimeWalkText = value; }
        public string TxblTimeCarText { get => txblTimeCarText; set => txblTimeCarText = value; }



        public ICommand LoadedCommand
        {
            get
            {
                return loadedCommand = new RelayCommand<object[]>(p =>
                {
                    Grid GridRoot = p[0] as Grid;
                    Image ImageRoot = p[1] as Image;
                    GridRoot.Height = HeightMax;
                    GridRoot.Width = WidthMax;
                    string path = @"C:\Users\Admin\source\repos\MapWpf\MapWpfMVVM\Data\ImageData.jpg";
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(path);
                    bitmap.EndInit();
                    bitmap.Freeze();
                    ImageRoot.Source = bitmap;
                    GCCLoad();
                });
            }
        }

        public ICommand PreviewMouseLeftButtonUpScrollRootCommand
        {
            get
            {
                return previewMouseLeftButtonUpScrollRootCommand = new RelayCommand<ScrollViewer>(p =>
                {
                    p.ReleaseMouseCapture();
                });
            }
        }

        public ICommand PreviewMouseLeftButtonDownScrollRootCommand
        {
            get
            {
                return previewMouseLeftButtonDownScrollRootCommand = new RelayCommand<Tuple<object, object>>(p =>
                  {
                      var ScrollRoot = (p.Item2 as object[])[0] as ScrollViewer;
                      var e = p.Item1 as MouseButtonEventArgs;
                      scrollMousePoint = e.GetPosition(ScrollRoot);
                      hOff = ScrollRoot.HorizontalOffset;
                      vOff = ScrollRoot.VerticalOffset;
                      //this.Cursor = Cursors.Hand;
                      ScrollRoot.CaptureMouse();
                  });
            }
        }

        public ICommand PreviewMouseMoveScrollRootCommand
        {
            get
            {
                return previewMouseMoveScrollRootCommand = new RelayCommand<Tuple<object, object>>(p =>
                   {
                       var ScrollRoot = (p.Item2) as ScrollViewer;
                       var e = p.Item1 as MouseEventArgs;
                       if (ScrollRoot.IsMouseCaptured)
                       {
                           ScrollRoot.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - e.GetPosition(ScrollRoot).Y));
                           ScrollRoot.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(ScrollRoot).X));
                       }
                   });
            }
        }

        public ICommand PreviewMouseWheelScrollRootCommand
        {
            get
            {
                return previewMouseWheelScrollRootCommand = new RelayCommand<Tuple<object, object>>(p =>
                   {
                       var GridRoot = (p.Item2) as Grid;
                       var e = p.Item1 as MouseWheelEventArgs;
                       e.Handled = true;
                       var position = e.MouseDevice.GetPosition(GridRoot);
                       if (e.Delta > 0)
                       {
                           if (GridRoot.Height <= HeightMax || GridRoot.Width <= WidthMax)
                           {
                               var gs = GridRoot.RenderTransform;
                               GridRoot.Height = GridRoot.Height * zoom;
                               GridRoot.Width = GridRoot.Width * zoom;
                               RelayoutPath(GridRoot, zoom, (double)thickness + 2);
                           }
                       }
                       else
                       {
                           if (GridRoot.Height >= HeightMin || GridRoot.Width >= WidthMin)
                           {
                               GridRoot.Height = GridRoot.Height / zoom;
                               GridRoot.Width = GridRoot.Width / zoom;
                               RelayoutPath(GridRoot, Math.Pow(zoom, -1), (double)thickness / 10);
                           }

                       }


                       GCCLoad();
                   });
            }
        }

        public ICommand SizeChangedGridRoot
        {
            get
            {
                return sizeChangedGridRoot = new RelayCommand<Tuple<object, object>>(p =>
                   {
                       var e = p.Item1 as SizeChangedEventArgs;
                       var ImageRoot = (p.Item2 as object[])[0] as Image;
                       var scale = (p.Item2 as object[])[1] as ScaleTransform;
                       scale.ScaleX = e.NewSize.Width / ImageRoot.Source.Width;
                       scale.ScaleY = e.NewSize.Height / ImageRoot.Source.Height;
                   });
            }
        }

        public ICommand ClickBtnFind
        {
            get
            {
                return clickBtnFind = new RelayCommand<Grid>(async GridRoot =>
                {
                    GridRoot.Children.Remove(GridRoot.Children.OfType<Path>().SingleOrDefault());
                    var begin = TxbAtcAddressBeginText.ToLower();
                    var end = TxbAtcAddressEndText.ToLower();
                    if (begin.Trim().Length == 0 || end.Trim().Length == 0)
                    {
                        MessageBox.Show("Address Empty");
                        return;
                    }
                    int b = -1;

                    var task1 = Task.Factory.StartNew(() =>
                     {
                         Parallel.For(0, ListAddress.Count, p =>
                         {
                             if (ListAddress[p].ToLower().Contains(begin) && b == -1)
                             {
                                 b = p;
                             }
                         });
                     });
                    int en = -1;
                    var task2 = Task.Factory.StartNew(() =>
                    {
                        Parallel.For(0, ListAddress.Count, p =>
                        {
                            if (ListAddress[p].ToLower().Contains(end) && en == -1)
                            {
                                en = p;                                
                            }
                        });
                    });
                    await Task.WhenAll(new Task[] {task1,task2 });
                    var tops = GetGraph.Disjkstra(b, en);
                    TxblDistanceText = tops.Item1.ToString() + " m";
                    TxblTimeWalkText = (Math.Round((tops.Item1 / (5000 / 3600)) / 60)).ToString() + " minutes (5km/h)";
                    TxblTimeCarText = (Math.Round((tops.Item1 / 16.6667) / 60)).ToString() + " minutes (60km/h)";


                    PathGeometry myPathGeometry = new PathGeometry();

                    // Display the PathGeometry. 
                    var rate = GridRoot.ActualHeight / HeightMax;
                    for (int i = 0; i < tops.Item2.Length; i++)
                    {
                        if (i == tops.Item2.Length - 1)
                        {
                            break;
                        }
                        PathFigure myPathFigure = new PathFigure
                        {
                            StartPoint = new Point(GetGraph.TopGraphs[tops.Item2[i]].GetPoints.X * rate, GetGraph.TopGraphs[tops.Item2[i]].GetPoints.Y * rate)
                        };
                        myPathFigure.Segments.Add(new LineSegment(
                                new Point(GetGraph.TopGraphs[tops.Item2[i + 1]].GetPoints.X * rate, GetGraph.TopGraphs[tops.Item2[i + 1]].GetPoints.Y * rate), true));
                        myPathGeometry.Figures.Add(myPathFigure);
                    }

                    Path myPath = new Path
                    {
                        Name = "Path",
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        Data = myPathGeometry
                    };
                    GridRoot.Children.Add(myPath);
                });
            }
        }

        public ICommand SelectionChangedTxbAtcAddressBeginCommand
        {
            get
            {
                return selectionChangedTxbAtcAddressBeginCommand=new RelayCommand<object[]>(para=> 
                {
                    var GridRoot = para[0] as Grid;
                    var sender = para[1];
                    GridRoot.Children.Remove(GridRoot.Children.OfType<Ellipse>().Where(p => p.Name.Equals("Begin")).SingleOrDefault());
                    if ((sender as AutoCompleteBox).SelectedItem != null)
                    {

                        var rate = GridRoot.ActualHeight / HeightMax;
                        var begin = GetGraph.TopGraphs.ToArray().OfType<TopGraph>().Where(p => p.Name.Equals((sender as AutoCompleteBox).SelectedItem.ToString())).SingleOrDefault();
                        var cricle = DrawMark(10, begin.GetPoints.X * rate, begin.GetPoints.Y * rate);
                        cricle.Name = "Begin";
                        GridRoot.Children.Add(cricle);
                    }
                });
            }
        }
        public ICommand SelectionChangedTxbAtcAddressEndCommand
        {
            get
            {
                return selectionChangedTxbAtcAddressEndCommand = new RelayCommand<object[]>(para=>
                {
                    var GridRoot = para[0] as Grid;
                    var sender = para[1];
                    GridRoot.Children.Remove(GridRoot.Children.OfType<Ellipse>().Where(p => p.Name.Equals("End")).SingleOrDefault());
                    if ((sender as AutoCompleteBox).SelectedItem != null)
                    {
                        var rate = GridRoot.ActualHeight / HeightMax;
                        var end = GetGraph.TopGraphs.ToArray().OfType<TopGraph>().Where(p => p.Name.Equals((sender as AutoCompleteBox).SelectedItem.ToString())).SingleOrDefault();
                        var cricle = DrawMark(10, end.GetPoints.X * rate, end.GetPoints.Y * rate);
                        cricle.Name = "End";
                        GridRoot.Children.Add(cricle);
                    }
                });
            }
        }
        private void RelayoutPath(Grid grid, double zoom, double thickness)
        {
            var myPath = grid.Children.OfType<Path>().SingleOrDefault();

            if (myPath == null)
            {
                return;
            }
            myPath.StrokeThickness = thickness;
            var Data = (myPath.Data as PathGeometry).Figures;
            foreach (PathFigure item in Data)
            {
                item.StartPoint = new Point(item.StartPoint.X * zoom, item.StartPoint.Y * zoom);
                (item.Segments[0] as LineSegment).Point = new Point((item.Segments[0] as LineSegment).Point.X * zoom, (item.Segments[0] as LineSegment).Point.Y * zoom);

            }


        }

        #region Draw Line and Mark
        /// <summary>
        /// Return a circle 
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Ellipse DrawMark(double radius, double x, double y)
        {
            var ellipse = new Ellipse
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(x, y, 0, 0),
                Fill = new SolidColorBrush(Colors.Blue),
                Stroke = new SolidColorBrush(Colors.Black),
                Width = radius,
                Height = radius
            };
            return ellipse;
        }

        /// <summary>
        /// Draw a line
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        private Line DrawLine(double x1, double x2, double y1, double y2, double thickness)
        {
            var line = new Line
            {
                X1 = x1,
                X2 = x2,
                Y1 = y1,
                Y2 = y2,
                StrokeThickness = thickness,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Stroke = new SolidColorBrush(Colors.Red),
                Fill = new SolidColorBrush(Colors.Red)
            };
            return line;
        }

        #endregion

        void GCCLoad()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IGraph graph)
        {
            GetGraph = graph;
            GetData();
            ListAddress = new ObservableCollection<string>();
            GetGraph.TopGraphs.ForEach(p => { ListAddress.Add(p.Name); });
        }

        void GetData()
        {
            GetGraph.ConvertTextToList();
        }



        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}