﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics.Tracing;
using System.Collections;
using System.Xml.Schema;


namespace ModelRewriter
{
    class Template
    {
        public string name { get; set; }
        public Location initialLocation = new Location();
        public List<Location> locations = new List<Location>();
        public List<Transition> transitions = new List<Transition>();
        public List<Transition> faultTransitions = new List<Transition>();
        public string localDeclarations;

		public Template(XElement xe)
		{
            var localDeclarations = xe.Element("declaration").Value;

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
            var timeGuard = "t == 1";
            ConstantPool CP = new ConstantPool();
            foreach (var loc in locations)
            {
                List<Label> labels = new List<Label>();
                var instArg = loc.inst.instArgs;
                switch (instArg[0])
                {
                    case "aload":
                        /* aload load a refernce from locals into the opstack
                         * the opstack counter osc is incremented as the opstack grows.
                         * Opstack: .. -> .. , locals[n]
                         */
                        labels = new List<Label>(){
                            new Label { 
                                content = timeGuard, kind = "guard" },
                            new Label { 
                                content = String.Format("os[osp] = loc{0}, osc++, t = 0", instArg[1]), 
                                kind = "assignment" }
                        };
                        transitions.Add(new Transition(loc, PCToLocation(loc.inst.pc + 1),labels)); //TODO is +1 allways true?
                        break;
                    case "arraylength":
                        /* arrays are represented as an object on the heap
                         * the first field (index 0) is used for the lenght
                         * making it identical to getfield 0
                         * Opstack: .. , ref -> .. , lenght
                         */
                        labels = new List<Label>(){
                            new Label { 
                                content = timeGuard, kind = "guard" },
                            new Label { 
                                content = "os[osp] = H[os[osp]], t = 0", kind = "assignment" }
                        };
                        transitions.Add(new Transition(loc, PCToLocation(loc.inst.pc + 1),labels));
                        break;
                    case "getstatic":
                        /* get a static field value of a class, 
                         * where the field is identified by field reference in the constant pool index
                         * Opstack: .. -> .. , CP[n]
                         */
                        labels = new List<Label>(){
                            new Label { 
                                content = timeGuard, kind = "guard" },
                            new Label { 
                                content = String.Format("os[osp] = cp{0}, osp++, t = 0",
                                    CP.Add(String.Join(" ", instArg.Skip(1)))), 
                                kind = "assignment" }
                        };
                        transitions.Add(new Transition(loc, PCToLocation(loc.inst.pc + 3),labels));
                        break;

                    case "iconst":
                        /* Pushes a constant value to the opstack
                         * Opstack: .. -> .. , n
                         */
                        labels = new List<Label>(){
                            new Label { 
                                content = timeGuard, kind = "guard" },
                            new Label { 
                                content = String.Format("os[osp] = {0}, osp++, t = 0", instArg[1]), 
                                kind = "assignment" }
                        };
                        transitions.Add(new Transition(loc, PCToLocation(loc.inst.pc + 1),labels));
                        break;
                    case "ifcmpme":
                        /* if greather or eq
                         * Opstack: .. ,value1, value2 -> ..
                         */
                        labels = new List<Label>(){
                            new Label { 
                                content = timeGuard + " && os[osp - 2] >= os[osp - 1]", kind = "guard" },
                            new Label { 
                                content = "osc = osc - 2, t = 0", kind = "assignment" }
                        };
                        transitions.Add(new Transition(loc, PCToLocation(Convert.ToInt32(instArg[1])), labels));

                        labels = new List<Label>(){
                            new Label { 
                                content = timeGuard + " && os[osc - 2] < os[osc - 1]", kind = "guard" },
                            new Label { 
                                content = "osc = osc - 2, t = 0", kind = "assignment" }
                        };
                        transitions.Add(new Transition(loc, PCToLocation(loc.inst.pc + 3), labels));
                        break;
                    case "invokespecial":
                        break;
                    case "invokevirtual":
                        //done!
                        break;
                    case "ldc":
                        labels = new List<Label>(){
                            new Label { 
                                content = timeGuard, kind = "guard" },
                            new Label { 
                                content = String.Format("os[osp] = cp{0}, osp++, t = 0",
                                    CP.Add(String.Join(" ", instArg.Skip(1)))), 
                                kind = "assignment" }
                        };
                        transitions.Add(new Transition(loc, PCToLocation(loc.inst.pc + 2),labels));
                        break;
                    case "return":
                        break;
                    default:
                        throw new System.NotImplementedException(instArg[0]);

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
            XElement el = new XElement("template");
            el.Add(new XElement("name", name));
            el.Add(new XElement("declaration", localDeclarations));
            
            // add locations
            foreach (var loc in locations)
            {
                el.Add(loc.getXML());
            }

            // add initial state
            XElement initState = new XElement("init");
            initState.SetAttributeValue("ref", initialLocation.id);
            el.Add(initState);

            foreach (var trans in transitions)
            {
                el.Add(trans.getXML());
            }

			return el;
		}

        public void calculateReachableLocations()
        {
            // calculate which states are reachable by a single bit flip
            foreach (Location l in locations)
            {
                foreach (Location lNext in locations)
                {
                    if ((l.id != lNext.id && isReachable(l, lNext)))// && !(reachableLocs.ContainsKey(l) || reachableLocs.ContainsKey(lNext)))
                    {
                        l.reachableLocs.Add(lNext);
                    }
                }
            }
        }

        public static bool isReachable(Location l1, Location l2)
        {
            // 8 bits for a single byte
            for(int i = 0; i < 7; i++)
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
