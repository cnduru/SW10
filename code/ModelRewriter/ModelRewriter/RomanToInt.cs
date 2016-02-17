using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelRewriter
{
    public static class RomanToInt
    {
        private enum RomanDigit
        {
            I = 1,
            V = 5,
            X = 10,
            L = 50,
            C = 100,
            D = 500,
            M = 1000
        }

        // Converts a Roman numerals value into an integer
        public static int RomanToNumber(string roman)
        {
            // Rule 7
            roman = roman.ToUpper().Trim();
            if (roman == "N") return 0;

            // Rule 4
            if (roman.Split('V').Length > 2 ||
                roman.Split('L').Length > 2 ||
                roman.Split('D').Length > 2)
                throw new ArgumentException("Rule 4");

            // Rule 1
            int count = 1;
            char last = 'Z';
            foreach (char numeral in roman)
            {
                // Valid character?
                if ("IVXLCDM".IndexOf(numeral) == -1)
                    throw new ArgumentException("Invalid numeral");

                // Duplicate?
                if (numeral == last)
                {
                    count++;
                    if (count == 4)
                        throw new ArgumentException("Rule 1");
                }
                else
                {
                    count = 1;
                    last = numeral;
                }
            }

            // Create an ArrayList containing the values
            int ptr = 0;
            ArrayList values = new ArrayList();
            int maxDigit = 1000;
            while (ptr < roman.Length)
            {
                // Base value of digit
                char numeral = roman[ptr];
                int digit = (int)Enum.Parse(typeof(RomanDigit), numeral.ToString());

                // Rule 3
                if (digit > maxDigit)
                    throw new ArgumentException("Rule 3");

                // Next digit
                int nextDigit = 0;
                if (ptr < roman.Length - 1)
                {
                    char nextNumeral = roman[ptr + 1];
                    nextDigit = (int)Enum.Parse(typeof(RomanDigit), nextNumeral.ToString());

                    if (nextDigit > digit)
                    {
                        if ("IXC".IndexOf(numeral) == -1 ||
                            nextDigit > (digit * 10) ||
                            roman.Split(numeral).Length > 3)
                            throw new ArgumentException("Rule 3");

                        maxDigit = digit - 1;
                        digit = nextDigit - digit;
                        ptr++;
                    }
                }

                values.Add(digit);

                // Next digit
                ptr++;
            }

            // Rule 5
            for (int i = 0; i < values.Count - 1; i++)
                if ((int)values[i] < (int)values[i + 1])
                    throw new ArgumentException("Rule 5");

            // Rule 2
            int total = 0;
            foreach (int digit in values)
                total += digit;

            return total;
        }
    }
}
