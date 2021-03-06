﻿using System;
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
            public int paramCount;
        }

        public Stopwatch[] stopwatch;
        public object threadLock = new object();
        public int currentStopwatch = 0;
        public bool isBalanced = false;

        public List<StructTrace> trackList = new List<StructTrace>();
        public List<StructTrace> newTrace = new List<StructTrace>();

        public void StartTrace()
        {
            lock(threadLock)
            {
                currentStopwatch++;
                //if (currentStopwatch > 1)
                    //stopwatch[currentStopwatch - 2].Stop();
                Array.Resize(ref stopwatch, currentStopwatch);
                trackList.Add(getFrameInfo(true));
                stopwatch[currentStopwatch - 1] = Stopwatch.StartNew();
            }
        }

        public void StopTrace()
        {
            lock(threadLock)
            {
                stopwatch[currentStopwatch - 1].Stop();
                trackList.Add(getFrameInfo(false, true));
                //if (currentStopwatch > 0)
                    //stopwatch[currentStopwatch - 1].Start();
            }   
        }

        private StructTrace getFrameInfo(bool isOpened, bool isStopTrace = false)
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
                    structTrace.paramCount = stackFrame.GetMethod().GetParameters().Length;
                    if (isStopTrace)
                    {
                        structTrace.executionTime = stopwatch[currentStopwatch - 1].ElapsedMilliseconds;
                        Array.Resize(ref stopwatch, stopwatch.Count() - 1);
                        currentStopwatch--;
                    }
                    structTrace.traceId = trackList.Count() + 1;
    
                    return structTrace;
                }
            }

            return structTrace;
        }

        private void sortTrackList()
        {
            trackList.Sort((a, b) => a.threadId.CompareTo(b.threadId));
        }

        private List<StructTrace> balanceTrace()
        {
            if (isBalanced)
                return newTrace;
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
                    parentTrackStack.Add(trackList[i].traceId);
                    newTrace.Add(tempTrace);
                    currentLayout++;
                }
                else
                {
                    tempTrace = newTrace.Find(x => x.traceId == parentTrackStack.Last());
                    tempTrace.executionTime = trackList[i].executionTime;
                    newTrace[newTrace.IndexOf(newTrace.First(x => x.traceId == parentTrackStack.Last()))] = tempTrace;
                    parentTrackStack.RemoveAt(parentTrackStack.Count() - 1);
                    currentLayout--;
                }

                i++;
            }
            isBalanced = true;
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
            XAttribute xmlThreadTime = new XAttribute("time", threadExecution(currentThread) + "ms");
            xmlThread.Add(xmlThreadId);
            xmlThread.Add(xmlThreadTime);
            root.Add(xmlThread);
            foreach (StructTrace trace in newTrace)
            {
                if (currentThread != trace.threadId)
                {
                    currentThread = trace.threadId;
                    xmlThread = new XElement("thread");
                    xmlThreadId = new XAttribute("id", currentThread);
                    xmlThreadTime = new XAttribute("time", threadExecution(currentThread) + "ms");
                    xmlThread.Add(xmlThreadId);
                    xmlThread.Add(xmlThreadTime);
                    root.Add(xmlThread);
                }
                XAttribute xmlMethodName = new XAttribute("name", trace.methodName);
                XAttribute xmlTime = new XAttribute("time", trace.executionTime + "ms");
                XAttribute xmlPackage = new XAttribute("package", trace.className);
                XAttribute xmlParametersCount = new XAttribute("paramsCount", trace.paramCount);
                if (trace.parentId > 0)
                {
                    XElement xmlChildMethod = new XElement("method");
                    xmlChildMethod.Add(xmlMethodName);
                    xmlChildMethod.Add(xmlTime);
                    xmlChildMethod.Add(xmlPackage);
                    xmlChildMethod.Add(xmlParametersCount);
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
                    xmlMethod.Add(xmlParametersCount);
                    xmlThread.Add(xmlMethod);
                }
            }
            xmlDocument.Add(root);

            xmlDocument.Save("kek.xml");
        }

        private double threadExecution(int currentThread)
        {
            double timeMs = 0;
            foreach (StructTrace trace in newTrace)
            {
                if ((currentThread == trace.threadId) && (trace.parentId == 0))
                {
                    timeMs += trace.executionTime;
                }
            }

            return timeMs;
        }

        public void getTree()
        {
            balanceTrace();
            int currentThread = newTrace[0].threadId;
            int i = 0;
            Console.WriteLine("[Thread {0}, time {1}ms]", currentThread, threadExecution(currentThread));
            foreach (StructTrace trace in newTrace)
            {
                if (currentThread != trace.threadId)
                {
                    currentThread = trace.threadId;
                    Console.WriteLine("[Thread {0}, time {1}ms]", currentThread, threadExecution(currentThread));
                }
                for (i = 0; i < trace.layout; i++)
                {
                    Console.Write("   ");
                }
                Console.WriteLine("└--Method: {0} in {1} class [{2} ms]", trace.methodName, trace.className, trace.executionTime);          
            }
        }

        public void getTrackList()
        {
            balanceTrace();
            foreach (StructTrace structTrace in newTrace)
            {
                Console.WriteLine("[ID: {3}] [Method {0} in {1} class]\nExecution time: {2} ms", structTrace.methodName, structTrace.className, structTrace.executionTime, structTrace.traceId);
                Console.WriteLine("Thread id: {0}\nIs opened: {1}\n", structTrace.threadId, structTrace.isOpened.ToString());
            }
        }
    }
}
