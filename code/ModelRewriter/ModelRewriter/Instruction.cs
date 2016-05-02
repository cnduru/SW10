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
            var catchMatch= new Regex("catch start: ([0-9]+);");
            var pcMatch = new Regex("^[0-9]+").Match(code);//.ToString();

            if(pcMatch.Success)
            {
                pc = Convert.ToInt32(pcMatch.ToString());
                instArgs = code.Replace(pcMatch.ToString()+".","").Split(null).ToList();
                instArgs.RemoveAll(str => String.IsNullOrEmpty(str));
            }else
            {
                // for exceptions
                MatchCollection matches = catchMatch.Matches(code);
                instArgs.Add("catch");
                instArgs.Add(matches[0].Groups[1].Value);
            }
        }
    }
}

