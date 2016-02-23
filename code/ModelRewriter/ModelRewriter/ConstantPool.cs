using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace ModelRewriter
{
    public class ConstantPool : IEnumerable
    {
        private Dictionary<string, int> _cp = new Dictionary<string, int>();

        public ConstantPool()
        {
        }

        public int Add(string value)
        {
            if (!_cp.ContainsKey(value))
            {
                _cp[value] = _cp.Count();
            }
            return  _cp[value];
        }

        public IEnumerator GetEnumerator()
        {
            return _cp.GetEnumerator();
        }
    }
}

