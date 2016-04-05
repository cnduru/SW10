using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelRewriter
{
    class BytecodeInstruction
    {
        public string dec, mnemonic, hex;
        //public List<BytecodeInstruction> relatedInstructions = new List<BytecodeInstruction>();
        public BytecodeInstruction relatedInstruction;

        public BytecodeInstruction(string hexi, string mnem = "")
        {
            hex = hexi;
            dec = hexToDec(hexi);
            mnemonic = mnem;
        }

        public void generateRelatedInstructions()
        {
            // flip 5th bit. Zero indexed
            var instDec = Convert.ToInt32(dec) ^ Convert.ToInt32(Math.Pow(2, 4));
            relatedInstruction = new BytecodeInstruction(decToHex(instDec.ToString()), "");
        }

        private string decToHex(string dec)
        {
            return Convert.ToInt32(dec).ToString("X");
        }

        private string hexToDec(string hex)
        {
            return Convert.ToInt64(hex, 16).ToString();
        }

        public List<Label> getLabels()
        {
            var labels = new List<Label>();
            
            if (mnemonic == "goto")
            {
                labels = new List<Label>()
                {
                    new Label
                    { 
                        content = "osp++", kind = "assignment", x = 50, y = -50
                    },
                    new Label
                    { 
                        content = string.Format("modified to: {0}", relatedInstruction.mnemonic), kind = "comments", x = 100, y = -70
                    }
                };
            }
           


            return labels;
        }
    }
}
