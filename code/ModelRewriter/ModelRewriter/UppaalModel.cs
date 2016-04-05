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


        //List<List<string>> fields = new List<List<string>>();
        Dictionary<string, List<string>> fields = 
            new Dictionary<string, List<string>>();
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

        public void parseClass(string jbc, string cls){
            var lines = jbc.Split('\n');
            var classSig = lines[0];
            var methods = findMethods(lines.Skip(1));
            fields[cls] = findFields(lines.Skip(1));

            foreach (var m in methods)
            {
                AddTemplate(m, cls);
            }
        }

        //Sets dec, sys, and queries
        public void updateDec()
        {
            int heapsize = getHeapsize();
            globalDeclarations = @"clock t;
const int heap_size = 10;
int H[heap_size];
int cp0;
int cp1;
int par0;
broadcast chan mainc;
broadcast chan DubTestc;
bool done = false;
bool opstack_fault = false;

";
                        /*foreach (var kvp in fields)
            {
                foreach (var f in kvp.Value)
                {
                    
                }
            }*/



            system = @"s = main();
s1 = DubTest();
system s, s1;";
            queries = @"<formula>Pr[<= 50] (<> done)
            </formula>
            <comment>
            </comment>";
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
			nta.Add(BuildXElement("queries", queries));
			xml.Save(path);
		}
            
        //Adds method templates from jbc
        public void AddTemplate(List<string> method, string cls)
        {
            templates.Add(new Template(method, cls));
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
            faultTemplate.locations[1].committed = true;
            templates.Add(faultTemplate);

            Save(path);
        }


        public void rewriteDataFault(string path)
        {
            XElement dataFaultTemplateXML = XElement.Parse(XMLProvider.getDataFaultTemplate());
            XMLHandler xhl = new XMLHandler();
            Template dataFaultTemplate = xhl.getTemplateDataFault(dataFaultTemplateXML);
            dataFaultTemplate.locations[2].committed = true;

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

                foreach (var l in te.locations)
                {
                    // if location is not a part of the original program, skip it
                    if (l.pc == null)
                    {
                        continue;
                    }

                    foreach (var l2 in te.locations)
                    {
                        if(l.id != l2.id)
                        {
                            foreach (var edge in te.transitions)
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
                foreach (var loc in tem.locations)
                {
                    if(loc.pc == "null")
                    {
                        continue;
                    }

                    // get instruction name and resolve to byte code instruction
                    int index = loc.name.IndexOf("_");
                    string inst = loc.name.Replace("__", "_");//.Substring(loc.name.IndexOf("_") + 1);
                    inst = Regex.Replace(inst, @"pc\d*_", "");
                    inst = Regex.Replace(inst, @"_\d*", "");

                    BytecodeInstruction bci = insts.instructionToBytecode(inst);

                    if (index != -1 && bci != null)
                    {
                        var tempTrans = new List<Transition>();

                        foreach (var edge in tem.transitions)
                        {

                            if (edge.source.id == loc.id)
                            {
                                var a = bci.relatedInstruction;

                                var labs = a.getLabels();
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

                        tem.transitions.AddRange(tempTrans);
                    }
                }
            }


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
                errorLoc.id = "id9999"; // arbitrary id set high so as to not interfere with "regular" locs
                t.locations.Add(errorLoc);

                // make transitions
                foreach (var loc in t.locations)
	            {
                    // make sure we only get locations which are from the original program
                    if (loc.pc != null && loc.pc != "None" && loc.id != errorLoc.id)
                    {
                        var errorTransition = new Transition(loc, errorLoc);
                        t.transitions.Add(errorTransition);
                    }
	            }
            }
        }
	}
}