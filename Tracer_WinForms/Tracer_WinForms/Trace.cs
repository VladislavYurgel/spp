using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer_WinForms
{
    class Trace
    {
        public int threadId;
        public int traceId;
        public int parentId;
        public string methodName;
        public string methodPackage;
        public double time;
        public int paramsCount;

        public override string ToString()
        {
            return "[ID: " + traceId + "] " + methodName + ", parentId: " + parentId;
        }
    }
}
