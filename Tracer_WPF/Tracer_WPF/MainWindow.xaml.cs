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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tracer_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static GridLength menuLength;
        private static bool menuIsOpen = false;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void gearImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            /*if (!menuIsOpen)
            {
                menuLength = MenuColumn.Width;
                MenuColumn.Width = new GridLength(60, GridUnitType.Pixel);
                menuIsOpen = true;
            }
            else
            {
                MenuColumn.Width = menuLength;
                menuIsOpen = false;
            }*/
            if (!menuIsOpen)
            {
                ShowHideMenu("sbHideMenu", MenuColumn);
            }
            else
            {
                ShowHideMenu("sbShowMenu", MenuColumn);
            }
        }

        private void ShowHideMenu(string Storyboard, ColumnDefinition grid, Image imageHide = null, Image imageShow = null)
        {
            Storyboard sb = Resources[Storyboard] as Storyboard;
            sb.Begin(grid);

            if (Storyboard.Contains("Show"))
            {

            }
        }
    }
}
