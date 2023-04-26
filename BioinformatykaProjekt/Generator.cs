using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioinformatykaProjekt
{
    public class Generator
    {
        private char[] Nucleobases =
        {
            'A', 'C', 'G', 'T'
        };

        public char[] Sequence;
        public List<string> Spectrum;

        public void Generate(uint dnaSize, uint oligoSize)
        {
            Sequence = new char[dnaSize];

            Random random = new Random(DateTime.Now.Millisecond);

            for (uint n = 0; n < dnaSize; n++)
            {
                Sequence[n] = Nucleobases[random.Next(0, 4)];
            }

            Spectrum = new List<string>();

            for (uint i = 0; i <= dnaSize - oligoSize; i++)
            {
                string o = "";
                for (uint j = 0; j < oligoSize; j++)
                    o += (Sequence[i + j]);
                if (!Spectrum.Contains(o))
                    Spectrum.Add(o);
            }
        }
    }
}
