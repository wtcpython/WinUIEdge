using System;
using System.Collections.Generic;
using Windows.Storage;

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
                if (!current.Children.ContainsKey(ch))
                {
                    current.Children[ch] = new TrieNode();
                }
                current = current.Children[ch];
            }
            current.IsEndOfWord = true;
        }

        public IEnumerable<string> SearchWords(string prefix)
        {
            TrieNode current = root;

            foreach (var ch in prefix)
            {
                if (!current.Children.ContainsKey(ch))
                {
                    yield break;
                }
                current = current.Children[ch];
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
            LoadWordsAsync(filePath);
        }

        private async void LoadWordsAsync(string filePath)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(filePath));
            var words = await FileIO.ReadLinesAsync(file);

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
