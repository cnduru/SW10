using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ModelRewriter
{
    class Location
    {
        public Instruction inst;
        public string id { get; set; }
        public string x { get; set; }
        public string y { get; set; }
        public string name { get; set; }
        public string pc 
        {
            get { return inst.pc.ToString(); }
            set { inst.pc = Convert.ToInt32(value); }
        }

        public Location(int count, string instLine)
        {
            inst = new Instruction(instLine);
            id = "id" + count;
            x = (count * 50).ToString();
            y = "0";
            name = "pc" + new Regex("^[0-9]+\\. +[a-zA-Z]+").Match(instLine)
                .ToString().Replace(" ","").Replace('.','_');
            inst = new Instruction(instLine);
        }

        public XElement getXElement(bool urgent = false)
        {
            XElement locationElement = new XElement("location");
            locationElement.SetAttributeValue("id", id);
            locationElement.SetAttributeValue("x", x);
            locationElement.SetAttributeValue("y", y);
            XElement nameElement = new XElement("name", name);
            nameElement.SetAttributeValue("x", x);
            nameElement.SetAttributeValue("y", Convert.ToInt32(y) - 100);

            if(urgent)
            {
                locationElement.Add(new XElement("urgent"));
                return locationElement;
            }

            return locationElement;        
        }
    }
}
