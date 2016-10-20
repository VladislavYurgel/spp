using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer_WPF.Model;

namespace Tracer_WPF.ViewModel
{
    public class NodeViewModel : ViewModelBase
    {
        static private ObservableCollection<Node> nodes = new ObservableCollection<Node>();
        public ObservableCollection<Node> Nodes
        {
            get { return nodes; }
            set { SetProperty(ref nodes, value); }
        }

        public NodeViewModel()
        {
            Nodes.Add(new Node { Name = "First node" });
        }
    }
}
