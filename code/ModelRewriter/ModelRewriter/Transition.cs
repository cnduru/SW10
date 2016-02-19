using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModelRewriter
{
    class Transition
    {
        public string source { get; set; }
        public string target { get; set; }

        public XElement getXML()
        {
            XElement transitionElement = new XElement("transition");
            XElement srcElement = new XElement("source");
            XElement targetElement = new XElement("target");

            srcElement.SetAttributeValue("ref", source);
            targetElement.SetAttributeValue("ref", target);

            transitionElement.Add(srcElement);
            transitionElement.Add(targetElement);

            return transitionElement;
        }
    }
}
