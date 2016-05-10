using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ModelRewriter
{
	public class JParser
	{
        public static List<string> ClassNames = new List<string>();
        public static List<string> MethodNames = new List<string>();

        public JParser(IEnumerable<string> path)
		{
            UppaalModel model = new UppaalModel();
            var jClasses = new List<JClass>(); 
            ClassNames.Add("Object");
            foreach (var p in path)
            {
                var jClass = new JClass(p);
                ClassNames.Add(jClass.Name);
                jClasses.Add(jClass);

                foreach (var method in jClass.Methods)
                {
                    var methodName = JClass.FirstNonKeyword(method.First());
                    if (!MethodNames.Contains(methodName))
                    {
                        MethodNames.Add(methodName);
                    }
                }
            }

            foreach (var jClass in jClasses)
            {
                model.AddTemplates(jClass);
            }

            var methods = new List<string>();
            foreach (var jClass in jClasses)
            {
                jClass.SetSuper(jClasses);
                methods.AddRange(jClass.Methods.Select(x => jClass.Name + "_" + JClass.FirstNonKeyword(x.First())));
            }
            //Build the heap
            foreach (var jClass in jClasses)
            {
                jClass.UpdateFields();
            }

            foreach (var jClass in jClasses)
            {
                jClass.FindAloc();
            }

            var heap = AllocateHeap(JClass.GetAloc(jClasses.First().GetMethod("install")), jClasses);
                


            //Add Invoke
            model.AddInvokevirtual(jClasses);

            model.InitDec(heap.Count + 1,3,3, methods); 
            model.Save("new3.xml");
		}

        private List<string> AllocateHeap(List<string> allocs, List<JClass> jClasses)
        {
            var heap = new List<string>();
            heap.AddRange(allocs);
         
            foreach (var clsName in allocs)
            {
                var cls = jClasses.Where(x => x.Name == clsName).ToList().First();
                heap.AddRange(cls.Fields.Select(x => cls.Name + "_" + x));
                heap.AddRange(AllocateHeap(cls.Inits.Skip(1).ToList(), jClasses));
            }
            return heap;
        }

        public static int getClassID(string className)
        {
            for (int i = 0; i < ClassNames.Count; i++)
            {
                if (ClassNames[i] == className)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetMethodIndex(string method)
        {
            var name = JClass.GetMethodName(method).Split('_').Last();
            return MethodNames.FindIndex(x => x == name);
        }
	}
}

