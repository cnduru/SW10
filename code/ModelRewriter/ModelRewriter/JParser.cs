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
            foreach (var p in path)
            {
                var name = p.Split(new char[]{'.'}).First();
                model.parseClass(System.IO.File.ReadAllText(p), name);
            }
            model.Save("new3.xml");
		}

      
	}
}

