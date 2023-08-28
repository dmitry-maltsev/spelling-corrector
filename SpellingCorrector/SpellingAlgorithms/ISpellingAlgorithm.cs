namespace SpellingCorrector.SpellingAlgorithms;

public interface ISpellingAlgorithm
{
    IEnumerable<Suggestion> FindSuggestions(string word, int maxEditDistance);
}