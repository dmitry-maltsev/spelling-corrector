using System.Diagnostics;
using SpellingCorrector;
using SpellingCorrector.CorrectionAlgorithms;

ICorrectionAlgorithm corrector = new Greedy();

var memSize = GC.GetTotalMemory(true);
var timer = Stopwatch.StartNew();

LoadDictionary("Dictionaries/ru-100k.txt");

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

void LoadDictionary(string filePath)
{
    if (!File.Exists(filePath))
    {
        throw new FileNotFoundException("The file path does not exist.");
    }

    using var reader = new StreamReader(filePath);
        
    while (reader.ReadLine() is { } line)
    {
        var values = line.Split();
        corrector.AddEntry(word: values[0], frequency: long.Parse(values[1]));
    }
}