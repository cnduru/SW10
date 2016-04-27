using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.Configuration;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using System.IO;

namespace ModelRewriter
{
	public class UppaalModel
	{
		const string header = @"<?xml version=""1.0"" encoding=""utf-8""?>" 
			+ @"<!DOCTYPE nta PUBLIC '-//Uppaal Team//DTD Flat System 1.1//EN'"
			+ @" 'http://www.it.uu.se/research/group/darts/uppaal/flat-1_2.dtd'>";


        string globalDeclarations;
		List<Template> templates;
		string system;
		string queries;


        //var fields = new Dictionary<string, List<string>();

		//Create a minimal UPPAAL model
		public UppaalModel() : this(XDocument.Parse(header + @"<nta>"
            + "<declaration></declaration><system></system>"
            + "<queries></queries></nta>")) { }

		//Create a UPPAAL model from existing file
		public UppaalModel(string path, string countermeasure = "") : this(XDocument.Load(path), countermeasure) { }

		//Create a UPPAAL model from xml XDocument
		public UppaalModel(XDocument xml, string countermeasure = "")
		{
			var nta =  xml.Element ("nta");
            globalDeclarations = getGlobalDeclarations(nta.Element("declaration").Value);
			system = getSystem(nta.Element ("system").Value, countermeasure);
            queries = nta.Element ("queries").Value;

            XMLHandler xlh = new XMLHandler(xml);
            templates = xlh.getTemplates();//new List<Template>();
		}

        //Sets dec, sys, and queries
        public void InitDec(int heapSize, int cpSize, int maxPar, List<string> methods)
        {
            var gloDecBuild = new StringBuilder();
            var sysBuild = new StringBuilder();
            gloDecBuild.Append("clock t;\n");
            // exception
            gloDecBuild.Append("bool exceptionOccurred;\n");
            
            //HEAP
            gloDecBuild.Append(String.Format("const int heap_size = {0};\n",heapSize));
            gloDecBuild.Append("int H[heap_size];\n");
            //ConstantPool
            for (int i = 0; i < cpSize; i++) {
                gloDecBuild.Append("int cp" + i + ";\n");
            }
            //Method parameters
            for (int i = 0; i < maxPar; i++)
            {
                gloDecBuild.Append("int par" + i + ";\n");
            }
            //Invoke channel
            gloDecBuild.Append("broadcast chan cVirtual");
            //Method channels
            foreach (var mName in methods)
            {
                gloDecBuild.Append("broadcast chan c" + mName + ";\n");
                sysBuild.Append("i" + mName + " = " + mName +"();\n");
            }
            sysBuild.Append("system " + String.Join(",", methods.Select(x => "i" + x)) + ";\n");
                
            gloDecBuild.Append("bool done = false;\n");
            gloDecBuild.Append("bool opstack_fault = false;\n");

            gloDecBuild.Append(
                "const int classFields[3] = {3, 2, 3};\n\n" +
                "int heapPointer = 0;\n" +
                "int alocNew(int classID){\n"+
                "    int ref = heapPointer;\n" +
                "    if(classID < 0) return -1;\n" +
                "    H[ref] = classID;\n" +
                "    heapPointer += classFields[classID];\n" +
                "    return ref;\n" +
                "}");


            globalDeclarations = gloDecBuild.ToString();
            system = sysBuild.ToString();


            queries = @"Pr[<= 50] (<> done)";
        }

		//Store UPPAAL model to a file
		public void Save(string path)
		{
			//Minimal xml document
			var xml = XDocument.Parse(header + "<nta></nta>");
			var nta = xml.Element ("nta"); //nta is the root element

			nta.Add(BuildXElement("declaration", globalDeclarations));

            foreach (var template in templates) 
			{
				nta.Add(template.getXML());
			}

			nta.Add(BuildXElement("system", system));
            var xqueries = new XElement("queries", "queries");
            var xquery = new XElement("query", "query");
            xquery.Add(BuildXElement("formula", queries));
            xqueries.Add(xquery);
            nta.Add(xqueries);
			xml.Save(path);
		}
            
        //Adds method templates from jbc
        public void AddTemplates(JClass cls)
        {
            foreach (var m in cls.Methods)
            {
                templates.Add(new Template(m, cls.Name));
            }
        }


        public void AddInvokevirtual(List<JClass> cls){
            var methods = new List<string>();
            foreach (var cl in cls)
            {
                foreach (var method in cl.Methods)
                {
                    var sig = method.First();
                    if (cl.IsVirtual(sig))
                    {
                        methods.Add(cl.Name + "_" + JClass.GetMethodName(sig));
                    }
                }
            }
            templates.Add(new Template(methods));
        }

		//Creates a xml element with a tag and value
		private XElement BuildXElement(string tag, string value)
		{
			var t = new XElement(tag, tag);
            t.SetValue(value);
			return t;
		}

        private string getGlobalDeclarations(string declarations)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(declarations);
            sb.Append("\nint faultAt = 0;");
            sb.Append("\nclock globalClock;");

            return sb.ToString();
        }

        string countermeasure;
        
        private string getSystem(string stem, string countermeasure)
        {
            List<string> loadedSystem = new List<string>();
            List<string> loadedProcesses = new List<string>();
            StringBuilder sb = new StringBuilder();

            string patternSystem = @"(\w*\s=\s\w*\(\));";//@"(Process\d? = \w*\(\));";

            // add fault system
            if(countermeasure == "pc")
            {
                loadedSystem.Add("Fault = FaultInj();\n");
                sb.Append("Fault = FaultInj();\n");
            }
            else if(countermeasure == "data")
            {
                loadedSystem.Add("Fault = dataFault();\n");
                sb.Append("Fault = dataFault();\n");
            }
            else if (countermeasure == "instruction")
            {
                // are new additions to the system needed in this case since edges are just added?
                loadedSystem.Add("Fault = InstFaultInj();\n");
                sb.Append("Fault = InstFaultInj();\n");
            }

            MatchCollection matches = Regex.Matches(stem, patternSystem);

            foreach (Match match in matches)
            {
                try
                {
                    loadedSystem.Add("" + match.Groups[0].Value + "\n");
                    sb.Append("" + match.Groups[0].Value + "\n");
                }
                catch
                {
                }
            }

            bool isLast = false;
            int i = 1;

            sb.Append("\nsystem ");

            foreach (var systemItem in loadedSystem)
            {
                isLast = i == loadedSystem.Count ? true : false;

                sb.Append(systemItem.Substring(0, systemItem.IndexOf(" =")));

                if (!isLast)
                {
                    sb.Append(", ");
                }
                else
                {
                    sb.Append(";");
                }

                i++;
            }

            return sb.ToString();
        }

        public void rewritePCFault(string path)
        {
            foreach (var te in templates)
            {
                te.addFaultTransitions();
            }

            // is this still true??? this has to be first or locations from faultTemplate are not added
            XElement faultTemplateXML = XElement.Parse(XMLProvider.getFaultTemplate());
            XMLHandler xhl = new XMLHandler();

            Template faultTemplate = xhl.getTemplatePCFault(faultTemplateXML);
            faultTemplate.Locations[1].committed = true;
            templates.Add(faultTemplate);

            Save(path);
        }


        public void rewriteDataFault(string path)
        {
            XElement dataFaultTemplateXML = XElement.Parse(XMLProvider.getDataFaultTemplate());
            XMLHandler xhl = new XMLHandler();
            Template dataFaultTemplate = xhl.getTemplateDataFault(dataFaultTemplateXML);
            dataFaultTemplate.Locations[2].committed = true;

            // todo: generalize this
            globalDeclarations += "\nclock faultClock;\n";
            globalDeclarations += "int faultTime;\n";
            globalDeclarations += "int bitPos;\n";
            globalDeclarations += "broadcast chan f;\n";

            templates.Add(dataFaultTemplate);


            foreach (var te in templates)
            {
                // ignore fault template
                if(te.name.Contains("fault") || te.name.Contains("Fault"))
                {
                    continue;
                }

                foreach (var l in te.Locations)
                {
                    // if location is not a part of the original program, skip it
                    if (l.pc == null)
                    {
                        continue;
                    }

                    foreach (var l2 in te.Locations)
                    {
                        if(l.id != l2.id)
                        {
                            foreach (var edge in te.Transitions)
                            {
                                if(edge.source.id == l.id && edge.target.id == l2.id)
                                {
                                    edge.grds.content += " && faultClock < faultTime";
                                }
                            } 
                        }
                    }

                }
            }

            Save(path);
        }

        public void rewriteInstructionFault(string path)
        {
            ByteCodeInstructions insts = new ByteCodeInstructions();

            foreach (var tem in templates)
            {
                foreach (var loc in tem.Locations)
                {
                    if(loc.pc == null)
                    {
                        continue;
                    }

                    foreach (var l2 in tem.Locations)
                    {
                        if (loc.id != l2.id)
                        {
                            foreach (var edge in tem.Transitions)
                            {
                                if (edge.source.id == loc.id && edge.target.id == l2.id)
                                {
                                    edge.grds.content += " && faultAtId != " + loc.Guid;
                                }
                            }
                        }
                    }
              
                    // get instruction name and resolve to byte code instruction
                    int index = loc.name.IndexOf("_");
                    string inst = loc.name.Replace("__", "_");//.Substring(loc.name.IndexOf("_") + 1);
                    inst = Regex.Replace(inst, @"pc\d*_", "");
                    inst = inst.Replace("\n", "");
                    string instNotSpecial = Regex.Replace(inst, @"_\d*", "");

                    BytecodeInstruction bci = insts.instructionToBytecode(inst);
                    BytecodeInstruction bciNotSpecial = insts.instructionToBytecode(instNotSpecial);

                    // for handling special case in instruction, e.g. iconst_0 vs. ifeq 7
                    if(bciNotSpecial != null)
                    {
                        // special case, overwrite bci
                        bci = bciNotSpecial;
                    }

                    var tempTrans = new List<Transition>();

                    if(bci == null || bci.relatedInstruction.mnemonic == "") // should we use "" or null for non-valid instruction?
                    {   
                        // make transition to error state if invalid instruction
                        var x = tem.idToLocation(Constants.errorLocId);
                        var labs = new List<Label>()
                        {
                            new Label() 
                            {
                                content = string.Format("faultAtId == {0}", loc.Guid),
                                kind = "guard"
                            }
                         };   

                        Transition errorTransition = new Transition(loc, x, labs);
                        tempTrans.Add(errorTransition);
                    }

                    if (bci != null && index != -1)
                    {

                        foreach (var edge in tem.Transitions)
                        {   
                            // for inst fault edges
                            if (edge.source.id == loc.id && !edge.source.id.Contains("-") && !edge.target.id.Contains("-"))
                            {
                                var a = bci.relatedInstruction;

                                var labs = a.getLabels();

                                // add guard to distribute probability equally among fault and valid edge
                                Label probLbl = new Label() {content = "faultAtId == " + loc.Guid, kind = "guard", x = loc.x, y = loc.y };
                                labs.Add(probLbl);

                                int lx = 50, lx2 = 90, ly = 100;
                                Transition tt = new Transition(edge.source, edge.target, labs);

                                List<Transition.Nail> nails = new List<Transition.Nail>();
                                Transition.Nail nail1 = new Transition.Nail { x = loc.x - lx, y = loc.y - ly };
                                Transition.Nail nail2 = new Transition.Nail { x = loc.x - lx2, y = loc.y - ly };
                                nails.Add(nail1);
                                nails.Add(nail2);
                                tt.nails = nails;

                                tempTrans.Add(tt);
                            }
                        }
                    }

                    tem.Transitions.AddRange(tempTrans);
                }
            }

            // add inst fault template
            XElement instFaultTemplateXML = XElement.Parse(XMLProvider.getInstructionFaultTemplate());
            XMLHandler xhl = new XMLHandler();
            Template instFaultTemplate = xhl.getTemplateDataFault(instFaultTemplateXML);
            instFaultTemplate.Locations[1].committed = true;

            // define number range for fault number
            Label selectFaultIdLabel = new Label(){content = string.Format("i:int[0,{0}]", XMLHandler.idCount), kind = "select", x = -110, y = -127};
            instFaultTemplate.Transitions[0].labels.Add(selectFaultIdLabel);

            // todo: generalize this
            globalDeclarations += "\nint faultAtId;\n";

            templates.Add(instFaultTemplate);


            Save(path);

        }

        List<List<string>> findMethods(IEnumerable<string> jbc)
        {
            var methods = new List<List<string>>();
            List<string> curMethod = new List<string>();

            var methodStart = new Regex("^(privat)|(public).+\\(");
            var inst = new Regex("^([0-9]+\\.)"); 
            foreach(var line in jbc) 
            {
                if (methodStart.IsMatch(line))
                {
                    curMethod = new List<string>();
                    curMethod.Add(line);
                    methods.Add(curMethod);
                }
                else if (inst.IsMatch(line))
                {
                    curMethod.Add(line);
                }
            }
            return methods;
        }

        List<string> findFields(IEnumerable<string> jbc)
        {
            var fields = new List<string>();
            var methodStart = new Regex("^(privat)|(public).+\\(");
            var field = new Regex("^(privat)|(public)");
            foreach (var line in jbc)
            {
                if (!methodStart.IsMatch(line) && field.IsMatch(line))
                {
                    fields.Add(line);
                }
            }
            return fields;
        }

        private int getHeapsize()
        {
            foreach (var template in templates)
            {
                if (template.name.Contains("Install"))
                {
                    //var cln = template.name.Split("_")[0];

                };

            }
            return 0;
        }

        public void addErrorLocation()
        {
            // add extra error location for each template
            foreach (var t in templates)
            {
                // make location
                Location errorLoc = new Location();
                errorLoc.x = -400;
                errorLoc.y = 200;
                errorLoc.name = "error";
                errorLoc.id = Constants.errorLocId; // arbitrary id set high so as to not interfere with "regular" locs
                t.Locations.Add(errorLoc);
            }
        }
	}
}