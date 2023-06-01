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
		public int oligoSize;
		int realNegatives;
		int realPositives;
		public List<Node> Spectrum;

		public void Generate(int dnaSize, int oligoSize, int negatives, int positives, int seed = 0)
		{
			this.oligoSize = oligoSize;
			realNegatives = (int)((float)dnaSize * ((float)negatives / 100));
			realPositives = (int)((float)dnaSize * ((float)positives / 100));
			Sequence = new char[dnaSize];

			//Generowanie sekwencji
			Random random;

            if (seed == 0) { random = new Random(DateTime.Now.Microsecond); }
			else { random = new Random(seed); }

			for (int n = 0; n < dnaSize; n++)
			{
				Sequence[n] = Nucleobases[random.Next(0, 4)];
			}

			//Tworzenie spektrum z sekwencji
			Spectrum = new List<Node>();

			for (int i = 0; i <= dnaSize - oligoSize; i++)
			{
				string o = "";

				for (int j = 0; j < oligoSize; j++)
					o += (Sequence[i + j]);

				Node node = new Node(o);
				Spectrum.Add(node);

			}

			//Dodawanie błędów negatywnych
			for(int i = 0; i < realNegatives; i++)
				Spectrum.RemoveAt(random.Next(0, Spectrum.Count));

			//Dodawanie błędów pozytywnych
			while(realPositives > 0)
			{
				string newNode = "";

				for (int i = 0; i < oligoSize; i++)
					newNode += Nucleobases[random.Next(0, 4)];

				Node node = new Node(newNode);
				Spectrum.Add(node);
				realPositives--;
			}
		}
	}
}
