﻿using System;
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

        public List<Template> getTemplates(string path)
        {
            List<Template> templates = new List<Template>();

            foreach (var template in _doc.Descendants("template"))
            {
                templates.Add(getTemplate(template));            
            }

            return templates;
        }

        public Template getTemplate(XElement xel)
        {
            Template t = new Template(xel);
            t.initialLocation.id = xel.Element("init").Attribute("ref").Value;

            // store name object from XML
            t.name = (string)xel.Element("name");

            // store information about locations from UPPAAL model in objects from XML
            foreach (var locs in xel.Descendants("location"))
            {
                Location l = new Location();

                l.id = (string)locs.Attribute("id");
                l.name = (string)locs.Element("name");

                try
                {
                    XElement xele = locs.Element("label");
                    XAttribute invElement = xele.Attribute("kind");
                    int xCoord = Convert.ToInt32(xele.Attribute("x").Value);
                    int yCoord = Convert.ToInt32(xele.Attribute("y").Value);

                    Label invariant = new Label { kind = "invariant", content = (string)xele.Value, x = xCoord, y = yCoord };
                    l.invariant = invariant;
                }catch (Exception ex)
                {

                }

                try
                {
                    // load pc number
                    Regex regex = new Regex(@"pc(\d*)");
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
                }

                l.x = Convert.ToInt32(locs.Attribute("x").Value);
                l.y = Convert.ToInt32(locs.Attribute("y").Value);

                t.locations.Add(l);
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

            t.transitions = transitions;
            t.addFaultTransitions();

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
