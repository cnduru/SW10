using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

using System.Xml.Linq;

namespace ModelRewriter
{
    class Transition
    {
        private string sample = "<transition><source ref=\"id1\"/>" +
            "<target ref=\"id0\"/>" +
            "<label kind=\"select\" x=\"18\" y=\"-51\">se</label>" +
            "<label kind=\"guard\" x=\"18\" y=\"-34\">gu</label>" +
            "<label kind=\"synchronisation\" x=\"18\" y=\"-17\">sy</label>" +
            "<label kind=\"assignment\" x=\"18\" y=\"0\">up</label></transition>";
        public Location source { get; set; }
        public Location target { get; set; }
        private List<Label> labels = new List<Label>();
        private List<Nail> nails = new List<Nail>();

        [Obsolete]
        public Transition ()
        {
            
        }

        public Transition (Location from, Location to)
        {
            source = from;
            target = to;
        }

        public Transition (Location from, Location to, List<Label> labels)
        {
            source = from;
            target = to;
            labels = labels;
        }

        struct Nail 
        {
            public int x;
            public int y;
        }

        int offset = 0;

        public XElement getXML()
        {
            XElement transitionElement = new XElement("transition");
            Nail n = new Nail() { x = 100, y = 100 + offset };
            nails.Add(n);
            XElement nailElement = new XElement("nail");
            nailElement.SetAttributeValue("x", n.x);
            nailElement.SetAttributeValue("y", n.y);
            offset++;


            XElement srcElement = new XElement("source");
            XElement targetElement = new XElement("target");



            srcElement.SetAttributeValue("ref", source);
            targetElement.SetAttributeValue("ref", target);

            transitionElement.Add(srcElement);
            transitionElement.Add(targetElement);
            transitionElement.Add(nailElement);

            return transitionElement;
        }
        public struct Label
        {
            public string content;
            public string kind;
            public int x;
            public int y;
        }
            
    }
}
