using System;
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
                        var target = "vir_CGI.xml";
                        //new JParser(args.Skip(1), target);
                        //new JParser(new List<string>() { "exlinesDup.txt" });
                        new JParser(new List<string>() { "virtual/VirtualCGI.txt", "virtual/AclassCGI.txt", "virtual/BclassCGI.txt" }, target);
                        //new JParser(new List<string>(){ "simple_purse_cgi/ExampleCGI.txt"});
                        //new JParser(new List<string>(){ "simple_purse/ExampleCFI.txt"}, target);
                        new Rewriter(target);
                        break;
                    default:
                        new Rewriter(args[1]);
                        break;
                }
			} 
			else 
			{
                new JParser(new List<string>() { "simple_purse_cgi/ExampleCGI.txt" }, "new3.xml");//"Sample.txt", "A.txt", "B.txt" });
                new Rewriter("new3.xml");//C://Users//Avalon//SW10//code//models//sample.xml");
			}
        }
    }
}
