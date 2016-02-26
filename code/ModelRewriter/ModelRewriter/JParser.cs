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
		const string sample = @"Class DubTest 
public static void main ( java.lang.String [] 0 ) ;
0.   aload 0
1.   arraylength
2.   iconst 1
3.   ifcmpme 11
6.   getstatic java.lang.System.out : java.io.PrintStream
9.   ldc string 'Hello World!'
11.  invokevirtual void java.io.PrintStream.print ( java.lang.String )
14.  return

public DubTest ( ) ;  
0.   aload 0
1.   invokespecial void java.lang.Object.<init> ( )
4.   return";

		public JParser(string path)
		{
			//var um = new UppaalModel ("/home/kristian/Desktop/new.xml");
			//um.Save ("new2.xml");
            parseClass(sample).Save("new3.xml");
		}

        UppaalModel parseClass(string jbc){
			var lines = jbc.Split('\n');
			var classSig = lines[0];
			var methods = findMethods(lines.Skip(1));
            var model = new UppaalModel();

            foreach (var m in methods)
            {
                model.AddTemplate(m);
            }
            model.updateDec();
            return model;
		}

		List<List<string>> findMethods(IEnumerable<string> jbc)
		{
			var methods = new List<List<string>>();
			List<string> curMethod = new List<string>();

			var methodStart = new Regex("^(privat)|(public)");
			var inst = new Regex("^([0-9]+\\.)"); 
			foreach(var line in jbc) 
			{
				if(methodStart.IsMatch(line)) 
				{
					curMethod = new List<string>();
					curMethod.Add(line);
					methods.Add(curMethod);
				}
				else if(inst.IsMatch(line)) 
				{
					curMethod.Add(line);
				}
			}
			return methods;
		}
	}
}

