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
		public UppaalModel(string path) : this(XDocument.Load(path)) { }

		//Create a UPPAAL model from xml XDocument
		public UppaalModel(XDocument xml)
		{
			var nta =  xml.Element ("nta");
            globalDeclarations = getGlobalDeclarations(nta.Element("declaration").Value);
			system = getSystem(nta.Element ("system").Value);
			queries = nta.Element ("queries").Value;

			var xh = new XMLHandler (xml);
			templates = xh.getTemplates ("useless?");
		}

        //Sets dec, sys, and queries
        public void updateDec()
        {
            globalDeclarations = @"clock t;
int H[10];
int cp0;
int cp1;
int par0;
chan mainc;
chan DubTestc;";
            system = @"s = main();
s1 = DubTest();
system s, s1;";
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

        private string getSystem(string stem)
        {
            List<string> loadedSystem = new List<string>();
            List<string> loadedProcesses = new List<string>();
            StringBuilder sb = new StringBuilder();

            string patternSystem = @"(\w*\s=\s\w*\(\));";//@"(Process\d? = \w*\(\));";

            // add fault system
            loadedSystem.Add("Fault = FaultInj();\n");
            sb.Append("Fault = FaultInj();\n");

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
            // this has to be first or locations from faultTemplate are not added
            XElement faultTemplateXML = XElement.Parse(XMLProvider.getFaultTemplate());
            XMLHandler xhl = new XMLHandler();
            Template faultTemplate = xhl.getTemplate(faultTemplateXML);
            faultTemplate.locations[1].urgent = true;
            templates.Add(faultTemplate);
            Save(path);
        }
	}

}

