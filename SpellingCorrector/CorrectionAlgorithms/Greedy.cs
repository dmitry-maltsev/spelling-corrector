using SpellingCorrector.DistanceAlgorithms;

namespace SpellingCorrector.CorrectionAlgorithms;

public class Greedy : ICorrectionAlgorithm
{
    private readonly Dictionary<string, long> _dictionary = new(100_000);
    
    private readonly IDistance _distanceAlgorithm = new DamerauOSA();
    
    public int EntriesCount => _dictionary.Count;
    
    public void AddEntry(string word, long frequency)
    {
        _dictionary.Add(word, frequency);
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