using SpellingCorrector.Algorithms;

namespace SpellingCorrector;

public record Suggestion(string Word, int Distance, int Frequency);

public class SymSpell
{
    private const int MaxEditDistance = 2;
    private const int MaxPrefixLength = 7;
    
    private readonly Dictionary<string, string[]> _dictionary = new(3_850_000);
    private readonly Dictionary<string, int> _frequencies = new(100_000);
    private readonly IDistance _distanceAlgorithm = new DamerauOSA();

    public int Count => _dictionary.Count;

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
            AddWord(word: values[0], frequency: int.Parse(values[1]));
        }
    }

    private void AddWord(string word, int frequency)
    {
        _frequencies.Add(word, frequency);
        
        var edits = GenerateEdits(word, MaxEditDistance);
                 
        foreach (var edit in edits)
        {
            if (_dictionary.TryGetValue(edit, out var candidates))
            {
                Array.Resize(ref candidates, candidates.Length + 1);
            }
            else
            {
                candidates = new string[1];
                _dictionary.Add(edit, candidates);
            }
            
            candidates[^1] = word;
        }
    }
    
    private static HashSet<string> GenerateEdits(string word, int depth)
    {
        // if (word.Length > MaxPrefixLength) 
        //     word = word[..MaxPrefixLength];

        var edits = new HashSet<string> { word };
        if (word.Length <= MaxEditDistance)
            edits.Add(string.Empty);
        
        GenerateEdits(word, depth, edits);
        
        return edits;
    }

    private static void GenerateEdits(string word, int depth, ISet<string> edits)
    {
        if (depth == 0) return;

        for (var i = 0; i < word.Length; i++)
        {
            var edit = word.Remove(i, 1);
            if (edits.Add(edit))
                GenerateEdits(edit, depth - 1, edits);
        }
    }
    
    public List<Suggestion> Lookup(string word, int maxEditDistance = MaxEditDistance, int topCount = 3)
    {
        var words = new HashSet<string>();
        var suggestions = new List<Suggestion>();
        
        var edits = GenerateEdits(word, maxEditDistance);

        foreach (var edit in edits)
        {
            if (!_dictionary.TryGetValue(edit, out var candidates)) continue;
            
            foreach (var candidate in candidates)
            {
                if (!words.Add(candidate)) continue;
                
                var distance = (int)_distanceAlgorithm.Distance(word, candidate, MaxEditDistance);
                if (distance < 0) continue;
                
                var frequency = _frequencies[candidate];
                suggestions.Add(new Suggestion(candidate, distance, frequency));
            }
        }

        return suggestions
            .OrderBy(x => x.Distance)
            .ThenByDescending(x => x.Frequency)
            .Take(topCount)
            .ToList();
    }
}