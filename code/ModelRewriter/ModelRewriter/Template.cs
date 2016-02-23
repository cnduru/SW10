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
            locations = ResolveLocations(method);
            ResolveCode();
        }

        private List<Location> ResolveLocations(List<string> method)
        {
            var locs = new List<Location>();
            for (int i = 1; i < method.Count ; i++)
            {
                locs.Add(new Location(i-1, method[i])); 
            }
            return locs;
        }

        //Magic happens here!
        private void ResolveCode()
        {
            foreach (var loc in locations)
            {
                switch (loc.inst.instArgs[0])
                {
                    case "aload":
                        var t = new Transition(loc, PCToLocation(loc.inst.pc + 1)); //TODO is that allways true?
                        transitions.Add(t);
                        break;
                    default:
                        throw new System.NotImplementedException(loc.inst.instArgs[0]);
                }
            }
        }

        private Location PCToLocation(int pc)
        {
            foreach (var loc in locations)
            {
                if (loc.inst.pc == pc)
                {
                    return loc;
                }
            }
            throw new KeyNotFoundException();
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

        string FirstNonKeyword(string sig){
            List<string> javaKeywords = new List<string>{"abstract", "assert", "boolean", "break", "byte", "case", 
                "catch", "char", "class", "const", "continue", "default", "do", "double", "else", 
                "enum", "extends", "final", "finally", "float", "for", "goto", "if", "implements", 
                "import", "instanceof", "int", "interface", "long", "native", "new", "package", 
                "private", "protected", "public", "return", "short", "static", "strictfp", "super", 
                "switch", "synchronized", "this", "throw", "throws", "transient", "try", "void", 
                "volatile", "while", "false", "null", "true"};
            foreach (var word in sig.Split(' '))
            {
                if (!javaKeywords.Contains(word))
                {
                    return word;
                }
            }
            return null;
        }
    }
}
