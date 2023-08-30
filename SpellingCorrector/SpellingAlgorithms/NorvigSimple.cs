using SpellingCorrector.DistanceAlgorithms;

namespace SpellingCorrector.SpellingAlgorithms;

public class NorgivSimple : ISpellingAlgorithm
{
    private const string Alphabet = "абвгдежзийклмнопрстуфхцчшщъыьэюя";

    private readonly Dictionary<string, long> _dictionary;
    private readonly IDistance _distanceAlgorithm;

    public NorgivSimple(int initialCapacity, IDistance distanceAlgorithm)
    {
        _dictionary = new Dictionary<string, long>(initialCapacity);
        _distanceAlgorithm = distanceAlgorithm;
    }

    public NorgivSimple() : this(100_000, new DamerauOSA())
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

    private static IEnumerable<string> GenerateEdits(string word)
    {
        var deletes = 
            from i in Enumerable.Range(0, word.Length)
            select word[..i] + word[(i + 1)..];

        var transposes =
            from i in Enumerable.Range(0, word.Length - 1)
            select string.Concat(
                word.AsSpan(0, i),
                word.AsSpan(i + 1, 1),
                word.AsSpan(i, 1),
                word.AsSpan(i + 2));

        var replaces = 
            from i in Enumerable.Range(0, word.Length)
            from c in Alphabet
            select word[..i] + c + word[(i + 1)..];

        var inserts = 
            from i in Enumerable.Range(0, word.Length + 1)
            from c in Alphabet
            select word[..i] + c + word[i..];
        
        return deletes
            .Union(transposes)
            .Union(replaces)
            .Union(inserts);
    }
    
    public IEnumerable<Suggestion> FindSuggestions(string term, int maxEditDistance)
    {
        var seenWords = new HashSet<string> { term };

        if (_dictionary.TryGetValue(term, out var wordFrequency))
            yield return new Suggestion(term, 0, wordFrequency);
        
        if (maxEditDistance == 0) yield break;
        
        foreach (var edit1 in GenerateEdits(term))
        {
            if (!seenWords.Add(edit1)) continue;
            if (_dictionary.TryGetValue(edit1, out var frequency1))
                yield return new Suggestion(edit1, 1, frequency1);
        }
        
        foreach (var edit1 in GenerateEdits(term))
        {
            foreach (var edit2 in GenerateEdits(edit1))
            {
                if (!seenWords.Add(edit2)) continue;
                if (_dictionary.TryGetValue(edit2, out var frequency2))
                    yield return new Suggestion(edit2, 2, frequency2);
            }
        }
    }
}