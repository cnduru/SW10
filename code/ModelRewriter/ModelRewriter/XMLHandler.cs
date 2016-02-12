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

        public XMLHandler(string path)
        {
            _doc = XDocument.Load(path);
        }

        public Declarations getDeclarations()
        {
            // store declarations from XML
            Declarations decl = new Declarations();

            // this foreach is not really needed but i couldn't find a way to just grab the first element
            foreach (var dec in _doc.Descendants("declaration"))
            {
                decl.addDeclaration((string)dec.Value);
            }

            return decl;
        }

        public List<Template> getTemplates(string path)
        {
            List<Template> templates = new List<Template>();
            

            foreach (var template in _doc.Descendants("template"))
            {
                Template t = new Template();

                // store name object from XML
                t.name = (string)template.Element("name");

                // store location objects from XML
                foreach (var locs in template.Descendants("location"))
                {
                    Location l = new Location();

                    l.id = (string)locs.Attribute("id");
                    l.x = (string)locs.Attribute("x");
                    l.y = (string)locs.Attribute("y");

                    t.locations.Add(l);
                }

                templates.Add(t);
            }

            return templates;
        }
    }
}
