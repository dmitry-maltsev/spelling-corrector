using SpellingCorrector.Algorithms;

namespace SpellingCorrector;

public class Greedy
{
    private const int DefaultEditDistance = 2;

    private readonly Dictionary<string, long> _dictionary = new(100_000);
    
    private readonly IDistance _distanceAlgorithm = new DamerauOSA();
    
    public int EntriesCount => _dictionary.Count;
    
    public void CreateDictionaryFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The file path does not exist.");
        }

        using var reader = new StreamReader(filePath);
        
        while (reader.ReadLine() is { } line)
        {
            var values = line.Split();
            _dictionary.Add(values[0], int.Parse(values[1]));
        }
    }

    public List<Suggestion> Lookup(string word, int maxEditDistance = DefaultEditDistance, int topCount = 3)
    {
        var suggestions = new List<Suggestion>();

        foreach (var entry in _dictionary)
        {
            var candidate = entry.Key;
            var frequency = entry.Value;
            
            var distance = (int)_distanceAlgorithm.Distance(word, candidate, maxEditDistance);
            if (distance < 0) continue;

            suggestions.Add(new Suggestion(candidate, distance, frequency));
        }
        
        return suggestions
            .OrderBy(x => x.Distance)
            .ThenByDescending(x => x.Frequency)
            .Take(topCount)
            .ToList();
    }
}