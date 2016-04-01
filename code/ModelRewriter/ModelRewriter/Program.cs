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
				switch (args [0]) {
				case "b":
                    new JParser (args.Skip(1));
                    new Rewriter("new3.xml");
					break;
				default:
					new Rewriter (args [1]);
					break;
				}
			} 
			else 
			{
                //new JParser(new List<string>() { "Sample.txt", "A.txt", "B.txt" });
                new Rewriter("new3.xml");//C://Users//Avalon//SW10//code//models//sample.xml");
			}
        }
    }
}
