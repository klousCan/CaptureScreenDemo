using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RisCaptureLib
{
    internal class MaskWindow : Window
    {
        private MaskCanvas innerCanvas;
        private Bitmap screenSnapshot;
        private Timer timeOutTimmer;
        private readonly ScreenCaputre screenCaputreOwner;
        //截图显示按键
        private ToolBarControl toolBarContrl = null;
        public MaskWindow(ScreenCaputre screenCaputreOwner)
        {
            this.screenCaputreOwner = screenCaputreOwner;
            Ini();
        }

        private void Ini()
        {

            //ini normal properties
            //Topmost = true;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;

            //set bounds to cover all screens
            var rect = SystemInformation.VirtualScreen;
            Left = rect.X;
            Top = rect.Y;
            Width = rect.Width;
            Height = rect.Height;

            //set background 
            screenSnapshot = HelperMethods.GetScreenSnapshot();
            if (screenSnapshot != null)
            {
                var bmp = screenSnapshot.ToBitmapSource();
                bmp.Freeze();
                Background = new ImageBrush(bmp);
            }

            //ini canvas
            innerCanvas = new MaskCanvas
            {
                MaskWindowOwner = this
            };
            Content = innerCanvas;

        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.RightButton == MouseButtonState.Pressed && e.ClickCount >= 2)
            {
                CancelCaputre();
            }
            ToolBarHidden();
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (timeOutTimmer != null && timeOutTimmer.Enabled)
            {
                timeOutTimmer.Stop();
                timeOutTimmer.Start();
            }
            ToolBarMove();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            CreatToolBar(e.GetPosition(innerCanvas));
        }
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
            {
                CancelCaputre();
            }
        }

        private void CancelCaputre()
        {
            Close();
            screenCaputreOwner.OnScreenCaputreCancelled(null);
        }

        internal void OnShowMaskFinished(Rect maskRegion)
        {

        }

        internal void ClipSnapshot(Rect clipRegion)
        {
            BitmapSource caputredBmp = CopyFromScreenSnapshot(clipRegion);

            if (caputredBmp != null)
            {
                screenCaputreOwner.OnScreenCaputred(null, caputredBmp);
            }

            //close mask window
            Close();
        }


        internal BitmapSource CopyFromScreenSnapshot(Rect region)
        {
            var sourceRect = region.ToRectangle();
            var destRect = new Rectangle(0, 0, sourceRect.Width, sourceRect.Height);

            if (screenSnapshot != null)
            {
                var bitmap = new Bitmap(sourceRect.Width, sourceRect.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawImage(screenSnapshot, destRect, sourceRect, GraphicsUnit.Pixel);
                }

                return bitmap.ToBitmapSource();
            }

            return null;
        }

        public void Show(int timeOutSecond, System.Windows.Size? defaultSize)
        {
            if (timeOutSecond > 0)
            {
                if (timeOutTimmer == null)
                {
                    timeOutTimmer = new Timer();
                    timeOutTimmer.Tick += OnTimeOutTimmerTick;
                }
                timeOutTimmer.Interval = timeOutSecond * 1000;
                timeOutTimmer.Start();
            }

            if (innerCanvas != null)
            {
                innerCanvas.DefaultSize = defaultSize;
            }

            Show();
            Focus();

        }

        private void OnTimeOutTimmerTick(object sender, System.EventArgs e)
        {
            timeOutTimmer.Stop();
            CancelCaputre();
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            toolBarContrl.Visibility = Visibility.Hidden;
        }

        private void CreatToolBar(System.Windows.Point location)
        {
            if (toolBarContrl == null)
            {
                toolBarContrl = new ToolBarControl();
                innerCanvas.Children.Add(toolBarContrl);
                Canvas.SetLeft(toolBarContrl, location.X - toolBarContrl.ActualWidth);
                Canvas.SetTop(toolBarContrl, location.Y - 1);
                toolBarContrl.OnOK += OKAction;
                toolBarContrl.OnCancel += CancelAction;
            }
            toolBarContrl.Visibility = Visibility.Visible;
        }
        private void ToolBarHidden()
        {
            if (toolBarContrl != null)
            {
                toolBarContrl.Visibility = Visibility.Hidden;
            }
        }

        private void ToolBarMove()
        {
            Rect temRect = innerCanvas.GetSelectionRegion();
            if (toolBarContrl != null)
            {
                Canvas.SetLeft(toolBarContrl, temRect.X + temRect.Width - toolBarContrl.ActualWidth);
                Canvas.SetTop(toolBarContrl, temRect.Y + temRect.Height);
            }
        }
        public void OKAction()
        {
            innerCanvas.finishAction();
        }

        public void CancelAction()
        {
            CancelCaputre();
        }
    }
}
