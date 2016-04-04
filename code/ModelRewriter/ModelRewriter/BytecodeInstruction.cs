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
            // flip 3rd bit
            var instDec = Convert.ToInt32(dec) ^ Convert.ToInt32(Math.Pow(2, 3));
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
  
            if (mnemonic == "aload")
            {
                labels = new List<Label>()
                {
                    new Label
                    { 
                        //content = "osp++, os[osp] = loc0", kind = "assignment", x = 50, y = -50
                        content = "osp++", kind = "assignment", x = 50, y = -50
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
            else if (mnemonic == "arraylength")
            {
            }
            else if (mnemonic == "getstatic")
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
            else if (mnemonic.Contains("sload_"))
            {
                // get index
                string index = mnemonic.Substring(mnemonic.IndexOf("_") + 1);

                labels = new List<Label>()
                {
                    new Label
                    { 
                        //content = string.Format("osp++, os[osp] = loc{0}", index), kind = "assignment", x = 50, y = -50
                        content = string.Format("osp++", index), kind = "assignment", x = 50, y = -50
                    },
                    new Label
                    { 
                        content = string.Format("modified to: sload_{0}", index), kind = "comments", x = 100, y = -70
                    }
                };
            }
            else if (mnemonic == "sconst_5") // fix this one to make general
            {
                labels = new List<Label>()
                {
                    new Label
                    { 
                        //content = "osp++, os[osp] = loc0", kind = "assignment", x = 50, y = -50
                        content = "osp++", kind = "assignment", x = 50, y = -50
                    },
                    new Label
                    { 
                        content = "modified to: sload_1", kind = "comments", x = 100, y = -70
                    }
                };
            }
            else if (mnemonic == "iflt_w")
            {
                labels = new List<Label>()
                {
                    new Label
                    { 
                        //content = "os[osp] = 0, osp--", kind = "assignment", x = 50, y = -50
                        content = "osp--", kind = "assignment", x = 50, y = -50
                    },
                    new Label
                    { 
                        content = "modified to: ifltw", kind = "comments", x = 100, y = -70
                    }
                };
            }
            else if (mnemonic == "stableswitch")
            {
                labels = new List<Label>()
                {
                    new Label
                    { 
                        //content = "os[osp] = 0, osp--", kind = "assignment", x = 50, y = -50
                        content = "osp--", kind = "assignment", x = 50, y = -50
                    },
                    new Label
                    { 
                        content = "modified to: stableswitch", kind = "comments", x = 100, y = -70
                    }
                };
            }
            else if (mnemonic == "itableswitch")
            {
                labels = new List<Label>()
                {
                    new Label
                    { 
                        //content = "os[osp] = 0, osp--", kind = "assignment", x = 50, y = -50
                        content = "osp--", kind = "assignment", x = 50, y = -50
                    },
                    new Label
                    { 
                        content = "modified to: itableswitch", kind = "comments", x = 100, y = -70
                    }
                };
            }
            else if (mnemonic == "slookupswitch")
            {
                labels = new List<Label>()
                {
                    new Label
                    { 
                        //content = "os[osp] = 0, osp--", kind = "assignment", x = 50, y = -50
                        content = "osp--", kind = "assignment", x = 50, y = -50
                    },
                    new Label
                    { 
                        content = "modified to: slookupswitch", kind = "comments", x = 100, y = -70
                    }
                };
            }
            else if (mnemonic == "ilookupswitch")
            {
                labels = new List<Label>()
                {
                    new Label
                    { 
                        //content = "os[osp] = 0, osp--", kind = "assignment", x = 50, y = -50
                        content = "osp--", kind = "assignment", x = 50, y = -50
                    },
                    new Label
                    { 
                        content = "modified to: ilookupswitch", kind = "comments", x = 100, y = -70
                    }
                };
            }
            else if (mnemonic == "getfield_a")
            {
                labels = new List<Label>()
                {
                    new Label
                    { 
                        content = "modified to: getfield_a", kind = "comments", x = 100, y = -70
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
