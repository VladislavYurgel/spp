using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Tracer_WinForms
{
    class XMLParser
    {
        protected int currentId = 0;
        protected int currentThread = 0;
        protected List<int> parentsId = new List<int>();
        protected List<Trace> traceList = new List<Trace>();
        protected int parentsCount = 0;

        public List<Trace> Parse(string xmlText)
        {
            XmlDocument xmlDocument = new XmlDocument();
            Trace tempTrace = new Trace();
            string xmlNodeName;

            xmlDocument.LoadXml(xmlText);

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement)
            {
                xmlNodeName = xmlNode.Name.ToUpper();
                if (xmlNodeName == "THREAD")
                    currentThread = int.Parse(xmlNode.Attributes["id"].Value);
                for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
                {
                    tempTrace = new Trace();
                    tempTrace.traceId = currentId;
                    tempTrace.threadId = currentThread;
                    tempTrace.methodName = xmlNode.ChildNodes[i].Attributes["name"].Value.ToString();
                    tempTrace.methodPackage = xmlNode.ChildNodes[i].Attributes["package"].Value.ToString();
                    tempTrace.time = double.Parse(xmlNode.ChildNodes[i].Attributes["time"].Value.ToString().Replace("ms", ""));
                    tempTrace.paramsCount = int.Parse(xmlNode.ChildNodes[i].Attributes["paramsCount"].Value);
                    tempTrace.parentId = 0;

                    // Add to trace list
                    traceList.Add(tempTrace);

                    parentsId.Add(currentId);
                    currentId++;

                    parseChild(xmlNode.ChildNodes[i]);
                }
            }

            return traceList;
        }

        public void Build(TreeView treeView, string fileName)
        {
            parentsCount = 0;
            List<Trace> newTrace = new List<Trace>();
            List<int> parents = new List<int>();
            newTrace = TreeToList(treeView);
            int j = 0;
            XDocument xmlDocument = new XDocument();
            XElement xmlThread = new XElement("thread");
            XElement xmlMethod = new XElement("method");
            XElement root = new XElement("root");
            int currentThread = newTrace[0].threadId;
            XAttribute xmlThreadId = new XAttribute("id", currentThread);
            XAttribute xmlThreadTime = new XAttribute("time", MyTree.calculateTime(currentThread, newTrace) + "ms");
            xmlThread.Add(xmlThreadId);
            xmlThread.Add(xmlThreadTime);
            root.Add(xmlThread);

            foreach (Trace trace in newTrace)
            {
                if (currentThread != trace.threadId)
                {
                    currentThread = trace.threadId;
                    xmlThread = new XElement("thread");
                    xmlThreadId = new XAttribute("id", currentThread);
                    xmlThreadTime = new XAttribute("time", MyTree.calculateTime(currentThread, newTrace) + "ms");
                    xmlThread.Add(xmlThreadId);
                    xmlThread.Add(xmlThreadTime);
                    root.Add(xmlThread);
                    parents.Add(0);
                }
                XAttribute xmlMethodName = new XAttribute("name", trace.methodName);
                XAttribute xmlTime = new XAttribute("time", trace.time);
                XAttribute xmlPackage = new XAttribute("package", trace.methodPackage);
                XAttribute xmlParamsCount = new XAttribute("paramsCount", trace.paramsCount);
                if (trace.parentId > 0)
                {
                    XElement xmlChildMethod = new XElement("method");
                    xmlChildMethod.Add(xmlMethodName);
                    xmlChildMethod.Add(xmlTime);
                    xmlChildMethod.Add(xmlPackage);
                    xmlChildMethod.Add(xmlParamsCount);

                    if (parents.Contains(trace.parentId))
                    {
                        IEnumerable<XElement> elementsList =
                            from el in xmlThread.Elements()
                            select el;
                        j = 0;
                        foreach (XElement el in elementsList)
                        {
                            if (j == (trace.traceId - trace.parentId))
                            {
                                xmlMethod = el;
                                break;
                            }
                            j++;
                        }
                        xmlMethod.Add(xmlChildMethod);
                        xmlMethod = xmlChildMethod;
                    }
                    else
                    {
                        xmlMethod.Add(xmlChildMethod);
                        xmlMethod = xmlChildMethod;
                    }
                    parentsCount++;
                    parents.Add(trace.parentId);
                }
                else
                {
                    if (parents.Count > (parents.Count - parentsCount))
                    {
                        int firstParent = parents[parents.Count - parentsCount];
                        parents.Clear();
                        parents.Add(firstParent);
                    }
                    parentsCount = 0;
                    if (!xmlMethod.IsEmpty)
                        xmlThread.Add(xmlMethod);
                    xmlMethod = new XElement("method");
                    xmlMethod.Add(xmlMethodName);
                    xmlMethod.Add(xmlTime);
                    xmlMethod.Add(xmlPackage);
                    xmlMethod.Add(xmlParamsCount);
                    xmlThread.Add(xmlMethod);
                }
            }
            xmlDocument.Add(root);

            xmlDocument.Save(fileName);
        }

        protected List<Trace> TreeToList(TreeView treeView)
        {
            List<Trace> tempTraceList = new List<Trace>();
            Trace tempTrace = new Trace();
            TreeNode parentNode = new TreeNode();
            Match match = Regex.Match(treeView.Nodes[0].Text, @"(?<=id=)[\d]*");
            int currentThread = int.Parse(match.Groups[0].Value);
            int currentId = -1;
            parentsId = new List<int>();

            for (int i = 0; i < treeView.Nodes.Count; i++)
            {
                match = Regex.Match(treeView.Nodes[i].Text, @"\b[A-Za-z\d_]*");
                if (match.Groups[0].Value.ToUpper() == "THREAD")
                {
                    match = Regex.Match(treeView.Nodes[i].Text, @"(?<=id=)[\d]*");
                    currentThread = int.Parse(match.Groups[0].Value);
                    parentsId.Add(0);
                }
                parseChildTree(treeView.Nodes[i], ref currentId, currentThread, ref tempTraceList);
            }

            return tempTraceList;
        }

        protected void parseChildTree(TreeNode treeNode, ref int currentId, int currentThread, ref List<Trace> tempTraceList)
        {
            Trace tempTrace;
            TreeNode parentNode;
            Match match;

            for (int i = 0; i < treeNode.Nodes.Count; i++)
            {
                currentId++;
                tempTrace = new Trace();
                tempTrace.traceId = currentId;
                tempTrace.threadId = currentThread;
                parentNode = treeNode.Nodes[i].Parent;
                if (parentNode != null)
                {
                    match = Regex.Match(parentNode.Text, @"\b[A-Za-z\d_]*");
                    if (match.Groups[0].Value.ToUpper() == "THREAD")
                        parentsId.Clear();
                }
                if (parentsId.Count > 0)
                    tempTrace.parentId = parentsId.Last();
                else
                    tempTrace.parentId = 0;
                match = Regex.Match(treeNode.Nodes[i].Text, @"\b([A-Za-z\d_]*)");
                tempTrace.methodName = match.Groups[0].Value;
                match = Regex.Match(treeNode.Nodes[i].Text, @"(?<=\(Params=)[\d]*");
                tempTrace.paramsCount = int.Parse(match.Groups[0].Value);
                match = Regex.Match(treeNode.Nodes[i].Text, @"(?<=Package=)[A-Za-z\d_]*");
                tempTrace.methodPackage = match.Groups[0].Value;
                match = Regex.Match(treeNode.Nodes[i].Text, @"(?<=time=)[\d]*");
                tempTrace.time = double.Parse(match.Groups[0].Value);
                tempTraceList.Add(tempTrace);
                if (treeNode.Nodes[i].GetNodeCount(true) > 0)
                {
                    parentsCount++;
                    parentsId.Add(currentId);
                    parseChildTree(treeNode.Nodes[i], ref currentId, currentThread, ref tempTraceList);
                }
                else
                {
                    if (parentsId.Count > 0)
                    {
                        int firstParent = parentsId[parentsId.Count - parentsCount];
                        parentsId.Clear();
                        parentsId.Add(firstParent);
                    }
                    parentsCount = 0;
                }
            }
        }

        protected void parseChild(XmlNode childNode)
        {
            Trace tempTrace;
            for (int i = 0; i < childNode.ChildNodes.Count; i++)
            {
                tempTrace = new Trace();
                tempTrace.traceId = currentId;
                tempTrace.threadId = currentThread;
                tempTrace.methodName = childNode.ChildNodes[i].Attributes["name"].Value.ToString();
                tempTrace.methodPackage = childNode.ChildNodes[i].Attributes["package"].Value.ToString();
                tempTrace.time = double.Parse(childNode.ChildNodes[i].Attributes["time"].Value.ToString().Replace("ms", ""));
                tempTrace.paramsCount = int.Parse(childNode.ChildNodes[i].Attributes["paramsCount"].Value);
                tempTrace.parentId = parentsId.Last();

                if (childNode.ChildNodes[i].HasChildNodes)
                {
                    parentsId.Add(currentId);
                    currentId++;
                    traceList.Add(tempTrace);
                    parseChild(childNode.ChildNodes[i]);
                }
                else
                {
                    currentId++;
                    parentsId.RemoveAt(parentsId.Count - 1);
                    traceList.Add(tempTrace);
                }
            }
        }
    }
}
