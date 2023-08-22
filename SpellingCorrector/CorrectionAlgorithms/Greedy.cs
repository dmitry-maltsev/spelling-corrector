using SpellingCorrector.DistanceAlgorithms;

namespace SpellingCorrector.CorrectionAlgorithms;

public class Greedy
{
    private readonly Dictionary<string, long> _dictionary = new(100_000);
    
    private readonly IDistance _distanceAlgorithm = new DamerauOSA();
    
    public int EntriesCount => _dictionary.Count;
    
    public void LoadDictionary(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The file path does not exist.");
        }

        using var reader = new StreamReader(filePath);
        
        while (reader.ReadLine() is { } line)
        {
            var values = line.Split();
            _dictionary.Add(values[0], long.Parse(values[1]));    
        }
    }
    
    private IEnumerable<Suggestion> FindSuggestions(string word, int maxEditDistance)
    {
        foreach (var (candidate, frequency) in _dictionary)
        {
            var distance = (int)_distanceAlgorithm.Distance(word, candidate, maxEditDistance);
            if (distance < 0) continue;

            yield return new Suggestion(candidate, distance, frequency);
        }
    }
    
    public List<Suggestion> Lookup(string word, int maxEditDistance, int topCount)
    {
        var suggestions = FindSuggestions(word, maxEditDistance);
        
        return suggestions
            .OrderBy(x => x.Distance)
            .ThenByDescending(x => x.Frequency)
            .Take(topCount)
            .ToList();
    }
}