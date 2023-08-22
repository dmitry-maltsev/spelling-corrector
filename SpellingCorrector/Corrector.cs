using SpellingCorrector.CorrectionAlgorithms;

namespace SpellingCorrector;

public class Corrector
{
    private const int DefaultEditDistance = 2;
    private const int DefaultTopCount = 3;

    private readonly ICorrectionAlgorithm _correction = new Greedy();

    public int EntriesCount => _correction.EntriesCount;
    
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
            
            _correction.AddEntry(
                word: values[0], 
                frequency: long.Parse(values[1]));
        }
    }

    public List<Suggestion> FindSuggestions(
        string word, 
        int maxEditDistance = DefaultEditDistance,
        int topCount = DefaultTopCount)
    {
        var suggestions = _correction.FindSuggestions(word, maxEditDistance);
        
        return suggestions
            .OrderBy(x => x.Distance)
            .ThenByDescending(x => x.Frequency)
            .Take(topCount)
            .ToList();
    }
}