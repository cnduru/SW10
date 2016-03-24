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
            UppaalModel uml = new UppaalModel(path, "data");
            uml.rewritePCFault("testxml.xml");
            uml.rewriteDataFault("testxml.xml");
        }
    }
}
