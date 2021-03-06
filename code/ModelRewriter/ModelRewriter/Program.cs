﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelRewriter
{
    class Program
    {
        static void Main(string[] args)
		{	
			if (args.Count () > 1) {
                switch (args[0])
                {
                    case "b":
                        var target = "xDUP.xml";
                        //new JParser(args.Skip(1), target);
                        //new JParser(new List<string>() { "exlinesDup.txt" });
                        //new JParser(new List<string>() { "virtual/VirtualCGI.txt", "virtual/AclassCGI.txt", "virtual/BclassCGI.txt" }, target);
                        new JParser(new List<string>(){ "simple_purse/Example_dup.txt"}, target);
                        //new JParser(new List<string>(){ "simple_purse/ExampleCFI1.txt"}, target);
                        new Rewriter(target);
                        break;
                    default:
                        new Rewriter(args[1]);
                        break;
                }
			} 
			else 
			{
                new JParser(new List<string>() { "virtualCFI/Virtual.txt", "virtualCFI/Aclass.txt", "virtualCFI/Bclass.txt" }, "vir_CFI.xml");//"Sample.txt", "A.txt", "B.txt" });
                new Rewriter("vir_CFI.xml");//C://Users//Avalon//SW10//code//models//sample.xml");
			}
        }
    }
}
