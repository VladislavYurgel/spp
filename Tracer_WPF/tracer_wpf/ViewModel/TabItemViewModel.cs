using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tracer_WPF.Model;

namespace Tracer_WPF.ViewModel
{
    public class TabItemViewModel : TabItemModel, INotifyPropertyChanged
    {
        static private ObservableCollection<TabItemModel> tabs = new ObservableCollection<TabItemModel>();
        static public ObservableCollection<TabItemModel> Tabs
        {
            get { return tabs; }
            set { if (tabs != value) tabs = value; }
        }

        public TabItemViewModel()
        {
            
        }
    }
}
