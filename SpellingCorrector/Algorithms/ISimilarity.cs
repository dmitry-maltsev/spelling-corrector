namespace SpellingCorrector.Algorithms;

public interface ISimilarity
{
    double Similarity(string string1, string string2);

    double Similarity(string string1, string string2, double minSimilarity);
}