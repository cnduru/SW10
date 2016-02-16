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
            XDocument doc = XDocument.Load(path);

            XMLHandler handler = new XMLHandler(doc);

            // add faultAt
            string declarations = handler.getDeclarations();
            declarations += "int faultAt = 0;\n";

            foreach (var sys in doc.Descendants("declarations"))
            {
                sys.SetValue(declarations);
            }

            // read templates from XML file
            _templates = handler.getTemplates(path);

            // add fault process
            string system = (string)handler.getSystem();
            string sys1 = system.Insert(system.IndexOf(" here.") + 6, "\nFaultInj = Fault();");
            string sys2 = sys1.Insert(sys1.IndexOf("system ") + 6, " FaultInj,");
            
            foreach (var sys in doc.Descendants("system"))
            {
                sys.SetValue(sys2);
            }
        }
    }
}
