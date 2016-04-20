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
                var jClass = new JClass(p);
                model.AddTemplates(jClass);
                jClasses.Add(jClass);
            }
            var methods = new List<string>();
            foreach (var jClass in jClasses)
            {
                jClass.SetSuper(jClasses);
                methods.AddRange(jClass.Methods.Select(x => jClass.Name + "_" + JClass.FirstNonKeyword(x.First())));
            }
            foreach (var jClass in jClasses)
            {
                jClass.UpdateFields();
            }

            foreach (var jClass in jClasses)
            {
                jClass.FindAloc();
            }

            model.AddInvokevirtual(jClasses);

            model.InitDec(20,3,3, methods); 
            model.Save("new3.xml");
		}
	}
}

