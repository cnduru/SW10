using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelRewriter
{
    class Rewriter
    {
        private List<Template> _templates;
        private Declarations _declarations;

        public Rewriter(string path)
        {
            XMLHandler handler = new XMLHandler(path);

            // add new declarations
            _declarations = handler.getDeclarations();

            // add faultAt
            _declarations.addDeclaration("\nint faultAt = 0;");

            // read templates from XML file
            _templates = handler.getTemplates(path);

            // add fault process
            string system = handler.getSystem();
            string sys1 = system.Insert(system.IndexOf(" here.") + 6, "\nFaultInj = Fault();");
            string sys2 = sys1.Insert(sys1.IndexOf("system ") + 6, " FaultInj,");
        }
    }
}
