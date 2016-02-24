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
            // read templates from XML file
            XDocument doc = XDocument.Load(path);
            XMLHandler handler = new XMLHandler(doc);

            // read UPPAAL model
            string text = System.IO.File.ReadAllText(path);
            
            // get index of declarations and insert new fault template
            int declIndex = text.IndexOf("</declaration>");
            string res = text.Insert(declIndex + 14, "\n" + XMLProvider.getFaultTemplate());

            // insert global variable
            res = res.Insert(declIndex, "\n" + "int faultAt = 0;\n");

            // add fault process to system
            res = res.Insert(res.IndexOf("template instantiations here.") + 29, "\nFault = FaultInj();");
            res = res.Insert(res.IndexOf("composed into a system.") + 31, " Fault,");

            // write file to disk
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("sampleGenerated.xml"))
            {
                file.Write(res);
            }


            UppaalModel uml = new UppaalModel("sampleGenerated.xml");
            uml.Save("testxml.xml");
            




            // update locations in templates with new transitions
            _templates = handler.getTemplates("lalala");
            
            // workaround for collection change exception
            List<Transition> tlist = new List<Transition>();

            // caluclate reachable locations
            foreach (Template t in _templates)
            {
                tlist = new List<Transition>();
                t.calculateReachableLocations();

                foreach (Location originalLocation in t.locations)
                {
                    foreach (var loc in originalLocation.reachableLocs)
                    {
                        tlist.Add(new Transition(originalLocation, loc));  
                    }
                }

                t.faultTransitions.AddRange(tlist);
            }


            // write templates
            doc = XDocument.Load("sampleGenerated.xml");

            // get each template node
            foreach (XElement template in doc.Root.Elements("template"))
            {
                // avoid the fault injection template
                if(!(template.Element("name").Value == "FaultInj"))
                {
                    foreach (Template writeTemplate in _templates)
                    {
                        writeTemplate.getXML();
                        foreach (Transition l in writeTemplate.faultTransitions)//_templates.ElementAt(2).faultTransitions)
                        {
                            // generate elements
                            XElement locationElement = l.getXML();

                            // add elements
                            template.Element("init").AddAfterSelf(locationElement);
                        }
                    }
                }
            }

            doc.Save("sampleGenerated.xml");
        }
    }
}
