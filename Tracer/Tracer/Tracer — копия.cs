using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Tracer
{
    class Tracer
    {
        public struct StructTrace
        {
            public int traceId;
            public int parentId;
            public int layout;
            public int threadId;
            public string methodName;
            public string className;
            public double executionTime;
            public bool isOpened;
        }

        public struct OpenedTrace
        {
            public string className;
            public string methodName;
        }

        public Stopwatch[] stopwatch;
        public object threadLock = new object();

        public List<StructTrace> trackList = new List<StructTrace>();
        public List<StructTrace> newTrace = new List<StructTrace>();

        public void StartTrace()
        {
            lock(threadLock)
            {
                Array.Resize(ref stopwatch, trackList.Count() + 1);
                stopwatch[trackList.Count()] = new Stopwatch();
                stopwatch[trackList.Count()].Start();
                trackList.Add(getFrameInfo(true));
            }
        }

        public void StopTrace()
        {
            lock(threadLock)
            {
                stopwatch[trackList.Count() - 1].Stop();
                trackList.Add(getFrameInfo(false));
            }
        }

        private StructTrace getFrameInfo(bool isOpened)
        {
            StructTrace structTrace = new StructTrace();

            StackTrace stackTrace = new StackTrace(true);
            StackFrame stackFrame;

            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                stackFrame = stackTrace.GetFrame(i);
                string className = stackFrame.GetMethod().DeclaringType.Name;
                if (className != typeof(Tracer).Name)
                {
                    structTrace.methodName = stackFrame.GetMethod().Name;
                    structTrace.className = stackFrame.GetMethod().DeclaringType.Name;
                    structTrace.isOpened = isOpened;
                    structTrace.threadId = Thread.CurrentThread.ManagedThreadId;
                    structTrace.executionTime = stopwatch[trackList.Count() - 1].ElapsedMilliseconds;
                    structTrace.traceId = trackList.Count() + 1;

                    return structTrace;
                }
            }

            return structTrace;
        }

        private void sortTrackList()
        {
            trackList.Sort((a, b) => b.threadId.CompareTo(a.threadId));
        }

        private List<StructTrace> balanceTrace()
        {
            sortTrackList();
            List<int> parentTrackStack = new List<int>();
            StructTrace tempTrace;
            int i = 0;
            int currentLayout = 0;
            foreach (StructTrace trace in trackList)
            {
                tempTrace = trackList[i];
                if (parentTrackStack.Count() > 0)
                {
                   tempTrace.parentId = parentTrackStack[parentTrackStack.Count() - 1];
                }

                tempTrace.layout = currentLayout;

                if (tempTrace.isOpened)
                {
                    currentLayout++;
                    parentTrackStack.Add(trackList[i].traceId);
                    newTrace.Add(tempTrace);
                }
                else
                {
                    currentLayout--;
                    parentTrackStack.RemoveAt(parentTrackStack.Count() - 1);
                }

                i++;
            }

            return newTrace;
        }

        public void buildXml()
        {
            XDocument xmlDocument = new XDocument();
            XElement xmlThread = new XElement("thread");
            XElement xmlMethod = new XElement("method");
            XElement root = new XElement("root");
            int currentThread = newTrace[0].threadId;
            XAttribute xmlThreadId = new XAttribute("id", currentThread);
            xmlThread.Add(xmlThreadId);
            root.Add(xmlThread);
            foreach (StructTrace trace in newTrace)
            {
                if (currentThread != trace.threadId)
                {
                    currentThread = trace.threadId;
                    xmlThread = new XElement("thread");
                    xmlThreadId = new XAttribute("id", currentThread);
                    xmlThread.Add(xmlThreadId);
                    root.Add(xmlThread);
                }
                XAttribute xmlMethodName = new XAttribute("name", trace.methodName);
                XAttribute xmlTime = new XAttribute("time", trace.executionTime + "ms");
                XAttribute xmlPackage = new XAttribute("package", trace.className);
                if (trace.parentId > 0)
                {
                    XElement xmlChildMethod = new XElement("method");
                    xmlChildMethod.Add(xmlMethodName);
                    xmlChildMethod.Add(xmlTime);
                    xmlChildMethod.Add(xmlPackage);
                    xmlMethod.Add(xmlChildMethod);
                    xmlMethod = xmlChildMethod;
                }
                else
                {
                    if (!xmlMethod.IsEmpty)
                        xmlThread.Add(xmlMethod);
                    xmlMethod = new XElement("method");
                    xmlMethod.Add(xmlMethodName);
                    xmlMethod.Add(xmlTime);
                    xmlMethod.Add(xmlPackage);
                    xmlThread.Add(xmlMethod);
                }
            }
            xmlDocument.Add(root);

            xmlDocument.Save("kek.xml");
        }

        public void getTree()
        {
            balanceTrace();
            int i = 0;
            foreach (StructTrace trace in newTrace)
            {
                for (i = 0; i < trace.layout; i++)
                {
                    Console.Write("   ");
                }
                Console.WriteLine("--- Method: {0} in {1} class [{2} ms]", trace.methodName, trace.className, trace.executionTime);          
            }
        }

        public void getTrackList()
        {
            trackList = balanceTrace();
            foreach (StructTrace structTrace in trackList)
            {
                Console.WriteLine("[ID: {3}] [Method {0} in {1} class]\nExecution time: {2} ms", structTrace.methodName, structTrace.className, structTrace.executionTime, structTrace.traceId);
                Console.WriteLine("Thread id: {0}\nIs opened: {1}\n", structTrace.threadId, structTrace.isOpened.ToString());
            }
        }
    }
}
