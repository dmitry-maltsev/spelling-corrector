using System.Diagnostics;
using SpellingCorrector.SpellingAlgorithms;

var spell = new LinSpell();

var memSize = GC.GetTotalMemory(true);
var timer = Stopwatch.StartNew();

spell.LoadDictionary("Dictionaries/ru-100k.txt");

timer.Stop();
var memDiff = GC.GetTotalMemory(true) - memSize;
Console.WriteLine($"Build dictionary of {spell.EntriesCount:N0} in {timer.Elapsed.TotalMilliseconds}ms. Memory: {memDiff / 1024.0 / 1024.0:N0}MB");

while (true)
{
    Console.Write("Enter word: ");
    
    var word = Console.ReadLine();
    if (word is null) continue;

    timer.Restart();
    
    var suggestions = spell.FindSuggestions(word, maxEditDistance: 2);
    var topSuggestions = suggestions
        .OrderBy(x => x.Distance)
        .ThenByDescending(x => x.Frequency)
        .Take(5)
        .ToList();
    
    timer.Stop();
    
    foreach (var suggestion in topSuggestions)
    {
        Console.WriteLine($"{suggestion.Word} - {suggestion.Distance} - {suggestion.Frequency:N0}");
    }
    
    Console.WriteLine($"Elapsed: {timer.Elapsed.TotalMilliseconds:0.000} ms");
}
