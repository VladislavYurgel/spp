using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer_WPF.ViewModel;

namespace Tracer_WPF.Model
{
    public class Node : ViewModelBase
    {
        public ObservableCollection<Node> nodes = new ObservableCollection<Node>();
        public ObservableCollection<Node> Nodes
        {
            get { return nodes; }
            set { SetProperty(ref nodes, value); }
        }

        private string name;
        public string Name
        {
            get
            {
                if (ParentId == 0)
                    return "Thread" + " " + "(id=" + ThreadId + ", time=" + Time + ")";
                else
                    return MethodName + " " + "(Params=" + ParamsCount + ", Package=" + Package + ", time=" + Time + ")";
            }
            set { SetProperty(ref name, value); }
        }

        private int nodeId;
        public int NodeId
        {
            get { return nodeId; }
            set { SetProperty(ref nodeId, value); }
        }

        private int parentId;
        public int ParentId
        {
            get { return parentId; }
            set { SetProperty(ref parentId, value); }
        }

        private string package;
        public string Package
        {
            get { return package; }
            set { SetProperty(ref package, value); SetProperty(ref name, Name, "Name"); }
        }

        private int paramsCount;
        public int ParamsCount
        {
            get { return paramsCount; }
            set { SetProperty(ref paramsCount, value); SetProperty(ref name, Name, "Name"); }
        }

        private int time;
        public int Time
        {
            get { return time; }
            set { SetProperty(ref time, value); SetProperty(ref name, Name, "Name");}
        }

        private int threadId;
        public int ThreadId
        {
            get { return threadId; }
            set { SetProperty(ref threadId, value); }
        }

        private string methodName;
        public string MethodName
        {
            get { return methodName; }
            set { SetProperty(ref methodName, value); SetProperty(ref name, Name, "Name"); }
        }
    }
}
