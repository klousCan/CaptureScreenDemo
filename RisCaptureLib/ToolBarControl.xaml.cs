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

namespace RisCaptureLib
{
    /// <summary>
    /// ToolBarControl.xaml 的交互逻辑
    /// </summary>
    public partial class ToolBarControl : UserControl
    {
        //确定事件
        public event Action OnOK;
        //取消事件
        public event Action OnCancel;
        public ToolBarControl()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            OnCancel?.Invoke();
        }

        private void buttonComplete_Click(object sender, RoutedEventArgs e)
        {
            OnOK?.Invoke();
        }

    }
}
