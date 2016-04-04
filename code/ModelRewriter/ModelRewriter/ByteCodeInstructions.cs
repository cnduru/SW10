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
            instructions.Add(new BytecodeInstruction("00", "nop"));
            instructions.Add(new BytecodeInstruction("15", resolveMnemonic("15")));
            instructions.Add(new BytecodeInstruction("92", resolveMnemonic("92")));
            instructions.Add(new BytecodeInstruction("7B", resolveMnemonic("7B")));
            instructions.Add(new BytecodeInstruction("7C", resolveMnemonic("7C")));
            instructions.Add(new BytecodeInstruction("7D", resolveMnemonic("7D")));
            instructions.Add(new BytecodeInstruction("7E", resolveMnemonic("7E")));
            instructions.Add(new BytecodeInstruction("7EE", resolveMnemonic("7EE"))); // test value
            instructions.Add(new BytecodeInstruction("8F", resolveMnemonic("8F")));
            instructions.Add(new BytecodeInstruction("83", resolveMnemonic("83")));
            instructions.Add(new BytecodeInstruction("8B", resolveMnemonic("8B")));



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
            {
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
                case "74":
                    return "itableswitch";
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
                default:
                    return null;//throw new System.NotImplementedException(opcode);
            }

        }
    }
}
