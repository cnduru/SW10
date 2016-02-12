using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModelRewriter
{
    class XMLHandler
    {
        private string _path = "";

        public XMLHandler(string path)
        {
            _path = path;
            readXML();
        }

        private void readXML()
        {
            XDocument doc = XDocument.Load(_path);
            foreach (var img in doc.Descendants("template"))
            {
                
                string src = (string)img.Attribute("name");
                /*img.SetAttributeValue("src", src + "with-changes");*/
                int x = 2;
            }
        }
    }
}
