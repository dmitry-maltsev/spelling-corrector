using SpellingCorrector.DistanceAlgorithms;

namespace SpellingCorrector.Tests;

public class DistanceTests
{
    private readonly IDistance _distanceAlgorithm = new DamerauOSA();
    
    [Theory]
    [InlineData("speling", "spelling", 1)]
    [InlineData("korrectud", "corrected", 2)]
    [InlineData("bycycle", "bicycle", 1)]
    [InlineData("inconvient", "inconvenient", 2)]
    [InlineData("arrainged", "arranged", 1)]
    [InlineData("peotry", "poetry", 1)]
    [InlineData("word", "word", 0)]
    [InlineData("quintessential", "quintessential", 0)]
    [InlineData("pelin", "spelling", -1)]
    [InlineData("qiuntesental", "quintessential", -1)]
    public void ShouldCorrectlyCalculateEditDistance(string word, string candidate, int expectedDistance)
    {
        var actualDistance = _distanceAlgorithm.Distance(word, candidate, maxDistance: 2);
        Assert.Equal(expectedDistance, actualDistance);
    }
}