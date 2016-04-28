using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModelRewriter
{
    class Rewriter
    {
        public Rewriter(string path)
        {
            UppaalModel uml;
            // countermeasure parameter has to fit UPPAAL rewrite mode - e.g. "pc" for PCFault and "heap" for heap fault
            // should we handle Uppaal model generation without faults?
            uml = new UppaalModel(path, "pc");
            uml.rewritePCFault("pcRewrite.xml");

            uml = new UppaalModel(path, "heap");
            uml.rewriteHeapFault("heapRewrite.xml");

            uml = new UppaalModel(path, "locals");
            uml.rewriteLocalFault("localRewrite.xml");

            uml = new UppaalModel(path, "instruction");
            uml.addErrorLocation();
            uml.rewriteInstructionFault("instructionRewrite.xml");

            uml = new UppaalModel(path, "opstack");
            uml.rewriteOpstackFault("opstackRewrite.xml");
        }
    }
}
