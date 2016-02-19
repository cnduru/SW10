using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModelRewriter
{
    class XMLHandler
    {
        private XDocument _doc;

        public XMLHandler(XDocument doc)
        {
            //_doc = XDocument.Load(path);
            _doc = doc;
        }

        public string getDeclarations()
        {
            return (string)_doc.Descendants("declaration").ElementAt(0);
        }

        public List<Template> getTemplates(string path)
        {
            List<Template> templates = new List<Template>();
            

            foreach (var template in _doc.Descendants("template"))
            {
				Template t = new Template(template);

                // store name object from XML
                t.name = (string)template.Element("name");

                // store information about locations from UPPAAL model in objects from XML
                foreach (var locs in template.Descendants("location"))
                {
                    Location l = new Location(0,"");

                    l.id = (string)locs.Attribute("id");
                    l.name = (string)locs.Element("name");

                    // translate roman numerals to integers
                    try
                    {
                        l.pc = Convert.ToString(RomanToInt.RomanToNumber(l.name.Substring(0, l.name.IndexOf("_"))));
                    }catch (Exception ex)
                    {
                        // yeah.. this should probably be handled more eloquently..
                        l.pc = "None";
                    }

                    l.x = (string)locs.Attribute("x");
                    l.y = (string)locs.Attribute("y");

                    t.locations.Add(l);
                }

                // store information about transitions from UPPAAL model in objects from XML
                List<Transition> transitions = new List<Transition>();

                foreach (var trans in template.Descendants("transition"))
                {
                    Transition srcDstPair = new Transition();
                    srcDstPair.source = (string)trans.Element("source").Attribute("ref");
                    srcDstPair.target = (string)trans.Element("target").Attribute("ref");
                    transitions.Add(srcDstPair);
                }

                t.transitions = transitions;

                templates.Add(t);            
            }

            return templates;
        }

        public string getSystem()
        {
            string system = null;

            // this foreach is not really needed but i couldn't find a way to just grab the first element
			//[0]?
            foreach (var sys in _doc.Descendants("system"))
            {
                system = (string)sys;
            }

            return system;
        }

        //Creates a xml element with an tag and value
        public XElement BuildXElement(string tag, string value)
        {
            var t = new XElement(tag, tag);
            t.SetValue(value);
            return t;
        }
    }
}
