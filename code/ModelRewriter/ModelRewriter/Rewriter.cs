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
            UppaalModel uml = new UppaalModel(path);
            uml.rewritePCFault("testxml.xml");
            //uml.rewriteDataFault("testxml.xml");
        }
    }
}
