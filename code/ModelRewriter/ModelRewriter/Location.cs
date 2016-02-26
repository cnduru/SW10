using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Diagnostics;

namespace ModelRewriter
{
    public class Location
    {
        public Instruction inst;
        public string id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string name { get; set; }
        public string pc  { get; set; }
        public List<Location> reachableLocs = new List<Location>();


        Label invariant;


        public Location()
        {

        }

        public Location(int count, string instLine)
        {
            id = "id" + count;
            y = (count * Constants.LabelOffsetY*4);
            x = 0;
            if (count >= 0)
            {
                parseInst(count, instLine);
            }
            else
            {
                parseCall(instLine);
            }
        }

        public XElement getXML(bool urgent = false, bool committed = false)
        {
            XElement locationElement = new XElement("location");
            locationElement.SetAttributeValue("id", id);
            locationElement.SetAttributeValue("x", x);
            locationElement.SetAttributeValue("y", y);
            XElement nameElement = new XElement("name", name);
            nameElement.SetAttributeValue("x", x + Constants.LabelOffsetX);
            nameElement.SetAttributeValue("y", y - Constants.LabelOffsetY);
            locationElement.Add(nameElement);
            locationElement.Add(invariant.GetXML());
            if(urgent && !committed)
            {
                locationElement.Add(new XElement("urgent"));
                return locationElement;
            }

            if(committed && !urgent)
            {
                locationElement.Add(new XElement("committed"));
                return locationElement;
            }

            return locationElement;        
        }

        private void parseCall(string instLine)
        {
            inst = new Instruction();
            name = new Regex(" [a-zA-Z]+ \\(").Match(instLine)
                .ToString().Replace(" ","").Replace("(","");
            invariant = new Label{
                content = "t <= " + Constants.instTime, 
                kind = "invariant", 
                y = y,
                x = -60               
            };
        }

        private void parseInst(int count, string instLine){
            inst = new Instruction(instLine);
            name = "pc" + new Regex("^[0-9]+\\. +[a-zA-Z]+").Match(instLine)
                .ToString().Replace(" ", "").Replace('.', '_');
            invariant = new Label{
                content = "t <= " + Constants.instTime, 
                kind = "invariant", 
                y = y,
                x = -60               
            };

        }
    }
}
