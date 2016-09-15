using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tracer
{
    class Program
    {
        public static Tracer tracer = new Tracer();

        static void Main(string[] args)
        {
            tracer.StartTrace();
            tracer.StopTrace();
            tracer.StartTrace();
            someMethod();
            tracer.StopTrace();

            SomeClass someClass = new SomeClass();
            someClass.methodInSomeClass();

            //tracer.getTrackList();
            tracer.getTree();
            tracer.buildXml();
        }

        public static int someMethod(int paramOne = 1, int paramTwo = 2)
        {
            tracer.StartTrace();
            Thread newThread = new Thread(anotherMethod);
            newThread.Start();
            //anotherMethod();
            int someVar = 5;
            Thread.Sleep(50);
            tracer.StopTrace();
            return someVar;
        }

        public static void anotherMethod()
        {
            tracer.StartTrace();
            Thread.Sleep(32);
            tracer.StopTrace();
        }
    }
}
