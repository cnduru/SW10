using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using MonoDevelop.Core.Collections;
using System.Xml.Xsl.Runtime;

namespace ModelRewriter
{
    public class JClass
    {
        JClass super;
        List<string> Fields;
        string classSig;
        bool fieldInit = false;

        public string Name;
        public List<List<string>> Methods;

        public JClass(string filePath)
        {
            var lines = File.ReadAllText(filePath).Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Where(x => x != "").ToArray();
            classSig = lines[0];
            Name = classSig.Split(new string[] { "extends" }, StringSplitOptions.None).First().Replace("Class", "").Replace(" ", "");
            Methods = findMethods(lines.Skip(1));
            Fields = findFields(lines.Skip(1));
        }

        public void SetSuper(List<JClass> classes){
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

        public void UpdateFields(){
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

            var methodStart = new Regex("\\(.+\\;");
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

        public static bool IsVirtual(string methodDef){
            return methodDef.Contains("public") && !methodDef.Contains("static"); 
        }

        public string GetMethodName(string methodDef){
            return Name + "_" + FirstNonKeyword(methodDef);
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
                if (!javaKeywords.Contains(word))
                {
                    return word;
                }
            }
            return null;
        }
    }
}

