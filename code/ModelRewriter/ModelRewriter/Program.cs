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
					new JParser (args[1]);
					break;
				default:
					new Rewriter (args [1]);
					break;
				}
			} 
			else 
			{
				new Rewriter ("C://Users//Avalon//SW10//code//models//sample.xml");
			}
        }
    }
}
