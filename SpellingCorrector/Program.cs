using System.Diagnostics;
using SpellingCorrector;

var corrector = new Corrector();

var memSize = GC.GetTotalMemory(true);
var timer = Stopwatch.StartNew();

corrector.LoadDictionary("Dictionaries/ru-100k.txt");

timer.Stop();
var memDiff = GC.GetTotalMemory(true) - memSize;
Console.WriteLine($"Build dictionary of {corrector.EntriesCount:N0} in {timer.Elapsed.TotalMilliseconds}ms. Memory: {memDiff / 1024.0 / 1024.0:N0}MB");

while (true)
{
    Console.Write("Enter word: ");
    
    var word = Console.ReadLine();
    if (word is null) continue;

    timer.Restart();
    var suggestions = corrector.FindSuggestions(word, topCount:3);
    timer.Stop();
    
    foreach (var suggestion in suggestions)
    {
        Console.WriteLine($"{suggestion.Word} - {suggestion.Distance} - {suggestion.Frequency:N0}");
    }
    
    Console.WriteLine($"Elapsed: {timer.Elapsed.TotalMilliseconds:0.000} ms");
}