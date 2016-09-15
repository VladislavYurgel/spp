using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tracer
{
    class SomeClass
    {
        public static Tracer tracer = Program.tracer;

        public void methodInSomeClass()
        {
            tracer.StartTrace();
            Thread.Sleep(10);
            anotherMethodinSomeClass();
            tracer.StopTrace();
        }
        
        public void anotherMethodinSomeClass()
        {
            tracer.StartTrace();
            Thread.Sleep(20);
            int varOne = 1;
            for (int i = 0; i < 12312432; i++)
            {
                varOne += 1;
            }
            newMethod();
            tracer.StopTrace();
        }

        public void newMethod()
        {
            tracer.StartTrace();
            Thread.Sleep(15);
            newSomeMethod();
            tracer.StopTrace();
        }

        public void newSomeMethod()
        {
            tracer.StartTrace();
            int newVar = 0;
            for (int i = 0; i < 50000; i++)
            {
                newVar++;
            }
            tracer.StopTrace();
        }
    }
}
