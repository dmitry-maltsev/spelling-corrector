namespace SpellingCorrector.CorrectionAlgorithms;

public record Suggestion(string Word, int Distance, long Frequency);

public interface ICorrectionAlgorithm
{
    const int DefaultEditDistance = 2;
    const int DefaultTopCount = 3;
    
    void AddEntry(string word, long frequency);
    
    int EntriesCount { get; }

    List<Suggestion> FindSuggestions(string word, int maxEditDistance = DefaultEditDistance, int topCount = DefaultTopCount);
}