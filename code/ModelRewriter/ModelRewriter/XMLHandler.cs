using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public XMLHandler()
        {

        }

        public string getDeclarations()
        {
            return (string)_doc.Descendants("declaration").ElementAt(0);
        }

        public Template getTemplatePCFault(XElement xel)
        {
            Template t = getTemplate(xel);
            //t.addFaultTransitions();

            return t;
        }

        public Template getTemplateDataFault(XElement xel)
        {
            // load template
            Template t = getTemplate(xel);
            
            return t;
        }

        public List<Template> getTemplates()
        {
            List<Template> t = new List<Template>();

            foreach (var template in _doc.Descendants("template"))
            {
                t.Add(getTemplate(template));
            }

            return t;
        }

        public static int idCount = 0;

        public Template getTemplate(XElement xel)
        {
            Template t = new Template(xel);
            t.InitialLocation.id = xel.Element("init").Attribute("ref").Value;

            // store name object from XML
            t.name = (string)xel.Element("name");

            // store information about locations from UPPAAL model in objects from XML
            foreach (var locs in xel.Descendants("location"))
            {
                Location l = new Location();

                l.id = (string)locs.Attribute("id");
                l.name = (string)locs.Element("name");
                l.Guid = idCount.ToString();
                idCount++;

                XElement xele = locs.Element("label");
                if (xele != null)
                {
                    int xCoord = Convert.ToInt32(xele.Attribute("x").Value);
                    int yCoord = Convert.ToInt32(xele.Attribute("y").Value);

                    Label invariant = new Label { kind = "invariant", content = (string)xele.Value, x = xCoord, y = yCoord };
                    l.Label = invariant;
                }

                try
                {
                    // load pc number
                    Regex regex = new Regex(@"pc(\d+)");
                    Match match = regex.Match(l.name);

                    if (match.Success)
                    {
                        l.pc = match.Value.Replace("pc", "");
                    }
                }
                catch (Exception ex)
                {
                    // yeah.. this should probably be handled more eloquently..
                    l.pc = "None";
                } //TODO fix fault template

                l.x = Convert.ToInt32(locs.Attribute("x").Value);
                l.y = Convert.ToInt32(locs.Attribute("y").Value);

                t.Locations.Add(l);
            }

            // store information about transitions from UPPAAL model in objects from XML
            List<Transition> transitions = new List<Transition>();

            foreach (var trans in xel.Descendants("transition"))
            {
                Transition srcDstPair = new Transition();

                srcDstPair.source.id = (string)trans.Element("source").Attribute("ref");
                srcDstPair.target.id = (string)trans.Element("target").Attribute("ref");

                // add guards
                srcDstPair.grds = getGuards(trans);

                // add selections
                srcDstPair.sels = getSelections(trans);

                // add updates
                srcDstPair.asms = getAssignments(trans);

                // add synchronizations
                srcDstPair.syncs = getSyncs(trans);

                transitions.Add(srcDstPair);
            }

            t.Transitions = transitions;

            return t;
        }

        public struct generalLabels
        {
            public string content;
            public int x;
            public int y;
        }

        private Transition.guards getGuards(XElement el)
        {
            try
            {
                XElement xel1 = el.Elements("label").Where(l => l.Attribute("kind").Value == "guard").ElementAt(0);

                return new Transition.guards()
                {
                    content = xel1.Value,
                    x = Convert.ToInt32(xel1.Attribute("x").Value),
                    y = Convert.ToInt32(xel1.Attribute("y").Value)
                };
            }
            catch (Exception ex) 
            {
                return new Transition.guards() { content = "", x = 0, y = 0 };
            }
        }

        private Transition.assignments getAssignments(XElement el)
        {
            try
            {
                XElement xel1 = el.Elements("label").Where(l => l.Attribute("kind").Value == "assignment").ElementAt(0);

                return new Transition.assignments()
                {
                    content = xel1.Value,
                    x = Convert.ToInt32(xel1.Attribute("x").Value),
                    y = Convert.ToInt32(xel1.Attribute("y").Value)
                };
            }
            catch (Exception ex)
            {
                return new Transition.assignments() { content = "", x = 0, y = 0 };
            }
        }
        private Transition.selections getSelections(XElement el)
        {
            try
            {
                XElement xel1 = el.Elements("label").Where(l => l.Attribute("kind").Value == "select").ElementAt(0);

                return new Transition.selections()
                {
                    content = xel1.Value,
                    x = Convert.ToInt32(xel1.Attribute("x").Value),
                    y = Convert.ToInt32(xel1.Attribute("y").Value)
                };
            }
            catch (Exception ex)
            {
                return new Transition.selections() { content = "", x = 0, y = 0 };
            }
        }

        private Transition.synchronizations getSyncs(XElement el)
        {
            try
            {
                XElement xel1 = el.Elements("label").Where(l => l.Attribute("kind").Value == "synchronisation").ElementAt(0);

                return new Transition.synchronizations()
                {
                    content = xel1.Value,
                    x = Convert.ToInt32(xel1.Attribute("x").Value),
                    y = Convert.ToInt32(xel1.Attribute("y").Value)
                };
            }
            catch (Exception ex)
            {
                return new Transition.synchronizations() { content = "", x = 0, y = 0 };
            }
        }
    }
}
