using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Security.Cryptography;

namespace ModelRewriter
{
    public class JClass
    {
        JClass super;
        public List<string> Fields;
        string classSig;
        bool fieldInit = false;
        public List<string> Inits;

        public string Name;
        public List<List<string>> Methods;
        public List<int> catchPCs;

        public JClass(string filePath)
        {
            var lines = File.ReadAllText(filePath).Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Where(x => x != "").ToArray();
            classSig = lines[0];
            Name = classSig.Split(new string[] { "extends" }, StringSplitOptions.None).First().Replace("Class", "").Replace(" ", "");
            Methods = findMethods(lines.Skip(1));
            Fields = findFields(lines.Skip(1));
        }


        public List<string> GetMethod(string methodName)
        {
            foreach (var method in Methods)
            {
                if (GetMethodName(method) == methodName)
                {
                    return method;
                }
            }
            return null;
        }

        public void SetSuper(List<JClass> classes)
        {
            if (classSig.Contains(" extends "))
            {
                var superName = classSig.Split(new string[] { " extends " }, StringSplitOptions.None).Last();
                foreach (var cls in classes)
                {
                    if (cls.Name == superName)
                    {
                        super = cls;
                    }
                }    
            }
        }

        public void UpdateFields()
        {
            if (!fieldInit && super != null)
            {
                super.UpdateFields();
                Fields.AddRange(super.Fields);
            }
            fieldInit = true;
        }

        List<List<string>> findMethods(IEnumerable<string> jbc)
        {
            var methods = new List<List<string>>();
            List<string> curMethod = new List<string>();
            catchPCs = new List<int>();

            var methodStart = new Regex("\\(.+\\;");
            var inst = new Regex("^([0-9]+\\.)");
            var exception = new Regex("catch start: ([0-9]+);");

            foreach(var line in jbc) 
            {
                if (methodStart.IsMatch(line))
                {
                    curMethod = new List<string>();
                    curMethod.Add(line);
                    methods.Add(curMethod);
                    catchPCs.Add(-1);
                }
                else if (inst.IsMatch(line))
                {
                    curMethod.Add(line);
                    catchPCs.Add(-1);
                }else if(exception.IsMatch(line))
                {
                    Match match = exception.Match(line);
                    if (match.Success)
                    {
                        catchPCs.Add(Convert.ToInt32(match.Groups[1].Value));
                    }
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

        public bool IsVirtual(string methodDef){
            //is public and not static and not ctor.
            return methodDef.Contains("public") && !methodDef.Contains("static") && methodDef != GetCTOR().First();
        }

        public static string GetMethodName(List<string> method)
        {
            return GetMethodName(method.First());
        }

        public static string GetMethodName(string methodDef)
        {
            return FirstNonKeyword(methodDef);
        } 

        public List<string> GetCTOR()
        {
            foreach (var method in Methods)
            {
                if (Name == GetMethodName(method.First()))
                {
                    return method;
                }
            }
            throw new MissingFieldException("no CTOR");

        }

        public void FindAloc()
        {
            Inits = GetAloc(GetCTOR());
        } 

        public static List<string> GetAloc(List<string> method)
        {
            var aloc = new Regex("new ([a-zA-Z][a-zA-Z0-9]*)");
            var inits = new List<string>();

            // temp fix for exception
            if(method == null)
                return new List<String>();

            foreach (var line in method)
            {
                if (aloc.IsMatch(line))
                {
                    var cls = aloc.Match(line).ToString().Split(new string[] { "new " }, StringSplitOptions.None).Last();
                    inits.Add(cls);
                }
            };
            return inits;
        }

        public static string FirstNonKeyword(string sig)
        {
            List<string> javaKeywords = new List<string>
                {"abstract", "assert", "boolean", "break", "byte", "case", 
                    "catch", "char", "class", "const", "continue", "default", "do", "double", "else", 
                    "enum", "extends", "final", "finally", "float", "for", "goto", "if", "implements", 
                    "import", "instanceof", "int", "interface", "long", "native", "new", "package", 
                    "private", "protected", "public", "return", "short", "static", "strictfp", "super", 
                    "switch", "synchronized", "this", "throw", "throws", "transient", "try", "void", 
                    "volatile", "while", "false", "null", "true"
                };
            foreach (var word in sig.Split(' '))
            {
                if (!javaKeywords.Contains(word) && word != "")
                {
                    return word.Split('(').First();
                }
            }
            return null;
        }
    }
}

