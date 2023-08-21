using System.Diagnostics;
using SpellingCorrector;

var symSpell = new SymSpell();

var memSize = GC.GetTotalMemory(true);
var timer = Stopwatch.StartNew();

symSpell.CreateDictionaryFromFile("Dictionaries/ru-100k.txt");

timer.Stop();
var memDiff = GC.GetTotalMemory(true) - memSize;
Console.WriteLine($"Build dictionary of {symSpell.Count:N0} in {timer.Elapsed.TotalMilliseconds}ms. Memory: {memDiff / 1024.0 / 1024.0:N0}MB");

while (true)
{
    Console.Write("Enter word: ");
    
    var word = Console.ReadLine();
    if (word is null) continue;

    timer.Restart();
    var suggestions = symSpell.Lookup(word, topCount:3);
    timer.Stop();

    Console.WriteLine($"Elapse: {timer.Elapsed.TotalMilliseconds:0.000} ms");

    foreach (var suggestion in suggestions)
    {
        Console.WriteLine($"{suggestion.Word} - {suggestion.Distance} - {suggestion.Frequency:N0}");
    }
}