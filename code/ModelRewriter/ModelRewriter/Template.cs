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

        public static bool isReachable(Location l1, Location l2)
        {
            if(l1.id == "id35" && l2.id == "id34")
            {
                int zz = 2;
            }

            for(int i = 0; i < 15; i++)
            {
                try
                {
                    int n1 = Int32.Parse(l1.pc.ToString());
                    int n2 = Int32.Parse(l2.pc.ToString());
                    if (((n1) ^ Convert.ToInt32(Math.Pow(2, i))) == n2)
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
