using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Linq;

namespace ModelRewriter
{
    class Transition
    {
        private string sample = "<transition><source ref=\"id1\"/>" +
            "<target ref=\"id0\"/>" +
            "<label kind=\"select\" x=\"18\" y=\"-51\">se</label>" +
            "<label kind=\"guard\" x=\"18\" y=\"-34\">gu</label>" +
            "<label kind=\"synchronisation\" x=\"18\" y=\"-17\">sy</label>" +
            "<label kind=\"assignment\" x=\"18\" y=\"0\">up</label></transition>";
        public string source { get; set; }
        public string target { get; set; }
        private List<Label> labels = new List<Label>();
        private List<Nail> nails = new List<Nail>();

       
        public Transition ()
        {
            
        }

        struct Nail 
        {
            public int x;
            public int y;
        }
        struct Label
        {
            public string kind;
            public int x;
            public int y;
        }
            
    }
}
