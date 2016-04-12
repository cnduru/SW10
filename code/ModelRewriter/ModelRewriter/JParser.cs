using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace ModelRewriter
{
	public class JParser
	{
        public JParser(IEnumerable<string> path)
		{
            UppaalModel model = new UppaalModel();
            var jClasses = new List<JClass>();
            foreach (var p in path)
            {
                var name = p.Split(new char[]{'.'}).First();
                var jClass = new JClass(p);
                model.AddTemplates(jClass);
                jClasses.Add(jClass);
            }
            foreach (var jClass in jClasses)
            {
                jClass.SetSuper(jClasses);
            }
            model.updateDec(); 
            model.Save("new3.xml");
		}
	}
}

