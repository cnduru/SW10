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
        public bool Urgent { get; set; }
        public bool committed { get; set; }
        public Label Label;
        public string Guid;

        private static int nextID = 0;

        public Location(){}

        public Location(string na, int xc, int yc)
        {
            name = na;
            x = xc;
            y = yc;
            Label = new Label{
                content = "1", 
                kind = "exponentialrate", 
                y = y,
                x = Constants.LabelOffsetX * 10 - 70               
            };
            id = "nId" + nextID;
            nextID++;
        }

        public Location(Location caller)
        {
            id = caller.id + "a";
            y = caller.y + Constants.LabelOffsetY * 2;
            x = Constants.LabelOffsetX * 10;
            Label = new Label{
                content = "t <= 5", 
                kind = "invariant", 
                y = y,
                x = Constants.LabelOffsetX * 10 - 70               
            };

        }

        // Method uses count as y coordinate and unique id in UPPAAL model 
        public Location(int identifier, string instLine)
        {
            id = "id" + identifier;
            y = (identifier * Constants.LabelOffsetY * 5);
            x = 0;
            if (identifier >= 0)
            {
                parseInst(identifier, instLine);
            }
            else
            {
                parseCall(instLine);
            }
        }

        public XElement getXML()
        {
            XElement locationElement = new XElement("location");
            locationElement.SetAttributeValue("id", id);
            locationElement.SetAttributeValue("x", x);
            locationElement.SetAttributeValue("y", y);
            XElement nameElement = new XElement("name", name);
            nameElement.SetAttributeValue("x", x + Constants.LabelOffsetX);
            nameElement.SetAttributeValue("y", y - Constants.LabelOffsetY);
            locationElement.Add(nameElement);

            if(Label != null)
            {
                locationElement.Add(Label.GetXML());
            }

            if(Urgent)
            {
                locationElement.Add(new XElement("urgent"));
                return locationElement;
            }

            if (committed)
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
            Label = new Label{
                content = "1", 
                kind = "exponentialrate", 
                y = y,
                x = -70               
            };
        }

        private void parseInst(int count, string instLine){

            /*if(instLine.Contains("try"))
            {
                instLine = "exception catch";
            }*/

            inst = new Instruction(instLine);

            name = "pc" + instLine.Replace("( ", "").Replace(" )", "").Replace("<", "").Replace(">", "")
                .Replace(",", "").Replace(" ", "_").Replace(".", "_").Replace(":", "_").Replace("__", "_");
            Label = new Label{
                content = "t <= " + Constants.instTime, 
                kind = "invariant", 
                y = y,
                x = -70               
            };
        }
    }
}
