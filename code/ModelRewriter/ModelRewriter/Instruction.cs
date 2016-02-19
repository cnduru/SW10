using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace ModelRewriter
{
    public class Instruction
    {
        public int pc;
        public List<string> instWithArgs = new List<string>(); 

        public Instruction(string code)
        {
            var pcStr = new Regex("^[0-9]+").Match(code).ToString();
            pc = Convert.ToInt32(pcStr);
            instWithArgs = code.Replace(pcStr+".","").Split(null).ToList();
            instWithArgs.RemoveAll(str => String.IsNullOrEmpty(str));
        }
    }
}

