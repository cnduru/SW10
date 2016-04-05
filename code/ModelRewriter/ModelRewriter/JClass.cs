using System;
using System.IO;
using System.Collections.Generic;

namespace ModelRewriter
{
    public class JClass
    {
        JClass super;

        public JClass(string name, string filePath)
        {
            var lines = File.ReadAllText(filePath).Split('\n');
            /*super = 
            var classSig = lines[0];
            var methods = findMethods(lines.Skip(1));
            fields[cls] = findFields(lines.Skip(1));*/
        }
    }
}

