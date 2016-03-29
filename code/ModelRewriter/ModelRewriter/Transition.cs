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
        public List<Nail> nails = new List<Nail>();
        public guards grds = new guards();
        public selections sels = new selections();
        public assignments asms = new assignments();
        public synchronizations syncs = new synchronizations();

        public struct guards 
        {
            public string content;
            public int x;
            public int y;
        };

        public struct selections
        {
            public string content;
            public int x;
            public int y;
        };


        public struct assignments
        {
            public string content;
            public int x;
            public int y;
        };

        public struct synchronizations
        {
            public string content;
            public int x;
            public int y;
        };

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

        public Transition (Location from, Location to, List<Label> labl)
        {
            source = from;
            target = to;
            for (int i = 0; i < labl.Count; i++)
            {
                var tmp = labl[i];
                tmp.SetCords(from, i * Constants.LabelOffsetY);
                labels.Add(tmp);
            }
        }

        public struct Nail 
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

            // content is "" if there was no guard
            if(grds.content != "" && grds.content != null)
            {
                transitionElement.Add(getGuards());
            }

            // content is "" if there was no guard
            if (sels.content != "" && sels.content != null)
            {
                transitionElement.Add(getSelections());
            }

            // content is "" if there was no guard
            if (asms.content != "" && asms.content != null)
            {
                transitionElement.Add(getAssignments());
            }

            // content is "" if there was no guard
            if (syncs.content != "" && syncs.content != null)
            {
                transitionElement.Add(getSyncs());
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

        private XElement getGuards()
        {
            XElement el = new XElement("label");

            el.SetValue(grds.content);
            el.SetAttributeValue("kind", "guard");
            el.SetAttributeValue("x", grds.x);
            el.SetAttributeValue("y", grds.y);

            return el;
        }

        private XElement getSelections()
        {
            XElement el = new XElement("label");

            el.SetValue(sels.content);
            el.SetAttributeValue("kind", "select");
            el.SetAttributeValue("x", sels.x);
            el.SetAttributeValue("y", sels.y);

            return el;
        }

        private XElement getAssignments()
        {
            XElement el = new XElement("label");

            el.SetValue(asms.content);
            el.SetAttributeValue("kind", "assignment");
            el.SetAttributeValue("x", asms.x);
            el.SetAttributeValue("y", asms.y);

            return el;
        }

        private XElement getSyncs()
        {
            XElement el = new XElement("label");

            el.SetValue(syncs.content);
            el.SetAttributeValue("kind", "synchronisation");
            el.SetAttributeValue("x", syncs.x);
            el.SetAttributeValue("y", syncs.y);

            return el;
        }
    }
}
