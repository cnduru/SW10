using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelRewriter
{    
    class ByteCodeInstructions
    {
        public List<BytecodeInstruction> instructions = new List<BytecodeInstruction>();

        public ByteCodeInstructions()
        {
            // initialize instructions
            /*instructions.Add(new BytecodeInstruction("00", "nop"));
            instructions.Add(new BytecodeInstruction("15", resolveMnemonic("15")));
            instructions.Add(new BytecodeInstruction("92", resolveMnemonic("92")));
            instructions.Add(new BytecodeInstruction("7B", resolveMnemonic("7B")));
            instructions.Add(new BytecodeInstruction("7C", resolveMnemonic("7C")));
            instructions.Add(new BytecodeInstruction("7D", resolveMnemonic("7D")));
            instructions.Add(new BytecodeInstruction("7E", resolveMnemonic("7E")));
            instructions.Add(new BytecodeInstruction("7EE", resolveMnemonic("7EE"))); // test value
            instructions.Add(new BytecodeInstruction("8F", resolveMnemonic("8F")));
            instructions.Add(new BytecodeInstruction("83", resolveMnemonic("83")));
            instructions.Add(new BytecodeInstruction("8B", resolveMnemonic("8B")));*/

            instructions.Add(new BytecodeInstruction("60", resolveMnemonic("60")));
            instructions.Add(new BytecodeInstruction("70", resolveMnemonic("70")));
            instructions.Add(new BytecodeInstruction("20", resolveMnemonic("20")));
            instructions.Add(new BytecodeInstruction("30", resolveMnemonic("30")));
            instructions.Add(new BytecodeInstruction("B", resolveMnemonic("B")));
            instructions.Add(new BytecodeInstruction("1B", resolveMnemonic("1B")));
            instructions.Add(new BytecodeInstruction("C", resolveMnemonic("C")));
            instructions.Add(new BytecodeInstruction("1C", resolveMnemonic("1C")));
            instructions.Add(new BytecodeInstruction("79", resolveMnemonic("79")));
            instructions.Add(new BytecodeInstruction("69", resolveMnemonic("69")));

            instructions.ForEach(e => e.generateRelatedInstructions());
            instructions.ForEach(e => e.mnemonic = resolveMnemonic(e.hex));

            foreach (var item in instructions)
            {
                var rel = item.relatedInstruction;

                if (resolveMnemonic(rel.hex) != null)
                {
                    rel.mnemonic = resolveMnemonic(rel.hex);
                    Console.WriteLine(string.Format("{0} -> {1}", item.mnemonic, rel.mnemonic));
                }
            }
        }

        public BytecodeInstruction instructionToBytecode(string mnemonic)
        {
            try
            {
                return instructions.First(i => i.mnemonic == mnemonic);
            } catch (Exception ex)
            {
                return null;
            }
        }

        public string resolveMnemonic(string opcode)
        {
            // fix opcode irregularities in case
            switch (opcode)
            {/*
                case "00":
                    return "nop";
                case "15":
                    return "aload";
                case "92":
                    return "arraylength";
                case "7EE":                             // test value
                    return "getstatic";
                case "7B":
                    return "getstatic_a";
                case "7C":
                    return "getstatic_b";
                case "7D":
                    return "getstatic_s";
                case "7E":
                    return "getstatic_i";
                case "8":
                    return "sconst_5";
                case "1D":
                    return "sload_1";
                case "9A":
                    return "iflt_w";
                case "73":
                    return "stableswitch";

                case "75":
                    return "slookupswitch";
                case "76":
                    return "ilookupswitch";
                case "iconst":
                    return null;
                case "ifcmpme":
                    throw new System.NotImplementedException(opcode);
                case "invokespecial":
                    return "8C";
                case "83":
                    return "getfield_a";
                case "8F":
                    return "new";
                case "8B":
                    return "invokevirtual";
                case "ldc":
                    throw new System.NotImplementedException(opcode);
                case "return":
                    return "7A";
                case "14":
                    return "iipush";
                */
                // original mnemonics
                case "60":
                    return "ifeq";
                case "20":
                    return "iload_0";
                case "B":
                    return "iconst_1";
                case "C":
                    return "iconst_2";
                case "79":
                    return "ireturn";

                // fault mnemonics
                case "70":                //0x60 -> 0x70
                    return "goto";
                case "30":                //0x20 -> 0x30
                    return "sstore_1";
                case "1B":                //0xb -> 0x1b
                    return "aload_3";
                case "1C":                //0xc -> 0x1c
                    return "sload_0";
                case "69":                //0x79 -> 0x69
                    return "if_acmpne";
                default:
                    return null;//throw new System.NotImplementedException(opcode);
            }

        }
    }
}
