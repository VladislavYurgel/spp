using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using Tracer_WPF.Model;
using Tracer_WPF.ViewModel;

namespace Tracer_WPF.Utils
{
    static class SystemTracer
    {
        public static object DialogWindow { get; private set; }

        public static string OpenFile()
        {
            Stream myStream = null;
            string fileName = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "XML files (*.xml)|*.xml";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
                fileName = openFileDialog.FileName;

            return fileName;
        }

        public static ObservableCollection<Node> Parse(string fileName)
        {
            int currentId = 1;
            int currentThread = 0;
            int lastThreadNodeId = 1;
            List<int> parentsId = new List<int>();

            string XMLText = File.ReadAllText(fileName);
            ObservableCollection<Node> tempNodes = new ObservableCollection<Node>();

            XmlDocument xmlDocument = new XmlDocument();
            Node tempNode = new Node();
            string xmlNodeName;

            xmlDocument.LoadXml(XMLText);

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement)
            {
                xmlNodeName = xmlNode.Name.ToUpper();
                if (xmlNodeName == "THREAD")
                {
                    currentThread = int.Parse(xmlNode.Attributes["id"].Value);
                    lastThreadNodeId = currentId;
                    tempNodes.Add(
                        new Node
                        {
                            MethodName = "Thread",
                            NodeId = currentId++,
                            ParentId = 0,
                            ThreadId = currentThread,
                            Time = int.Parse(xmlNode.Attributes["time"].Value.ToString())
                        }
                    );
                }

                for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
                {
                    tempNodes.Add(
                        new Node {
                            MethodName = xmlNode.ChildNodes[i].Attributes["name"].Value.ToString(),
                            NodeId = currentId,
                            Package = xmlNode.ChildNodes[i].Attributes["package"].Value.ToString(),
                            ParamsCount = int.Parse(xmlNode.ChildNodes[i].Attributes["paramsCount"].Value.ToString()),
                            ParentId = lastThreadNodeId,
                            Time = int.Parse(xmlNode.ChildNodes[i].Attributes["time"].Value.ToString()),
                            ThreadId = currentThread
                        }
                    );

                    parentsId.Add(currentId);
                    currentId++;

                    ParseChild(ref tempNodes, xmlNode.ChildNodes[i], ref currentId, ref parentsId, currentThread);
                }
            }

            return tempNodes;
        }

        public static void ParseChild(ref ObservableCollection<Node> nodeCollection, XmlNode xmlNode, ref int currentId, ref List<int> parentsId,
            int currentThread)
        {
            Node tempNode;
            for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
            {
                tempNode =
                    new Node
                    {
                        MethodName = xmlNode.ChildNodes[i].Attributes["name"].Value.ToString(),
                        NodeId = currentId,
                        Package = xmlNode.ChildNodes[i].Attributes["package"].Value.ToString(),
                        ParamsCount = int.Parse(xmlNode.ChildNodes[i].Attributes["paramsCount"].Value.ToString()),
                        ParentId = parentsId.Last(),
                        Time = int.Parse(xmlNode.ChildNodes[i].Attributes["time"].Value.ToString()),
                        ThreadId = currentThread
                    };

                if (xmlNode.ChildNodes[i].HasChildNodes)
                {
                    parentsId.Add(currentId);
                    currentId++;
                    nodeCollection.Add(tempNode);
                    ParseChild(ref nodeCollection, xmlNode.ChildNodes[i], ref currentId, ref parentsId, currentThread);
                }
                else
                {
                    currentId++;
                    parentsId.RemoveAt(parentsId.Count - 1);
                    nodeCollection.Add(tempNode);
                }
            }
        }

        public static ObservableCollection<Node> BuildNodes(ObservableCollection<Node> oldNodes)
        {
            ObservableCollection<Node> nodes = new ObservableCollection<Node>();
            
            foreach (Node node in oldNodes)
            {
                if (node.ParentId != 0)
                    SetNode(ref nodes, node.ParentId, node);
                else
                    nodes.Add(node);
            }

            return nodes;
        }

        public static void SetNode(ref ObservableCollection<Node> nodes, int neededId, Node insertedNode, bool newNode = true)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].NodeId == neededId)
                {
                    if (newNode)
                        nodes[i].Nodes.Add(insertedNode);
                    else
                        nodes[i] = insertedNode;

                    return;
                }
                if (nodes[i].Nodes.Count > 0)
                    SetNode(ref nodes[i].nodes, neededId, insertedNode);
            }
        }

        public static bool SaveXML()
        {
            TabItemModel selectedTab = MainWindow.tabControl.SelectedContent as TabItemModel;
            ObservableCollection<Node> nodes = (selectedTab != null) ? selectedTab.Nodes : null;

            if (nodes == null)
            {
                MessageBox.Show("Save error! You tried to save empty treeview", "XML save error");
                return false;
            }

            string fileName = CommandsViewModel.files[MainWindow.tabControl.SelectedIndex];

            XDocument xmlDoc = new XDocument();
            XElement xmlRoot = new XElement("root");
            XElement xmlThread;
            XAttribute attr;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].ParentId == 0)
                {
                    xmlThread = new XElement("thread");

                    attr = new XAttribute("id", nodes[i].ThreadId);
                    xmlThread.Add(attr);
                    attr = new XAttribute("time", nodes[i].Time);
                    xmlThread.Add(attr);

                    addXmlNode(nodes[i].Nodes, ref xmlThread);
                    xmlRoot.Add(xmlThread);
                }
            }
            xmlDoc.Add(xmlRoot);
            xmlDoc.Save(fileName);

            return true;
        }

        public static bool SaveAsXML()
        {
            TabItemModel selectedTab = MainWindow.tabControl.SelectedContent as TabItemModel;
            ObservableCollection<Node> nodes = (selectedTab != null) ? selectedTab.Nodes : null;

            if (nodes == null)
            {
                MessageBox.Show("Save error! You tried to save empty treeview", "XML save error");
                return false;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;

            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                if (Path.GetExtension(fileName) != ".xml")
                {
                    MessageBox.Show("Save error! This is not XML file");
                    return false;
                }

                XDocument xmlDoc = new XDocument();
                XElement xmlRoot = new XElement("root");
                XElement xmlThread;
                XAttribute attr;
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].ParentId == 0)
                    {
                        xmlThread = new XElement("thread");

                        attr = new XAttribute("id", nodes[i].ThreadId);
                        xmlThread.Add(attr);
                        attr = new XAttribute("time", nodes[i].Time);
                        xmlThread.Add(attr);

                        addXmlNode(nodes[i].Nodes, ref xmlThread);
                        xmlRoot.Add(xmlThread);
                    }
                }
                xmlDoc.Add(xmlRoot);
                xmlDoc.Save(fileName);

                TabItemViewModel.Tabs[MainWindow.tabControl.SelectedIndex].Header = Path.GetFileName(fileName);
                MainWindow.ChangeTabItemState(MainWindow.tabControl.SelectedIndex);
                CommandsViewModel.files[MainWindow.tabControl.SelectedIndex] = fileName;

                return true;
            }

            return false;
        }
        
        private static void addXmlNode(ObservableCollection<Node> nodes, ref XElement thread)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                XElement xmlMethod = new XElement("method");
                XAttribute attr;

                attr = new XAttribute("name", nodes[i].MethodName);
                xmlMethod.Add(attr);
                attr = new XAttribute("paramsCount", nodes[i].ParamsCount);
                xmlMethod.Add(attr);
                attr = new XAttribute("package", nodes[i].Package);
                xmlMethod.Add(attr);
                attr = new XAttribute("time", nodes[i].Time);
                xmlMethod.Add(attr);

                addXmlNode(nodes[i].Nodes, ref xmlMethod);

                thread.Add(xmlMethod);
            }
        }
    }
}
