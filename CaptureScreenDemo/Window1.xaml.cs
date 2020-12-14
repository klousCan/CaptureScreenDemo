using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CaptureScreenDemo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1
    {
        private readonly RisCaptureLib.ScreenCaputre screenCaputre = new RisCaptureLib.ScreenCaputre();
        private Size? lastSize;

        public Window1()
        {
            InitializeComponent();

            screenCaputre.ScreenCaputred += OnScreenCaputred;
            screenCaputre.ScreenCaputreCancelled += OnScreenCaputreCancelled;
        }

        private void OnScreenCaputreCancelled(object sender, System.EventArgs e)
        {
            Show();
            Focus();
        }

        private void OnScreenCaputred(object sender, RisCaptureLib.ScreenCaputredEventArgs e)
        {
            //set last size
            lastSize = new Size(e.Bmp.Width, e.Bmp.Height);


            Show();

            //test
            var bmp = e.Bmp;
            var win = new Window {SizeToContent = SizeToContent.WidthAndHeight, ResizeMode= ResizeMode.NoResize};

            var canvas = new Canvas {Width = bmp.Width, Height = bmp.Height, Background = new ImageBrush(bmp)};

            win.Content = canvas;
            win.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            Thread.Sleep(300);
            screenCaputre.StartCaputre(30, lastSize);
        }
    }
}
