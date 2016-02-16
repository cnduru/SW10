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
            doc.Descendants("declaration").ElementAt(0).SetValue(declarations);

            // read templates from XML file
            _templates = handler.getTemplates(path);
            
            // add fault template
            doc.Root.Elements().ElementAt(0).AddAfterSelf(
                new XElement("Template",
                    new XElement("name", "Fault"),
                    new XElement("location", 
                        new XAttribute("id", "id0"), 
                        new XAttribute("x", "-17"), 
                        new XAttribute("y", "-85")),
                    new XElement("location", 
                        new XAttribute("id", "id1"), 
                        new XAttribute("x", "-144"), 
                        new XAttribute("y", "-85"),
                        new XElement("committed")),
                    new XElement("init", 
                        new XAttribute("ref", "id1")),
                    new XElement("transition",
                        new XElement("source", new XAttribute("ref", "id1")),
                        new XElement("target", new XAttribute("ref", "id0")),
                        new XElement("label", "i:int[0,7]", 
                            new XAttribute("kind", "select"), 
                            new XAttribute("x", "-110"),
                            new XAttribute("y", "-127")),
                        new XElement("label", "faultAt = i", 
                            new XAttribute("kind", "assignment"), 
                            new XAttribute("x", "-110"), 
                            new XAttribute("y", "-68"))
                        )));
           

            // add fault process
            string system = (string)handler.getSystem();
            string sys1 = system.Insert(system.IndexOf(" here.") + 6, "\nFaultInj = Fault();");
            string sys2 = sys1.Insert(sys1.IndexOf("system ") + 6, " FaultInj,");
            doc.Descendants("system").ElementAt(0).SetValue(sys2);

            // save transformed XML model
            doc.Save("C://Users//Avalon//SW10//code//models//sampleGenerated.xml");
        }
    }
}
