using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point scrollMousePoint = new Point();
        double hOff = 1;
        double vOff = 1;
        public MainWindow()
        {
            InitializeComponent();
            GetData();
            Loading();
        }

        void GetData()
        {
           
        }

        void Loading()
        {
            GridRoot.Height = 8033;
            GridRoot.Width = 14705;
            string path = @"C:\Users\Admin\source\repos\MapWpf\MapWpf\Data\ImageData.jpg";
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();
            bitmap.Freeze();

            ImageRoot.Source = bitmap;
            GCCLoad();
        }

        void GCCLoad()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void BtnClick_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ScrollRoot_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScrollRoot.ReleaseMouseCapture();
        }

        private void ScrollRoot_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            scrollMousePoint = e.GetPosition(ScrollRoot);
            hOff = ScrollRoot.HorizontalOffset;
            vOff = ScrollRoot.VerticalOffset;
            //this.Cursor = Cursors.Hand;
            ScrollRoot.CaptureMouse();
        }

        private void ScrollRoot_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (ScrollRoot.IsMouseCaptured)
            {
                ScrollRoot.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - e.GetPosition(ScrollRoot).Y));
                ScrollRoot.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(ScrollRoot).X));
            }
        }

        

        private void ScrollRoot_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            
            
            //var position = e.MouseDevice.GetPosition(GridRoot);
            if (e.Delta > 0)
            {
                GridRoot.Height = GridRoot.Height * 1.1;
                GridRoot.Width = GridRoot.Width * 1.1;
                //position.X = position.X * 1.1;
                //position.Y = position.Y * 1.1;
                
                //position.X = position.X * 2;
                //position.Y = position.Y * 2;
                //ScrollRoot.ScrollToVerticalOffset(ScrollRoot.VerticalOffset*(1+1.1));
                //ScrollRoot.ScrollToHorizontalOffset(ScrollRoot.HorizontalOffset*(1+1.1));
            }
            else
            {
                if (GridRoot.Height>=750||GridRoot.Width>=1388)
                {

                    GridRoot.Height = GridRoot.Height / 1.1;
                    GridRoot.Width = GridRoot.Width / 1.1;
                    //position.X = position.X / 1.1;
                    //position.Y = position.Y / 1.1;
                }
                else
                {
                    return;
                }
            }         
            
            GCCLoad();
            //this.Cursor=new Cursor(Cursor.)
            /*
            var position = e.MouseDevice.GetPosition(ImageRoot);

            var renderTransformValue = ImageRoot.RenderTransform.Value;
            if (e.Delta > 0)
                renderTransformValue.ScaleAtPrepend(1.1, 1.1, position.X, position.Y);
            else
                renderTransformValue.ScaleAtPrepend(1 / 1.1, 1 / 1.1, position.X, position.Y);

            ImageRoot.RenderTransform = new MatrixTransform(renderTransformValue);
            GCCLoad();*/
        }

        /// <summary>
        /// Return a circle 
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Ellipse DrawMark(double radius,double x,double y)
        {
            var ellipse = new Ellipse
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(x, y, 0, 0),
                Fill = new SolidColorBrush(Colors.Red),
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
        private Line DrawLine(double x1,double x2,double y1,double y2,double thickness)
        {
            var line = new Line
            {
                X1=x1,
                X2=x2,
                Y1=y1,
                Y2=y2,
                StrokeThickness=thickness,
                HorizontalAlignment=HorizontalAlignment.Left,
                VerticalAlignment=VerticalAlignment.Top,
                Stroke = new SolidColorBrush(Colors.Red),
                Fill =new SolidColorBrush(Colors.Red)
            };
            return line;
        }
    }
}
