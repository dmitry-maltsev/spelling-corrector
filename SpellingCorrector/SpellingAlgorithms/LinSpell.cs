using SpellingCorrector.DistanceAlgorithms;

namespace SpellingCorrector.SpellingAlgorithms;

public class LinSpell : ISpellingAlgorithm
{
    private readonly Dictionary<string, long> _dictionary;
    private readonly IDistance _distanceAlgorithm;

    public LinSpell(int initialCapacity, IDistance distanceAlgorithm)
    {
        _dictionary = new Dictionary<string, long>(initialCapacity);
        _distanceAlgorithm = distanceAlgorithm;
    }

    public LinSpell() : this(100_000, new DamerauOSA())
    {
    }
    
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
    
    public IEnumerable<Suggestion> FindSuggestions(string word, int maxEditDistance)
    {
        foreach (var (candidate, frequency) in _dictionary)
        {
            var distance = (int)_distanceAlgorithm.Distance(word, candidate, maxEditDistance);
            if (distance < 0) continue;

            yield return new Suggestion(candidate, distance, frequency);
        }
    }
}