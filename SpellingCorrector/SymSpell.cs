using SpellingCorrector.Algorithms;

namespace SpellingCorrector;

public record Suggestion(string Word, int Distance, long Frequency);

public class SymSpell
{
    private const int DefaultEditDistance = 2;
    private const int DefaultPrefixLength = 7;
    private const int DefaultInitialCapacity = 64_000;

    private readonly int _maxEditDistance;
    private readonly int _maxPrefixLength;

    private readonly Dictionary<int, string[]> _editsMap = new(DefaultInitialCapacity);
    private readonly Dictionary<string, long> _wordsFrequencies = new (DefaultInitialCapacity);
    
    private readonly IDistance _distanceAlgorithm = new DamerauOSA();

    public SymSpell(
        int maxEditDistance = DefaultEditDistance,
        int maxPrefixLength = DefaultPrefixLength)
    {
        _maxEditDistance = maxEditDistance;
        _maxPrefixLength = maxPrefixLength;
    }
        
    public int EntriesCount => _editsMap.Count;
    public int WordsCount => _wordsFrequencies.Count;

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
        _wordsFrequencies.Add(word, frequency);
        
        var edits = GenerateEdits(word, _maxEditDistance);
                 
        foreach (var edit in edits)
        {
            var hash = GetHash(edit);
            
            if (_editsMap.TryGetValue(hash, out var candidates))
            {
                Array.Resize(ref candidates, candidates.Length + 1);
            }
            else
            {
                candidates = new string[1];
                _editsMap.Add(hash, candidates);
            }
            
            candidates[^1] = word;
        }
    }
    
    private HashSet<string> GenerateEdits(string word, int depth)
    {
        if (_maxPrefixLength > 0 && word.Length > _maxPrefixLength) 
            word = word[.._maxPrefixLength];

        var edits = new HashSet<string> { word };
        
        if (word.Length <= _maxEditDistance)
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
    
    private static int GetHash(string s)
    {
        return s.GetHashCode();
    }
    
    public List<Suggestion> Lookup(string word, int maxEditDistance = DefaultEditDistance, int topCount = 3)
    {
        var words = new HashSet<string>();
        var suggestions = new List<Suggestion>();
        
        var edits = GenerateEdits(word, maxEditDistance);

        foreach (var edit in edits)
        {
            var hash = GetHash(edit);
            if (!_editsMap.TryGetValue(hash, out var candidates)) continue;
            
            foreach (var candidate in candidates)
            {
                if (!words.Add(candidate)) continue;

                var distance = (int)_distanceAlgorithm.Distance(word, candidate, maxEditDistance);
                if (distance < 0) continue;
                
                var frequency = _wordsFrequencies[candidate];
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