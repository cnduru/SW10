using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ModelRewriter
{
    class Template
    {
        public string name { get; set; }
        public List<Location> locations = new List<Location>();
		XElement xml;

		public Template (XElement xe)
		{
			xml = xe;
		}

		public XElement getXML()
		{
			return xml;
		}
        public Dictionary<Location, Location> reachableLocs = new Dictionary<Location, Location>();
        public List<Transition> transitions = new List<Transition>();

        public void calculateReachableLocations()
        {
            // calculate which states are reachable by a single bit flip
            foreach (Location l in locations)
            {
                foreach (Location lNext in locations)
                {
                    if ((l.id != lNext.id && isReachable(l, lNext)) && !(reachableLocs.ContainsKey(l) || reachableLocs.ContainsKey(lNext)))
                    {
                        reachableLocs.Add(l, lNext);
                    }
                }
            }
        }

        public bool isReachable(Location l1, Location l2)
        {
            for(int i = 0; i < 15; i++)
            {
                try
                {
                    if ((Convert.ToInt16(l1.pc) ^ Convert.ToInt16(Math.Pow(2, i))) == (Convert.ToInt16(l2.pc)))
                    {
                        return true;
                    }
                }catch (Exception ex)
                {
                    // to catch "None"s
                    return false;
                }
            }

            return false;
        }
    }
}
