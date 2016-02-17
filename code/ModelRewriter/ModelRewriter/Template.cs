using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelRewriter
{
    class Template
    {
        public string name { get; set; }
        public List<Location> locations = new List<Location>();

        public bool isReachable(Location l1, Location l2)
        {
            for(int i = 0; i < 15; i++)
            {
                try
                {
                    if ((Convert.ToInt16(l1.pc) ^ Convert.ToInt16(Math.Pow(2, i))) == (Convert.ToInt16(l2.pc)))
                    {
                        return true;
                    }
                }catch (Exception ex)
                {
                    // to catch "None"s
                    return false;
                }
            }

            return false;
        }
    }
}
