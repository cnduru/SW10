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
            // countermeasure parameter has to fit UPPAAL rewrite mode - e.g. "pc" for PCFault and "data" for DataFault
            // should we handle Uppaal model generation without faults?
            //uml = new UppaalModel(path, "pc");
            //uml.rewritePCFault("pcRewrite.xml");

            //uml = new UppaalModel(path, "data");
            //uml.rewriteDataFault("dataRewrite.xml");

            uml = new UppaalModel(path, "instruction");
            uml.addErrorLocation();
            uml.rewriteInstructionFault("instructionRewrite.xml");

        }
    }
}
