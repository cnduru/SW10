using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics.Tracing;
using System.Collections;
using System.Xml.Schema;
using System.Dynamic;
using System.Collections.Specialized;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Policy;


namespace ModelRewriter
{
    class Template
    {
        public string name { get; set; }

        public Location InitialLocation = new Location();
        public List<Location> Locations = new List<Location>();
        public List<Transition> Transitions = new List<Transition>();
        private JClass classSelf;
        //public List<Transition> faultTransitions = new List<Transition>();
        // should probably be removed
        public string localDeclarations = "";

        private ConstantPool CP = new ConstantPool();


        public Template(XElement modelXML)
        {
            // name
            name = modelXML.Element("name").Value;

            // local decls
            var tempDecl = modelXML.Element("declaration");
            if (tempDecl != null)
            {
                localDeclarations = tempDecl.Value;
            }

            // initial location
            //initialLocation.id = modelXML.Element("init").Attribute("ref").Value;
 
            // transitions
            Transitions = transitionsFromXML(modelXML.Elements("transition").ToList());
        }

        public Template(string tName, Location initial, List<Location> locs, List<Transition> transits, string localDecls)
        {
            name = tName;
            InitialLocation = initial;
            Locations = locs;
            Transitions = transits;
            localDeclarations = localDecls;
        }

        private List<Transition> transitionsFromXML(List<XElement> xml)
        {
            // store information about transitions from UPPAAL model in objects from XML
            List<Transition> transitionsList = new List<Transition>();

            foreach (var transition in xml)
            {
                Transition srcDstPair = new Transition();

                srcDstPair.source.id = (string)transition.Element("source").Attribute("ref");
                srcDstPair.target.id = (string)transition.Element("target").Attribute("ref");
                transitionsList.Add(srcDstPair);
            }

            return transitionsList; 
        }

        public Template(List<string> method, JClass self, int catchIndex)
        {
            classSelf = self;
            name = self.Name + "_" + JClass.FirstNonKeyword(method.First());

            Locations = ResolveLocations(method);
            
            // instruction switch case
            ResolveCode(catchIndex);


            localDeclarations = @"const int os_size = 10;
const int locs_size = 10;
int os[os_size]; 
int osp = 0;
int locs[locs_size];

void osp_inc(){
    if (osp >= os_size - 1){
        opstack_fault = true;
        return;
    }
    osp++;
    return;
}

void osp_dec(int i){
    if (osp < i){
        opstack_fault = true;
        return;
    }
    osp -= i;
    return;
}

bool ifcmpme(){
    if (osp > 1){
        return os[osp-2] >= os[osp - 1];
    }
    return false;
}

bool ifcmpeq(){
    if (osp > 1){
        return os[osp-2] == os[osp - 1];
    }
    return false;
}

bool ifcmpge(){
    if (osp > 1){
        return os[osp - 2] >= os[osp - 1] ;
    }
    return false;
}

bool ifcmpne(){
    if (osp > 1){
        return os[osp - 2] != os[osp - 1] ;
    }
    return false;
}


bool ifeq(){
    if (osp > 0){
        return os[osp - 1] == 0;
    }
    return false;
}";
            InitialLocation = Locations.First();
        }

        /* pattern can be:
         * Select, s
         * Guard, g
         * sYnc, y
         * Update, u
         * Invariant, i
         * Exponentialrate, e
         * 
         * e.g. makeLabels("gu", "t == 1", "osp[0] = par0")
         */
        public static List<Label> makeLabels(string pattern, params string[] values)
        {
            var res = new List<Label>();
            var usedList = new List<char>();
            for (int i = 0; i < pattern.Length; i++)
            {
                if (usedList.Contains(pattern[i]))
                {
                    throw new Exception("Nduru should stop doing this");
                }
                usedList.Add(pattern[i]);
                switch (pattern[i])
                {
                    case 's': 
                        res.Add(new Label
                            { 
                                content = values[i], kind = "select"
                            });
                        break;
                    case 'g': 
                        res.Add(new Label
                            { 
                                content = values[i], kind = "guard"
                            });
                        break;
                    case 'y': 
                        res.Add(new Label
                            { 
                                content = values[i], kind = "synchronisation"
                            });
                        break;
                    case 'u': 
                        res.Add(new Label
                            { 
                                content = values[i], kind = "assignment"
                            });
                        break;
                    case 'i': 
                        res.Add(new Label
                            { 
                                content = values[i], kind = "invariant"
                            });
                        break;
                    case 'e': 
                        res.Add(new Label
                            { 
                                content = values[i], kind = "exponentialrate"
                            });
                        break;
                    default:
                        throw new KeyNotFoundException();
                }
            }
            return res;
                
        }

        //The CTOR for the Virtual template.
        public Template(List<string> method){
            name = "Virtual";

            //init Loc
            var initLoc = new Location("Invoke", 0 ,0);
            InitialLocation = initLoc;
            Locations.Add(initLoc);

            //resolve loc
            var SignaChecks = new List<string>();
            var resolveLoc = new Location("Resolver", 0, 100);
            resolveLoc.Urgent = true;
            Locations.Add(resolveLoc);

            Transitions.Add(new Transition(initLoc, resolveLoc,
                makeLabels("yu", "cVirtual?", "clID = H[par0]")
            ));
                
            //Error loc
            var errorLoc = new Location("Error", 200, 50);
            Locations.Add(errorLoc);
            Transitions.Add(new Transition(resolveLoc, errorLoc,
                makeLabels("g","clID == 0")
            ));

            var returnerLoc = new Location("Returner", -100, 300);
            returnerLoc.Urgent = true;
            Locations.Add(returnerLoc);

            Transitions.Add(new Transition(returnerLoc, initLoc, makeLabels("y", "cVirtual!")));

            //Method locations
            for (int i = 0; i < method.Count; i++)
            {
                var mLoc = new Location(method[i], i*200, 200);
                var sigCheck = String.Format("signature(clID, {0}, {1})", 
                    JParser.GetMethodIndex(method[i]).ToString(),
                    JParser.getClassID(mLoc.name.Split('_').First()));
                Locations.Add(mLoc);
                Transitions.Add(new Transition(resolveLoc, mLoc, makeLabels("gy",
                    sigCheck,
                    String.Format("c{0}!", mLoc.name)
                )));
                Transitions.Add(new Transition(mLoc, returnerLoc, 
                    makeLabels("y", String.Format("c{0}?", mLoc.name))));

                SignaChecks.Add(sigCheck);
            }

            Transitions.Add(new Transition(resolveLoc, resolveLoc, makeLabels("gu", 
                String.Format("clID != 0 && !({0})",String.Join(" || ", SignaChecks)),
                "clID = classHierarchy[clID]"
            )));
        }

        private List<Location> ResolveLocations(List<string> method)
        {
            var locs = new List<Location>();
            for (int i = 0; i < method.Count; i++)
            {
                locs.Add(new Location(i - 1, method[i])); 
            }
            return locs;
        }

        //Magic happens here!
        private void ResolveCode(int catchIndex)
        {
            var timeGuard = "t == 1";
            var newLocs = new List<Location>();
            List<Label> labels = new List<Label>();

            foreach (var loc in Locations)
            {
                //TODO maybe move to switch with instArg
                if (loc.inst.pc == -1)
                {
                    // special case for main
                    if (loc.name == "main")
                    {
                        loc.Urgent = true;
                        labels = new List<Label>()
                            {
                                new Label
                                {
                                    // TODO this should probably be fixed..
                                    content = String.Format("t = 0", 0), 
                                    kind = "assignment"
                                }
                            };
                        Transitions.Add(new Transition(loc, PCToLocation(0), labels));
                        continue; //clean up later
                    }

                    labels = new List<Label>()
                    {
                        new Label
                        { 
                            content = timeGuard, kind = "guard"
                        },
                        new Label
                        {
                            content = String.Format("c{0}?", name), 
                            kind = "synchronisation"
                        },
                        new Label
                        {
                            // TODO this should probably be fixed..
                            content = String.Format("locs[{0}] = par{0}, t = 0", 0), 
                            kind = "assignment"
                        }
                    };
                    Transitions.Add(new Transition(loc, PCToLocation(0), labels));

                    continue;
                }
                
                var instArg = loc.inst.instArgs;
                switch (instArg[0])
                {
                    case "aload":
                        /* aload load a refernce from locals into the opstack
                         * the opstack counter osp is incremented as the opstack grows.
                         * Opstack: .. -> .. , locals[n]
                         */
                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                content = timeGuard, kind = "guard"
                            },
                            new Label
                            { 
                                    content = String.Format(" osp_inc(), os[osp - 1] = locs[{0}], t = 0", instArg[1]), 
                                kind = "assignment"
                            }
                        };
                        Transitions.Add(new Transition(loc, NextLocation(loc), labels)); //TODO is +1 allways true?
                        break;

                    case "arraylength":
                        /* arrays are represented as an object on the heap
                         * the first field (index 0) is used for the lenght
                         * making it identical to getfield 0
                         * Opstack: .. , ref -> .. , lenght
                         */
                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                content = timeGuard, kind = "guard"
                            },
                            new Label
                            { 
                                content = "os[osp] = H[os[osp]], t = 0", kind = "assignment"
                            }
                        };
                        Transitions.Add(new Transition(loc, NextLocation(loc), labels));
                        break;

                    case "getstatic":
                        /* get a static field value of a class, 
                         * where the field is identified by field reference in the constant pool index
                         * Opstack: .. -> .. , CP[n]
                         */
                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                content = timeGuard, kind = "guard"
                            },
                            new Label
                            { 
                                    content = String.Format("osp_inc(), os[osp-1] = H[cp{0}], t = 0",
                                    CP.Add(String.Join(" ", instArg.Skip(1)))), 
                                kind = "assignment"
                            }
                        };
                        Transitions.Add(new Transition(loc, NextLocation(loc), labels));
                        break;

                    case "putfield":
                        Transitions.Add(new Transition(loc, NextLocation(loc), makeLabels("gu",
                            timeGuard,
                            String.Format("osp_dec(2), H[os[osp] + {0}] = os[osp + 1] ", 
                                classSelf.Fields.FindIndex(x => x.Split(' ').Last() == instArg[1].Split('.').Last() + ";") + 1))));
                        break;

                    case "getfield":
                        Transitions.Add(new Transition(loc, NextLocation(loc), makeLabels("gu",
                            timeGuard,
                            String.Format("os[osp - 1] = H[os[osp - 1] + {0}]", 
                                classSelf.Fields.FindIndex(x => x.Split(' ').Last() == instArg[1].Split('.').Last()+ ";") + 1))));
                        //lamda compares the name of: public int <name>; with class.<name>
                        break;
                    case "bipush":
                    case "iconst":
                        /* Pushes a constant value to the opstack
                         * Opstack: .. -> .. , n
                         */
                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                content = timeGuard, kind = "guard"
                            },
                            new Label
                            { 
                                    content = String.Format("osp_inc(), os[osp - 1] = {0}, t = 0", instArg[1]), 
                                kind = "assignment"
                            }
                        };
                        Transitions.Add(new Transition(loc, NextLocation(loc), labels));
                        break;
                    case "ifcmpeq":
                        labels = new List<Label>()
                            {
                                new Label
                                { 
                                    content = timeGuard + " && ifcmpeq()", kind = "guard"
                                },
                                new Label
                                { 
                                    content = "osp_dec(2), t = 0", kind = "assignment"
                                }
                            };
                        Transitions.Add(new Transition(loc, PCToLocation(loc.inst.pc + Convert.ToInt32(instArg[1])), labels, -50));

                        labels = new List<Label>()
                            {
                                new Label
                                { 
                                    content = timeGuard + " && !ifcmpeq()", kind = "guard"
                                },
                                new Label
                                { 
                                    content = "opstack_fault = osp < 2 ? true : opstack_fault, osp_dec(2), t = 0", kind = "assignment"
                                }
                            };

                        Transitions.Add(new Transition(loc, NextLocation(loc), labels));
                        break;
                    case "ifcmpme":
                        /* if greather or eq
                         * Opstack: .. ,value1, value2 -> ..
                         */
                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                    content = timeGuard + " && ifcmpme()", kind = "guard"
                            },
                            new Label
                            { 
                                    content = "osp_dec(2), t = 0", kind = "assignment"
                            }
                        };
                        Transitions.Add(new Transition(loc, PCToLocation(loc.inst.pc + Convert.ToInt32(instArg[1])), labels, -50));

                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                    content = timeGuard + " && !ifcmpme()", kind = "guard"
                            },
                            new Label
                            { 
                                    content = "opstack_fault = osp < 2 ? true : opstack_fault, osp_dec(2), t = 0", kind = "assignment"
                            },
                            new Label
                            { 
                                content = timeGuard, kind = "guard"
                            }
                        };
                        Transitions.Add(new Transition(loc, NextLocation(loc), labels));
                        break;
                    case "ifeq":
                        /* if top element equal to 0
                         * Opstack: .. , value1 -> ..
                         */
                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                    content = timeGuard + " && ifeq()", kind = "guard"
                            },
                            new Label
                            { 
                                    content = "osp_dec(1), t = 0", kind = "assignment"
                            }
                        };
                        Transitions.Add(new Transition(loc, PCToLocation(loc.inst.pc + Convert.ToInt32(instArg[1])), labels, -50));

                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                    content = timeGuard + " && !ifeq()", kind = "guard"
                            },
                            new Label
                            { 
                                    content = "opstack_fault = osp < 1 ? true : opstack_fault, osp_dec(1), t = 0", kind = "assignment"
                            }
                        };
                        Transitions.Add(new Transition(loc, NextLocation(loc), labels));
                        break;
                    case "new":
                        labels = new List<Label>()
                            {
                                new Label
                                { 
                                    content = timeGuard, kind = "guard"
                                },
                                new Label
                                { 
                                    content = String.Format("osp_inc(), os[osp-1] = alocNew({0}), t = 0",
                                        JParser.getClassID(instArg[1])), 
                                    kind = "assignment"
                                }
                            };
                        Transitions.Add(new Transition(loc, NextLocation(loc), labels));
                        break;
                    case "invokevirtual":
                        var waiterV = new Location(loc);
                        var param = loc.inst.instArgs.Skip(3).Where(p => p != "()" && p != "(" && p != ")").ToList();
                        labels = makeLabels("gyu",
                            timeGuard,
                            "cVirtual!",
                            String.Format("osp_dec({0}), par0 = os[osp], meID = {1}", 
                                param.Count + 1, 
                                JParser.GetMethodIndex(instArg[2].Replace(".", "_")))
                        );
                        newLocs.Add(waiterV);
                        Transitions.Add(new Transition(loc, waiterV, labels));

                        var virReturn = "t = 0";
                        if (loc.inst.instArgs[1] != "void")
                        {
                            virReturn += ", os[osp] = par0, osp_inc()";
                        }
                        labels = makeLabels("yu", "cVirtual?", virReturn);
                        Transitions.Add(new Transition(waiterV, NextLocation(loc), labels));

                        newLocs.Add(addExceptionLoc(waiterV, catchIndex, instArg));
                        break;    
                    case "invokespecial":
                    case "invokestatic":
                        var waiterS = new Location(loc);
                        Transitions.AddRange(InvokeStatic(loc, waiterS, NextLocation(loc)));
                        newLocs.Add(waiterS);
                        newLocs.Add(addExceptionLoc(waiterS, catchIndex, instArg));
                        break;

                    case "ldc":
                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                content = timeGuard, kind = "guard"
                            },
                            new Label
                            { 
                                    content = String.Format("osp_inc(), os[osp-1] = cp{0}, t = 0",
                                    CP.Add(String.Join(" ", instArg.Skip(1)))), 
                                kind = "assignment"
                            },
                            new Label
                            { 
                                content = timeGuard, kind = "guard"
                            }
                        };
                        Transitions.Add(new Transition(loc, NextLocation(loc), labels));
                        break;

                    case "ireturn":
                    case "return":
                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                content = timeGuard, kind = "guard"
                            },
                            new Label
                            {
                                content = String.Format("c{0}!", name), 
                                kind = "synchronisation"
                            },
                            new Label
                            { 
                                content = timeGuard, kind = "guard"
                            }
                        };

                        if (instArg[0] == "ireturn")
                        {
                            labels.Add(new Label
                                { 
                                    content = String.Format("osp_dec(1), par0 = os[osp], osp = 0, t = 0"),
                                    kind = "assignment"
                                });
                        }
                        else
                        {
                            labels.Add(new Label
                                { 
                                    content = String.Format("osp = 0, t = 0"),
                                    kind = "assignment"
                                });
                        }
                            
                        
                        if (name.Split('_').Last() == "main")
                        {
                            var endLoc = new Location(loc);
                            endLoc.name = "Done";
                            endLoc.Label = new Label
                            {
                                content = "1", 
                                kind = "exponentialrate"
                            };
                            newLocs.Add(endLoc);
                            labels.RemoveAll(l => l.kind == "assignment");
                            Transitions.Add(new Transition(loc, endLoc, labels));
                            break;
                        }
                        Transitions.Add(new Transition(loc, PCToLocation(-1), labels, -100));
                        break;
                    case "dup":
                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                content = "osp_dec(1), osp_inc(), osp_inc(), os[osp - 1] = os[osp - 2]", 
                                kind = "assignment"
                            },
                            new Label
                            { 
                                content = timeGuard, kind = "guard"
                            }
                        };
                        Transitions.Add(new Transition(loc, NextLocation(loc), labels));
                        break;
                    case "sipush":
                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                content = string.Format("osp_inc(), os[osp] = {0}", instArg[1]), 
                                kind = "assignment"
                            },
                            new Label
                            { 
                                content = timeGuard, kind = "guard"
                            }
                        };
                        Transitions.Add(new Transition(loc, NextLocation(loc), labels));
                        break;
                    case "iload":
                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                content = string.Format("osp_inc(), os[osp - 1] = locs[{0}]", instArg[1]), 
                                kind = "assignment"
                            },
                            new Label
                            { 
                                content = timeGuard, kind = "guard"
                            }
                        };
                        Transitions.Add(new Transition(loc, NextLocation(loc), labels));
                        break;
                    case "istore":
                        // fall through to astore because Kristian wanted to save space..
                        // lol, it does the same as astore from our perspetive anyway
                    case "astore":
                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                    content = string.Format("osp_dec(1), locs[{0}] = os[osp]", instArg[1]), 
                                kind = "assignment"
                            },
                            new Label
                            { 
                                content = timeGuard, kind = "guard"
                            }
                        };
                        Transitions.Add(new Transition(loc, NextLocation(loc), labels));
                        break;
                    case "ifcmpge":
                        var labelsNoJump = new List<Label>()
                        {
                            new Label
                            { 
                                content = string.Format("{0} && !ifcmpge()", timeGuard), 
                                kind = "guard"
                            },
                            new Label
                            { 
                                content = "osp_dec(2)", 
                                kind = "assignment"
                            }
                        };
                        var labelsJump = new List<Label>()
                        {
                            new Label
                            { 
                                content = string.Format("{0} && ifcmpge()", timeGuard), 
                                kind = "guard"
                            },
                            new Label
                            { 
                                content = "osp_dec(2)", 
                                kind = "assignment"
                            }
                        };

                        // edge to next location
                        Transitions.Add(new Transition(loc, NextLocation(loc), labelsNoJump));

                        // edge to jump destination
                        int absoluteAddress = loc.inst.pc + Convert.ToInt32(instArg[1]);
                        Location jumpLoc = PCToLocation(absoluteAddress);
                        Transitions.Add(new Transition(loc, jumpLoc, labelsJump, -50));

                        break;
                    case "athrow":
                        var throwLoc = new Location(loc);
                        throwLoc.Urgent = true;
                        newLocs.Add(throwLoc);

                        labels = Template.makeLabels("gu", 
                            timeGuard,
                            "exceptionOccurred = true"
                        );
                        Transitions.Add(new Transition(loc, throwLoc, labels));

                        labels = Template.makeLabels("y", 
                            String.Format("c{0}!", name));
                        
                        Transitions.Add(new Transition(throwLoc,  PCToLocation(-1), labels));
                        break;
                    case "goto":
                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                content = timeGuard, kind = "guard"
                            }
                        };

                        Transitions.Add(new Transition(loc, PCToLocation(loc.inst.pc + Convert.ToInt32(instArg[1])), labels, -50));
                        break;
                    case "ifcmpne":
                        List<Label> labJump = makeLabels("gu", 
                            timeGuard + " && ifcmpne()",   
                            "osp_dec(2)");
                        List<Label> labNoJump = makeLabels("gu",
                            timeGuard + " && !ifcmpne",
                            "osp_dec(2)");

                        Transitions.Add(new Transition(loc, PCToLocation(loc.inst.pc + Convert.ToInt32(instArg[1])), labJump, -50));
                        Transitions.Add(new Transition(loc, NextLocation(loc), labNoJump, -50));
                        break;
                    case "putstatic":
                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                content = timeGuard, kind = "guard"
                            },
                            new Label
                            { 
                                content = String.Format("osp_dec(1), H[cp{0}] = os[osp], t = 0",
                                CP.Add(String.Join(" ", instArg.Skip(1)))), 
                                kind = "assignment"
                            }
                        };
                        Transitions.Add(new Transition(loc, NextLocation(loc), labels));
                        break;
                    case "iadd":
                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                content = timeGuard, kind = "guard"
                            },
                            new Label
                            { 
                                content = "osp_dec(2), osp_inc(), os[osp - 1] = os[osp - 1] + os[osp], t = 0", 
                                kind = "assignment"
                            }
                        };
                        Transitions.Add(new Transition(loc, NextLocation(loc), labels));
                        break;
                    default:
                        labels = new List<Label>()
                        {
                            new Label
                            { 
                                content = timeGuard, kind = "guard"
                            }
                        };
                        Transitions.Add(new Transition(loc, NextLocation(loc), labels));
                        loc.name += "___NOT_IMPLEMENTED___NOT_IMPLEMENTED___NOT_IMPLEMENTED";
                        //throw new System.NotImplementedException(instArg[0]);
                        break;
                }
            }
            newLocs.RemoveAll(x => x == null);
            Locations.AddRange(newLocs);
        }

        private List<Transition>InvokeStatic(Location caller, Location waiter, Location next)
        {
            bool ret = caller.inst.instArgs[1] != "void";
            var objRef = caller.name.Contains("invokespecial") ? 1 : 0;
            string methodName = caller.inst.instArgs[2].Split('.').Last();
            string methodClassName = caller.inst.instArgs[2].Split('.').First();
            if (methodName == "<init>")
            {
                methodName = methodClassName;
            }
            var param = caller.inst.instArgs.Skip(3).Where(p => p != "()" && p != "(" && p != ")").ToList();
            bool included = JParser.ClassNames.Contains(methodClassName);
            var res = new List<Transition>();
            var call = new List<Label>()
            {
                new Label
                {
                    content = "t == 1", kind = "guard" 
                }
            };
            var wait = new List<Label>();
            if (included)
            {
                call.Add(new Label
                    {
                        content = String.Format("osp_dec({0}), t = 0, par0 = os[osp]", param.Count + objRef), 
                        kind = "assignment"
                    });
                call.Add(new Label
                    {
                        content = String.Format("c{0}!", methodClassName + "_" + methodName), 
                        kind = "synchronisation"
                    });
                wait.Add(new Label
                    {
                        content = String.Format("c{0}?", methodClassName + "_" + methodName), 
                        kind = "synchronisation"
                    });
                wait.Add(new Label
                    {
                        content = "!exceptionOccurred", kind = "guard" 
                    });
            }
            else
            {
                call.Add(new Label
                    {
                        content = String.Format("osp_dec({0}), t = 0", param.Count + objRef), 
                        kind = "assignment"
                    });


                // remove exponential label from non-implemented method call locations
                waiter.Label = new Label() { kind = "invariant", content = "t <= 5"};

                wait.Add(new Label
                    {
                        content = "t == 5", kind = "guard" 
                    });
            }
            if (ret)
            {
                wait.Add(new Label
                    {
                        content = "t = 0, os[osp] = par0, osp_inc()", kind = "assignment" 
                    });
            }
            else
            {
                wait.Add(new Label
                    {
                        content = "t = 0", kind = "assignment" 
                    });
            }
            res.Add(new Transition(caller, waiter, call));

            res.Add(new Transition(waiter, next, wait));

            return res;
        }

        private Location addExceptionLoc(Location waiter, int catchIndex, List<string> instArg){
            string methodName = instArg[2].Split('.').Last();
            string methodClassName = instArg[2].Split('.').First();
            if (!JParser.ClassNames.Contains(methodClassName))
            {
                return null;
            }
            var excepLoc = new Location(waiter);
            excepLoc.Committed = true;
            if (methodName == "<init>")
            {
                methodName = methodClassName;
            }

            int classId = classSelf.catchPCs[catchIndex];

            Transitions.Add(new Transition(waiter, excepLoc, 
                makeLabels("guy", 
                    "exceptionOccurred == true",
                    classId != -1 ? "osp_inc()" : "osp = 0",
                    String.Format("c{0}?", methodClassName + "_" + methodName))));
            Transitions.Add(new Transition(excepLoc, PCToLocation(classId), 
                makeLabels("y", String.Format("c{0}!", name))));
            return excepLoc;
        }


        private Location PCToLocation(int pc)
        {
            foreach (var loc in Locations)
            {
                if (loc.inst.pc == pc)
                {
                    return loc;
                }
            }
            throw new KeyNotFoundException();
        }

        public Location NextLocation(Location current)
        {
            var orderedLocs = Locations.OrderBy(x => x.inst.pc).ToList();
            foreach (var loc in orderedLocs)
            {
                if (loc.inst.pc > current.inst.pc)
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
            foreach (var loc in Locations)
            {
                el.Add(loc.getXML());
            }

            // add initial state
            XElement initState = new XElement("init");
            initState.SetAttributeValue("ref", InitialLocation.id);
            el.Add(initState);

            foreach (var trans in Transitions)
            {
                el.Add(trans.getXML());
            }

            return el;
        }

        public void calculateReachableLocations()
        {
           
            // calculate which states are reachable by a single bit flip
            foreach (Location l in Locations)
            {
                foreach (Location lNext in Locations)
                {
                    if ((l.id != lNext.id && isReachable(l, lNext)))// && !(reachableLocs.ContainsKey(l) || reachableLocs.ContainsKey(lNext)))
                    {
                        l.reachableLocs.Add(lNext);
                    }
                }
            }

            // insert new errror loc
            var errLoc = new Location("error", -500, Locations.OrderByDescending(l => l.y).First().y / 2);
            Locations.Add(errLoc);

            foreach (Location l in Locations)
            {
                if (l.pc == null)
                {
                    continue;
                }
                for (int i = l.reachableLocs.Count; i < 8 ; i++)
                {
                    l.reachableLocs.Add(errLoc);
                }
            }
        }

        public static bool isReachable(Location l1, Location l2)
        {
            // 8 bits for a single byte
            for (int i = 0; i < 7; i++)
            {
                try
                {
                    int n1 = Int32.Parse(l1.pc.ToString());
                    int n2 = Int32.Parse(l2.pc.ToString());
                    if (((n1) ^ Convert.ToInt32(Math.Pow(2, i))) == n2)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    // to catch "None"s
                    return false;
                }
            }

            return false;
        }



        public void addFaultTransitions()
        {
            // workaround for collection change exception
            List<Transition> tlist = new List<Transition>();

            // caluclate reachable locations
            calculateReachableLocations();

            foreach (Location originalLocation in Locations)
            {
                if (originalLocation.pc != null)
                {
                    foreach (var item in Transitions.FindAll(t => t.source.id == originalLocation.id))
                    {
                        item.grds.content += " && faultAt > globalClock";
                    }
                }


                foreach (var loc in originalLocation.reachableLocs)
                {
                    // add fault guard
                    Transition tr = new Transition(originalLocation, loc, new List<Label>(), 
                        Convert.ToInt32(originalLocation.pc) < Convert.ToInt32(loc.pc)? -300 : -250);
                    tr.grds.content += "faultAt <= globalClock";
                    tr.grds.x = -100;
                    tr.grds.y = loc.y;

                    // add fault update so only one fault happens each run
                    tr.asms.content += "faultAt = 1000, t = 0";
                    tr.asms.x = -130;
                    tr.asms.y = loc.y + 30;



                    tlist.Add(tr);
                }
            }

            //faultTransitions.AddRange(tlist);
            Transitions.AddRange(tlist);
        }

        public Location idToLocation(string id)
        {
            return Locations.First(l => l.id == id);
        }
    }
}
