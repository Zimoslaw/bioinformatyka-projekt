
using BioinformatykaProjekt;
using System.Diagnostics;

Generator gen1= new Generator();

gen1.Generate(20, 6);

Console.WriteLine("Sekwencja:");

foreach (char n in gen1.Sequence)
{
    Console.Write(n);
}

Console.WriteLine("\nSpektrum:");

foreach (string o in gen1.Spectrum)
{
    Console.Write($"[{o}],");
}
