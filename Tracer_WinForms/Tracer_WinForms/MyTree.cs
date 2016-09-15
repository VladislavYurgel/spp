using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tracer_WinForms
{
    class MyTree
    {
        protected TreeView treeView;
        public void GenerateTree(List<Trace> traceList, TreeView trView)
        {
            treeView = trView;
            int currentThread = traceList[0].threadId;
            List<int> parents = new List<int>();
            Trace tempTrace;

            TreeNode method = new TreeNode(traceList[0].methodName + " " + "(Params=" + traceList[0].paramsCount + ", Package=" + traceList[0].methodPackage + ", time=" + traceList[0].time + ")");
            TreeNode thread = new TreeNode("Thread (id=" + currentThread + ", time=" + double.Parse(calculateTime(currentThread, traceList).ToString()) + ")");
            treeView.Nodes.Add(thread);
            TreeNode tempNode;

            foreach (Trace trace in traceList)
            {
                if (currentThread != trace.threadId)
                {
                    currentThread = trace.threadId;
                    thread = new TreeNode("Thread (id=" + currentThread + ", time=" + double.Parse(calculateTime(currentThread, traceList).ToString()) + ")");
                    treeView.Nodes.Add(thread);
                }
                if (trace.parentId > 0)
                {
                    tempNode = new TreeNode(trace.methodName + " " + "(Params=" + trace.paramsCount + ", Package=" + trace.methodPackage + ", time=" + trace.time + ")");
                    if (parents.Contains(trace.parentId))
                    {
                        method = thread.Nodes[trace.traceId - trace.parentId - 1];
                        method.Nodes.Add(tempNode);
                        method = tempNode;
                    }
                    else
                    {
                        method.Nodes.Add(tempNode);
                        method = tempNode;
                    }
                   parents.Add(trace.parentId);
                }
                else
                {
                    if (method.GetNodeCount(true) > 0)
                        thread.Nodes.Add(method);
                    method = new TreeNode(trace.methodName + " " + "(Params=" + trace.paramsCount + ", Package=" + trace.methodPackage + ", time=" + trace.time + ")");
                    thread.Nodes.Add(method);
                }
            }
        }

        public static double calculateTime(int threadId, List<Trace> traceList)
        {
            double time = 0;

            foreach (Trace trace in traceList)
            {
                if ((threadId == trace.threadId) && !(trace.parentId > 0))
                    time += trace.time;
            }

            return time;
        }

        public void changeNode(TreeNodeMouseClickEventArgs e, ref TabControl tabControl)
        {
            TreeNode treeNode = e.Node;

            // RegExp for check, if it's thread, then don't show ChangeNode form
            string threadExp = @"(Thread)";
            if (!Regex.IsMatch(treeNode.Text, threadExp))
                new ChangeNode(ref treeNode, ref tabControl).Show();
            else
                Form1.changeNodeWindofIsOpen = false;
        }
    }
}
