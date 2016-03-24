using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.Configuration;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;

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
        public void updateDec()
        {
            globalDeclarations = @"clock t;
const int heap_size = 10;
int H[heap_size];
int cp0;
int cp1;
int par0;
broadcast chan mainc;
broadcast chan DubTestc;
bool done = false;
bool opstack_fault = false;";
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
        public void AddTemplate(List<string> method)
        {
            templates.Add(new Template(method));
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
                    // create loops on every 

                    // if location is not a part of the original program, skip it
                    if (l.pc == null)
                    {
                        continue;
                    }


                    int lx = 50, lx2 = 90, ly = 100;

                    var labels = new List<Label>()
                    {
                        new Label
                        { 
                            content = "faultTime == faultClock", kind = "guard", x = l.x, y = l.y + 50
                        },
                        new Label
                        { 
                            content = "f!", kind = "synchronisation"
                        }
                   };

                   // remember nails
                   Transition faultTrans = new Transition(l, l, labels);

                   List<Transition.Nail> nails = new List<Transition.Nail>();
                   Transition.Nail nail1 = new Transition.Nail { x = l.x - lx, y = l.y - ly };
                   Transition.Nail nail2 = new Transition.Nail { x = l.x - lx2, y = l.y - ly };
                   nails.Add(nail1);
                   nails.Add(nail2);
                   faultTrans.nails = nails;

                   te.transitions.Add(faultTrans);
                    
                }
            }

            Save(path);
        }
	}

}

