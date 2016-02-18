using System;

namespace ModelRewriter
{
	public class JParser
	{
		public JParser (string path)
		{
			var um = new UppaalModel ("/home/kristian/Desktop/new.xml");
			um.Save ("new2.xml");
		}
	}
}

