using SpellingCorrector.DistanceAlgorithms;

namespace SpellingCorrector.SpellingAlgorithms;

public class SymSpell : ISpellingAlgorithm
{
    private const int DefaultEditDistance = 2;
    private const int DefaultPrefixLength = -1;
    private const int DefaultInitialCapacity = 3_842_500;

    private readonly int _maxEditDistance;
    private readonly int _maxPrefixLength;

    private readonly Dictionary<int, string[]> _editsMap = new(DefaultInitialCapacity);
    private readonly Dictionary<string, long> _dictionary = new (DefaultInitialCapacity);
    
    private readonly IDistance _distanceAlgorithm = new DamerauOSA();

    public SymSpell(
        int maxEditDistance = DefaultEditDistance,
        int maxPrefixLength = DefaultPrefixLength)
    {
        _maxEditDistance = maxEditDistance;
        _maxPrefixLength = maxPrefixLength;
    }
        
    public int EntriesCount => _editsMap.Count;
    
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
            AddEntry(word: values[0], frequency: long.Parse(values[1]));
        }
    }

    private void AddEntry(string word, long frequency)
    {
        _dictionary.Add(word, frequency);
        
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
    
    public IEnumerable<Suggestion> FindSuggestions(string term, int maxEditDistance)
    {
        if (maxEditDistance > _maxEditDistance)
            throw new ArgumentOutOfRangeException(
                nameof(maxEditDistance), 
                $"{nameof(maxEditDistance)} should be less or equal to {_maxEditDistance}");
        
        if (_dictionary.TryGetValue(term, out var wordFrequency))
            yield return new Suggestion(term, 0, wordFrequency);
        
        if (maxEditDistance == 0) yield break;
        
        var seenWords = new HashSet<string> { term };
        var edits = GenerateEdits(term, maxEditDistance);

        foreach (var edit in edits)
        {
            var hash = GetHash(edit);
            if (!_editsMap.TryGetValue(hash, out var candidates)) continue;
            
            foreach (var candidate in candidates)
            {
                if (!seenWords.Add(candidate)) continue;
                if (Math.Abs(candidate.Length - term.Length) > maxEditDistance) continue;

                var distance = (int)_distanceAlgorithm.Distance(term, candidate, maxEditDistance);
                if (distance < 0) continue;
                
                var frequency = _dictionary[candidate];
                yield return new Suggestion(candidate, distance, frequency);
            }
        }
    }
}