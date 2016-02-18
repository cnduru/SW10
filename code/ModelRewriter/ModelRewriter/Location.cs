using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ModelRewriter
{
    class Location
    {
        public string id { get; set; }
        public string x { get; set; }
        public string y { get; set; }
        public string name { get; set; }
        public string pc { get; set; }

        public Location(int count, string instLine)
        {
            id = "id" + count;
            pc = new Regex("^[0-9]+").Match(instLine).ToString();
            x = (count * 50).ToString();
            y = "0";
            name = "pc" + new Regex("^[0-9]+\\. +[a-zA-Z]+").Match(instLine)
                .ToString().Replace(" ","").Replace('.','_');
        }
    }


}
