namespace SpellingCorrector.SpellingAlgorithms;

class TrieNode
{
    public Dictionary<char, TrieNode> Children { get; } = new();
    
    public bool IsEndOfWord { get; set; }
}

class Trie
{
    private readonly TrieNode _root = new();

    public void Insert(string word)
    {
        var node = _root;
        
        foreach (var c in word)
        {
            node.Children.TryAdd(c, new TrieNode());
            node = node.Children[c];
        }
        
        node.IsEndOfWord = true;
    }

    public List<Suggestion> FindSuggestions(string word, int maxDistance)
    {
        var suggestions = new List<Suggestion>();
        FindSuggestions(_root, word, maxDistance, "", suggestions);
        return suggestions;
    }

    private void FindSuggestions(
        TrieNode node, 
        string word, 
        int distanceLeft, 
        string candidate, 
        List<Suggestion> suggestions)
    {
        if (distanceLeft < 0) return;

        if (word.Length == 0)
        {
            if (node.IsEndOfWord)
                suggestions.Add(new Suggestion(candidate, 2 - distanceLeft, 0));
            
            return;
        }
        
        foreach (var (c, child) in node.Children)
        {
            // Matching character
            if (c == word[0])
            {
                FindSuggestions(child, word[1..], distanceLeft, candidate + c, suggestions);
                continue;
            }

            // Deletion
            // FindSuggestions(child, word, distanceLeft - 1, candidate, suggestions);

            // Substitution
            FindSuggestions(child, word[1..], distanceLeft - 1, candidate + c, suggestions);

            // Transposition
            if (word.Length >= 2)
            {
                // FindSuggestions(child, word[1] + word[0] + word[2..], distanceLeft - 1, candidate + c, suggestions);
            }
            
            // Insertion
            // FindSuggestions(child, word, distanceLeft - 1, candidate + c, suggestions);
        }
    }
}

class PrefixTree
{
    private readonly Trie _dictionaryTrie = new Trie();
    
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
            var word = values[0]; 
            var frequency = long.Parse(values[1]);
            
            _dictionaryTrie.Insert(word);
        }
    }

    public List<Suggestion> Lookup(string input, int maxEditDistance, int topCount)
    {
        var suggestions = _dictionaryTrie.FindSuggestions(input, maxEditDistance);
        
        return suggestions
            .OrderBy(x => x.Distance)
            //.ThenByDescending(x => x.Frequency)
            .Take(topCount)
            .ToList();
    }
}