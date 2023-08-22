namespace SpellingCorrector.CorrectionAlgorithms;

public record Suggestion(string Word, int Distance, long Frequency);

public interface ICorrectionAlgorithm
{
    void AddEntry(string word, long frequency);
    
    int EntriesCount { get; }

    IEnumerable<Suggestion> FindSuggestions(string word, int maxEditDistance);
}