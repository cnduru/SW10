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
            source = new Location();
            target = new Location();
        }

        public Transition (Location from, Location to)
        {
            source = from;
            target = to;
        }

        public Transition (Location from, Location to, List<Label> labl = null)
        {
            source = from;
            target = to;
            for (int i = 0; i < labl.Count; i++)
            {
                var tmp = labl[i];
                tmp.SetCords(from, i*Constants.LabelOffsetY);
                labels.Add(tmp);
            }
        }

        struct Nail 
        {
            public int x;
            public int y;
        }
            
        public XElement getXML()
        {
            XElement transitionElement = new XElement("transition");


            XElement srcElement = new XElement("source");
            XElement targetElement = new XElement("target");

            srcElement.SetAttributeValue("ref", source.id);
            targetElement.SetAttributeValue("ref", target.id);

            transitionElement.Add(srcElement);
            transitionElement.Add(targetElement);

            foreach (XElement label in getLabelsXML())
            {
                transitionElement.Add(label);
            }

            foreach (XElement nail in getNailsXML())
	        {
                transitionElement.Add(nail);
        	}

            return transitionElement;
        }

        private List<XElement> getNailsXML()
        {
            List<XElement> nailElements = new List<XElement>();

            foreach (var nail in nails)
            {
                XElement el = new XElement("nail");
                el.SetAttributeValue("x", nail.x);
                el.SetAttributeValue("y", nail.y);
                nailElements.Add(el);
            }

            return nailElements;
        }

        private List<XElement> getLabelsXML()
        {
            List<XElement> labelElements = new List<XElement>();

            foreach (var label in labels)
            {
                labelElements.Add(label.GetXML());
            }

            return labelElements;
        }
    }
}
