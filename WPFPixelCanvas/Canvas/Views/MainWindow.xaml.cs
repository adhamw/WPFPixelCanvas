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
using System.Windows.Shapes;
using WPFPixelCanvas.Canvas.ViewModels;

namespace WPFPixelCanvas.Canvas.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            //            dcMain = new ViewModel((int)imgtutta.Width, (int)imgtutta.Height);
            ViewModel dcMain = new(400, 300);
            this.DataContext = dcMain;

        }
    }
}
