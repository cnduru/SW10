using System;
using System.Xml.Linq;


namespace ModelRewriter
{
    public class Label
    {
        public string content;
        public string kind;
        public int x;
        public int y;

        public void SetCords(Location loc, int offset)
        {
            x = Constants.LabelOffsetX;
            y = Convert.ToInt32(loc.y) + offset;
        }

        public XElement GetXML()
        {
            var tmp = new XElement("label");
            tmp.SetAttributeValue("kind", kind);
            tmp.SetAttributeValue("x", x);
            tmp.SetAttributeValue("y", y);
            tmp.SetValue(content);
            return tmp;
        }
    }
}

