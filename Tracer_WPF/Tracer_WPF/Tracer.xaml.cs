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
using System.Windows.Shapes;

namespace Tracer_WPF
{
    /// <summary>
    /// Логика взаимодействия для Tracer.xaml
    /// </summary>
    public partial class Tracer : Window
    {
        private static GridLength menuLength;
        private static bool menuIsOpen = false;
        public Tracer()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void gearImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!menuIsOpen)
            {
                ShowHideMenu("sbShowMenu", MenuLeft);
                menuIsOpen = true;
            }
            else
            {
                ShowHideMenu("sbHideMenu", MenuLeft);
                menuIsOpen = false;
            }
        }

        private void ShowHideMenu(string storyboard, StackPanel stackPanel)
        {
            Storyboard sb = Resources[storyboard] as Storyboard;
            sb.Begin(stackPanel);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                MouseButtonEventArgs mouseEvents = null;
                gearImage_MouseUp(sender, mouseEvents);
            }
        }


    }
}
