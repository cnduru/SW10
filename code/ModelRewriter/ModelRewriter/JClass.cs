﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using MonoDevelop.Core.Collections;

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


    }
}
