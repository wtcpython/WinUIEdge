using System.Collections.Generic;
using System.IO;

namespace Edge
{
    // 前缀树节点
    public class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; set; }

        public bool IsEndOfWord { get; set; }

        public TrieNode()
        {
            Children = [];
            IsEndOfWord = false;
        }
    }

    // 前缀树
    public class Trie
    {
        private TrieNode root;

        public Trie()
        {
            root = new TrieNode();
        }

        // 插入节点
        public void Insert(string word)
        {
            TrieNode current = root;
            foreach (var ch in word)
            {
                if (!current.Children.TryGetValue(ch, out TrieNode value))
                {
                    value = new TrieNode();
                    current.Children[ch] = value;
                }
                current = value;
            }
            current.IsEndOfWord = true;
        }

        public IEnumerable<string> SearchWords(string prefix)
        {
            TrieNode current = root;

            foreach (var ch in prefix)
            {
                if (!current.Children.TryGetValue(ch, out TrieNode value))
                {
                    yield break;
                }
                current = value;
            }

            foreach(var word in FindWordsFromNode(current, prefix))
            {
                yield return word;
            }
        }

        private IEnumerable<string> FindWordsFromNode(TrieNode node, string currentWord)
        {
            if (node.IsEndOfWord)
            {
                yield return currentWord;
            }

            foreach (var child in node.Children)
            {
                foreach (var word in FindWordsFromNode(child.Value, currentWord + child.Key))
                {
                    yield return word;
                }
            }
        }
    }

    public class WordSearchEngine
    {
        private Trie trie;

        public WordSearchEngine(string filePath)
        {
            trie = new Trie();
            LoadWords(filePath);
        }

        private void LoadWords(string filePath)
        {
            var words = File.ReadAllLines(filePath);

            foreach (var word in words)
            {
                trie.Insert(word.Trim());
            }
        }

        public IEnumerable<string> SearchWords(string prefix)
        {
            return trie.SearchWords(prefix);
        }
    }
}
