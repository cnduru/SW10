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
        private List<Template> _templates;

        public Rewriter(string path)
        {
            UppaalModel uml = new UppaalModel("sample_timed.xml");
            uml.Save("testxml.xml");
        }
    }
}
