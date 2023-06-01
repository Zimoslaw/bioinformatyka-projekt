
using BioinformatykaProjekt;
using System.Diagnostics;
using System.Drawing;
using System.Reflection.Emit;

internal static class Program
{
	//Globalne parametry
	static int instances = 100; //Liczba instacji (łańcuchów)
	static int[] seqSize; //Rozmiar łańcucha
	static int[] oligoSize = new int[2]; //Rozmiar oligonukleotydu
	static int[] negatives; //Ilość błędów negatywnych (procent)
	static int[] positives; //Ilość błędów pozytywnych (procent)
	static int[] useCount; //Ile razy jeden oligonukleotyd może być wykorzystany (parametr algorytmu)

	private static void Main(string[] args)
	{
		if(args.Length < 2) { Console.WriteLine("Podaj argumenty"); return; }
		else
		{
			if (args[0] == "a" && args.Length == 6)
			{
				//Zakres rozmiaru łańcucha
				seqSize = new int[2] { 700, 700 };

				//Zakres rozmiaru nukleotydu
				oligoSize[0] = 10;
				oligoSize[1] = 10;

				//Zakres błędów negatywnych
				negatives = new int[2] { 2, 25 };

				//Zakres błędów negatywnych
				positives = new int[2] { 2, 25 };

				//Zakres parametru algorytmu
				try
				{
					useCount = new int[] { int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4]), int.Parse(args[5]) };
				}
				catch(Exception e)
				{
					Console.WriteLine(e.ToString());
					return;
				}
				Console.WriteLine("---- Badanie wpływu parametru algorytmu ----");
			}
			else if (args[0] == "b" && args.Length == 7)
			{
				instances = 20;

				//Stała długość nukleotydu
				oligoSize[0] = 9;

				try
				{
					if (args[1] == "size")
					{
						//Parametry długości łańcucha
						seqSize = new int[5] { int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4]), int.Parse(args[5]), int.Parse(args[6]) };

						//Zakres błędów negatywnych
						negatives = new int[2] { 2, 10 };

						//Zakres błędów negatywnych
						positives = new int[2] { 2, 10 };

						Console.WriteLine("---- Badanie wpływu rozmiaru łańcucha ----");
					}
					else if (args[1] == "neg")
					{
						//Zakres rozmiaru łańcucha
						seqSize = new int[2] { 500, 700 };

						//Parametry ilości błędów negatywnych
						negatives = new int[5] { int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4]), int.Parse(args[5]), int.Parse(args[6]) };

						//Zakres błędów negatywnych
						positives = new int[2] { 2, 10 };

						Console.WriteLine("---- Badanie wpływu ilości błędów negatywnych ----");
					}
					else if (args[1] == "pos")
					{
						//Zakres rozmiaru łańcucha
						seqSize = new int[2] { 500, 700 };

						//Zakres błędów negatywnych
						negatives = new int[2] { 2, 10 };

						//Parametry ilości błędów pozytywnych
						positives = new int[5] {int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4]), int.Parse(args[5]), int.Parse(args[6]) };

						Console.WriteLine("---- Badanie wpływu ilości błędów pozytywnych ----");
					}
					else
						return;
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					return;
				}
			}
			else
				return;
		}

		List<Generator> generators = new List<Generator>();
		List<string> resolvedSequences = new List<string>();

		//Generowanie instancji
		if (args[0] == "a")
		{
			for(int i = 0; i < instances; i++)
			{
				Generator generator = new Generator();
				Random random = new Random(DateTime.Now.Microsecond);

				int s = random.Next(seqSize[0], seqSize[1] + 1);
				int o = random.Next(oligoSize[0], oligoSize[1] + 1);
				int n = random.Next(negatives[0], negatives[1] + 1);
				int p = random.Next(positives[0], positives[1] + 1);

				generator.Generate(s, o, n, p);
				generators.Add(generator);

				Console.WriteLine($"Instancja {i + 1}:\tn= {s}\tk= {o}\tne= {n}%\tpe= {p}%\tstart= {generator.Spectrum.First().Value}");
			}
		}
		else if (args[0] == "b")
		{
			int seed = DateTime.Now.Microsecond;

            for (int param = 0; param < 5; param++)
			{
				for (int i = 0; i < instances; i++)
				{
					Generator generator = new Generator();
					Random random = new Random(DateTime.Now.Microsecond);

					int o = oligoSize[0];
					int s = 1;
					int n = 1;
					int p = 1;

					if (args[1] == "size")
					{
						s = seqSize[param];
						n = random.Next(negatives[0], negatives[1] + 1);
						p = random.Next(positives[0], positives[1] + 1);
					}
					else if (args[1] == "neg")
					{
						s = random.Next(seqSize[0], seqSize[1] + 1);
						n = negatives[param];
						p = random.Next(positives[0], positives[1] + 1);
					}
					else if (args[1] == "pos")
					{
						s = random.Next(seqSize[0], seqSize[1] + 1);
						n = random.Next(negatives[0], negatives[1] + 1);
						p = positives[param];
					}

					generator.Generate(s, o, n, p, seed + i);
					generators.Add(generator);

					Console.WriteLine($"Instancja {i + 1}:\tn= {s}\tk= {o}\tne= {n}%\tpe= {p}%\tstart= {generator.Spectrum.First().Value}");
				}
			}
		}

		//Sekwencjonowanie i tworzenie statystyk
		if (args[0] == "a") //Wpływ parametru algorytmu
		{
			for (int param = 0; param < 5; param++)
			{
				Console.WriteLine($"Dla parametru {useCount[param]}:");

				float averageLDistance = 0; //suma odległosci Levensteina

				
				foreach (Generator generator in generators)
				{
                    //Tworzenie grafu oligonukleotydów
                    foreach (Node node in generator.Spectrum)
					{
						//Czyszczenie połączeń
						node.Used = 0;
						node.NextNodes.Clear();

						//Tworzenie połączeń na podstawie wspólnej części
						for (int size = generator.oligoSize - 1; size > 0; size--)
						{
							foreach (Node next in generator.Spectrum)
							{
								if (next == node || node.NextNodes.ContainsKey(next))
									continue;

								string n1 = node.Value.Substring(node.Value.Length - size);
								string n2 = next.Value.Substring(0, size);
								if (n1 == n2)
								{
									node.NextNodes.Add(next, size); //Waga połączenia to długość wspólnej części
								}
							}
						}
					}

					string newSequence = ""; //Tworzona sekwencja

					NextNode(generator.Spectrum.First(), 0); //Wywołanie rekurencyjnego szukania ściezki zaczynając od pierwszego oligonukleotydu

					void NextNode(Node current, int sub)
					{
						newSequence += current.Value.Substring(sub);
						current.Use();

						//Wybieranie nastepnego wierchołka
						Node next = null;
						int max = 0;

						//Szukanie sąsiada z największą częścią wspólną
						foreach (KeyValuePair<Node, int> pair in current.NextNodes)
						{
							//Następny wierzołek nie może być wczesniej użyty i musi miec odpowiedno dużą część wspólną
							if (pair.Key.Used < 1 && pair.Value > max && pair.Value > oligoSize[0] - useCount[param] - 1)
							{
								next = pair.Key;
								max = pair.Value;
							}
						}

						//Sprawdzanie czy wybrano nastepny wierzchołek i czy długość oryginalnej sekwencji nie została osiągnięta
						if (next != null && newSequence.Length < generator.Sequence.Length)
						{
							NextNode(next, max); //rekurencyjne przechodzenie do nastepnego wierzchołka
						}
					}

					averageLDistance += LevenshteinDistance(generator.Sequence, newSequence);
				}

				Console.WriteLine($"Śr. Odl. Lev.= {Math.Round(averageLDistance / generators.Count, 2)}\n");
			}
		}
		else if (args[0] == "b") //Wpływ parametrów łańcucha
		{
			for (int param = 0; param < 5; param++)
			{
                if (args[1] == "size") { Console.WriteLine($"Dla parametru {seqSize[param]}:");}
                if (args[1] == "neg") { Console.WriteLine($"Dla parametru {negatives[param]}:"); }
                if (args[1] == "pos") { Console.WriteLine($"Dla parametru {positives[param]}:"); }

                float averageLDistance = 0;

                for (int i = 0; i < instances * (param+1); i++)
                {
                    foreach (Node node in generators.ElementAt(i).Spectrum)
                    {
                        node.Used = 0;
                        node.NextNodes.Clear();
                        for (int size = generators.ElementAt(i).oligoSize - 1; size > 0; size--)
                        {
                            foreach (Node next in generators.ElementAt(i).Spectrum)
                            {
                                if (next == node || node.NextNodes.ContainsKey(next))
                                    continue;

                                string n1 = node.Value.Substring(node.Value.Length - size);
                                string n2 = next.Value.Substring(0, size);
                                if (n1 == n2)
                                {
                                    node.NextNodes.Add(next, size);
                                }
                            }
                        }
                    }

                    List<Node> used = new List<Node>();

                    string newSequence = "";

                    NextNode(generators.ElementAt(i).Spectrum.First(), 0);

                    void NextNode(Node current, int sub)
                    {
                        newSequence += current.Value.Substring(sub);
                        current.Use();

                        Node next = null;
                        int max = 0;

                        foreach (KeyValuePair<Node, int> pair in current.NextNodes)
                        {
                            if (pair.Key.Used < 1 && pair.Value > max && pair.Value > oligoSize[0] - 7)
                            {
                                next = pair.Key;
                                max = pair.Value;
                            }
                        }

                        if (next != null && newSequence.Length < generators.ElementAt(i).Sequence.Length)
                        {
                            NextNode(next, max);
                        }
                    }

                    averageLDistance += LevenshteinDistance(generators.ElementAt(i).Sequence, newSequence);
                    //foreach(char c in generator.Sequence) { Console.Write(c); }
                    //Console.WriteLine('\n' + newSequence + '\n');
                }

                Console.WriteLine($"Śr. Odl. Lev.= {Math.Round(averageLDistance / instances, 2)}\n");
            }
        }

		int LevenshteinDistance(char[] s1, string s2)
		{
			int m = s1.Length;
			int n = s2.Length;
			int[,] d = new int[m+1,n+1];

			for (int i = 0; i <= m; d[i, 0] = i++) { }
			for (int j = 0; j <= n; d[0, j] = j++) { }

			for(int i = 1; i <= m; i++)
			{
				for (int j = 1; j <= n; j++)
				{
					int cost = (s2[j - 1] == s1[i - 1]) ? 0 : 1;
					d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
				}
			}
			return d[m, n];
		}

	}
}