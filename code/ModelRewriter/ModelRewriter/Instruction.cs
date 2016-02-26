using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace ModelRewriter
{
    public class Instruction
    {
        public int pc;
        public List<string> instArgs = new List<string>(); 

        public Instruction()
        {
            pc = -1;
        }

        public Instruction(string code)
        {
            var pcStr = new Regex("^[0-9]+").Match(code).ToString();
            pc = Convert.ToInt32(pcStr);
            instArgs = code.Replace(pcStr+".","").Split(null).ToList();
            instArgs.RemoveAll(str => String.IsNullOrEmpty(str));
        }
    }
}

