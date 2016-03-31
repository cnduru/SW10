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
        public List<BytecodeInstruction> relatedInstructions = new List<BytecodeInstruction>();

        public BytecodeInstruction(string hexi, string mnem = "")
        {
            hex = hexi;
            dec = hexToDec(hexi);
            mnemonic = mnem;
        }

        public void generateRelatedInstructions()
        {
            // 8 bits for a single byte
            for (int i = 0; i < 7; i++)
            {
                var instDec = Convert.ToInt32(dec) ^ Convert.ToInt32(Math.Pow(2, i));
                var bc = new BytecodeInstruction(decToHex(instDec.ToString()), "");
                relatedInstructions.Add(bc);
            }
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
  
            if (mnemonic == "aload")
            {
                labels = new List<Label>()
                {
                    new Label
                    { 
                        content = "osp_inc(), os[osp] = loc0", kind = "assignment", x = 50, y = -50
                    },
                    new Label
                    { 
                        content = "modified to: aload", kind = "comments", x = 100, y = -70
                    }
                };
            }
            else if (mnemonic == "nop")
            {
            }
            else if (mnemonic == "aload")
            {
            }
            else if (mnemonic == "arraylength")
            {
            }
            else if (mnemonic == "getstatic_a")
            {
            }
            else if (mnemonic == "getstatic_b")
            {
            }
            else if (mnemonic == "getstatic_s")
            {
            }
            else if (mnemonic == "getstatic_i")
            {
            }
            else if (mnemonic == "iconst")
            {
            }
            else if (mnemonic == "ifcmpme")
            {
            }
            else if (mnemonic == "invokespecial")
            {
            }
            else if (mnemonic == "invokevirtual")
            {
            }
            else if (mnemonic == "ldc")
            {
            }
            else if (mnemonic == "sload_1")
            {
                labels = new List<Label>()
                {
                    new Label
                    { 
                        content = "osp_inc(), os[osp] = loc0", kind = "assignment", x = 50, y = -50
                    },
                    new Label
                    { 
                        content = "modified to: sload_1", kind = "comments", x = 100, y = -70
                    }
                };
            }
            else if (mnemonic == "return")
            {
            }


            return labels;
        }
    }
}
