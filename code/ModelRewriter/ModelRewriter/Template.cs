using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics.Tracing;
using System.Collections;


namespace ModelRewriter
{
    class Template
    {
        public string name { get; set; }
        public List<Location> locations = new List<Location>();
        public Dictionary<Location, Location> reachableLocs = new Dictionary<Location, Location>();
        public List<Transition> transitions = new List<Transition>();

		XElement xml;

		public Template(XElement xe)
		{
			xml = xe;
		}

        public Template(List<string> method)
        {
            name = FirstNonKeyword(method.First());

            for (int i = 1; i < method.Count ; i++)
            {
                locations.Add(new Location(i-1, method[i])); 
            }




        }
            
		public XElement getXML()
		{
			return xml;
		}

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

        string FirstNonKeyword(string sig){
            List<string> javaKeywords = new List<string>{"abstract", "assert", "boolean", "break", "byte", "case", 
                "catch", "char", "class", "const", "continue", "default", "do", "double", "else", 
                "enum", "extends", "final", "finally", "float", "for", "goto", "if", "implements", 
                "import", "instanceof", "int", "interface", "long", "native", "new", "package", 
                "private", "protected", "public", "return", "short", "static", "strictfp", "super", 
                "switch", "synchronized", "this", "throw", "throws", "transient", "try", "void", 
                "volatile", "while", "false", "null", "true"};
            foreach (var s in sig.Split(' '))
            {
                if (!javaKeywords.Contains(s))
                {
                    return s;
                }
            }
            return null;
        }
    }
}
