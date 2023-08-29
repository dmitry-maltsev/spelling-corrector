namespace SpellingCorrector.SpellingAlgorithms;

public interface ISpellingAlgorithm
{
    IEnumerable<Suggestion> FindSuggestions(string term, int maxEditDistance);
}