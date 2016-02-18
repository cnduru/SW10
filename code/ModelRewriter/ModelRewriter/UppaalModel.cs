using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.Configuration;

namespace ModelRewriter
{
	public class UppaalModel
	{
		const string header = @"<?xml version=""1.0"" encoding=""utf-8""?>" 
			+ @"<!DOCTYPE nta PUBLIC '-//Uppaal Team//DTD Flat System 1.1//EN'"
			+ @" 'http://www.it.uu.se/research/group/darts/uppaal/flat-1_2.dtd'>";


		string declaration;
		List<Template> templates;
		string system;
		string queries;

		public UppaalModel () : this(XDocument.Parse(header + @"<nta>
				<declaration>// Place global declarations here.</declaration>
				<system>// Place template instantiations here.
				// List one or more processes to be composed into a system.
				</system><queries></queries></nta>")) { }

		public UppaalModel (string path) : this(XDocument.Load (path)) { }

		public UppaalModel (XDocument xml)
		{
			var nta =  xml.Element ("nta");
			declaration =  nta.Element ("declaration").Value;
			system = nta.Element ("system").Value;
			queries = nta.Element ("queries").Value;

			var xh = new XMLHandler (xml);
			templates = xh.getTemplates ("useless?");
		}

		public void Save(string path)
		{
			var xml = XDocument.Parse (header + "<nta></nta>");
			var nta = xml.Element ("nta");

			nta.Add (BuildXElement("declaration", declaration));
			foreach (var template in templates) {
				nta.Add (template.getXML());
			}

			nta.Add (BuildXElement("system", system));
			nta.Add (BuildXElement("queries", queries));
			xml.Save (path);
		}

		private XElement BuildXElement(string tag, string value)
		{
			var t = new XElement (tag, tag);
			t.SetValue (value); 
			return t;
		}
	}
}

