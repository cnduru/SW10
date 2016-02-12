using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelRewriter
{
    class Declarations
    {
        private string _declarations;

        public string getDeclarations()
        {
            return _declarations;
        }

        public void addDeclaration(string decl)
        {
            _declarations = _declarations + " " + decl;
        }
    }
}
