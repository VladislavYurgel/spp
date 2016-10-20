using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tracer_WPF.ViewModel;

namespace Tracer_WPF.Model
{
    public class TabItemModel : ViewModelBase
    {
        protected string header;
        public string Header
        {
            get { return header; }
            set { SetProperty(ref header, value); }
        }

        private ObservableCollection<Node> nodes = new ObservableCollection<Node>();
        public ObservableCollection<Node> Nodes
        {
            get { return nodes; }
            set { SetProperty(ref nodes, value); }
        }

        public TabItemModel()
        {
            
        }
    }
}
