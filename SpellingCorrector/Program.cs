﻿using System.Diagnostics;
using SpellingCorrector.CorrectionAlgorithms;

var spell = new SymSpell();

var memSize = GC.GetTotalMemory(true);
var timer = Stopwatch.StartNew();

spell.LoadDictionary("Dictionaries/frequency_dictionary_en_82_765.txt");

timer.Stop();
var memDiff = GC.GetTotalMemory(true) - memSize;
Console.WriteLine($"Build dictionary of {spell.EntriesCount:N0} in {timer.Elapsed.TotalMilliseconds}ms. Memory: {memDiff / 1024.0 / 1024.0:N0}MB");

while (true)
{
    Console.Write("Enter word: ");
    
    var word = Console.ReadLine();
    if (word is null) continue;

    timer.Restart();
    var suggestions = spell.Lookup(word, maxEditDistance: 2, topCount: 3);
    timer.Stop();
    
    foreach (var suggestion in suggestions)
    {
        Console.WriteLine($"{suggestion.Word} - {suggestion.Distance} - {suggestion.Frequency:N0}");
    }
    
    Console.WriteLine($"Elapsed: {timer.Elapsed.TotalMilliseconds:0.000} ms");
}
