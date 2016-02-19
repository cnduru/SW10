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
            /*
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
            doc.Save("C://Users//Avalon//SW10//code//models//sampleGenerated.xml");*/

            // read templates from XML file
            XDocument doc = XDocument.Load(path);
            XMLHandler handler = new XMLHandler(doc);

            // read UPPAAL model
            string text = System.IO.File.ReadAllText(@"C://Users//Avalon//SW10//code//models//sample.xml");
            
            // get index of declarations and insert new fault template
            int declIndex = text.IndexOf("</declaration>");
            string res = text.Insert(declIndex + 14, "\n" + XMLProvider.getFaultTemplate());

            // insert global variable
            res = res.Insert(declIndex, "\n" + "int faultAt = 0;\n");

            // add fault process to system
            res = res.Insert(res.IndexOf("template instantiations here.") + 29, "\nFault = FaultInj();");
            res = res.Insert(res.IndexOf("composed into a system.") + 31, " Fault,");

            // update locations in templates with new transitions
            _templates = handler.getTemplates(path);
            
            // workaround for collection change exception
            List<Transition> tlist;

            // caluclate reachable locations
            foreach (Template t in _templates)
            {
                List<string> ban = new List<string>();
                tlist = new List<Transition>();
                t.calculateReachableLocations();

                foreach (Location loc in t.locations)
                {
                    
                    // check whether we care about the state
                    if ((loc.pc == "None"))
                    {
                        ban.Add(loc.id);
                    }
                }

                foreach (Location originalLocation in t.locations)
                {
                    foreach (KeyValuePair<Location, Location> loc in t.reachableLocs)
                    {
                        if ((!ban.Contains(originalLocation.id) && (!ban.Contains(loc.Value.id)) && (!ban.Contains(loc.Key.id))))
                        {
                            if (loc.Key.name == "V_PUTFIELD")
                            {
                                int wrw = 2;
                            }

                            if (Template.isReachable(loc.Value, loc.Key))
                            {
                                Transition newTransition = new Transition();

                                newTransition.source = loc.Value.id;
                                newTransition.target = loc.Key.id;


                                // add the new transition
                                tlist.Add(newTransition);
                            }
                        }
                    }
                }

                // CONTINUE HERE TOMORROW - I THINK NUMBER OF ELEMENTS ARE CORRECT - 18. But are we avoiding the zeno states?
                t.transitions.AddRange(tlist);
                t.transitions = t.transitions.Distinct().ToList();
            }

            // 

            List<Template> updatedTemplates = new List<Template>();


            // write file to disk
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C://Users//Avalon//SW10//code//models//sampleGenerated.xml"))
            {
                file.Write(res);
            }
        }
    }
}
