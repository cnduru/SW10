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
            // countermeasure parameter has to fit UPPAAL rewrite mode - e.g. "pc" for PCFault and "data" for DataFault
            // should we handle Uppaal model generation without faults?
            UppaalModel uml = new UppaalModel(path, "instruction");
            //uml.rewritePCFault("testxml.xml");
            //uml.rewriteDataFault("testxml.xml");
            //uml.addErrorLocation();
            //uml.Save("new3.xml");


           uml.rewriteInstructionFault("instructionRewrite.xml");

        }
    }
}
