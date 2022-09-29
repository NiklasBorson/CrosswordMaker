using System;
using System.Collections;
using System.Collections.Generic;

namespace CrosswordMaker
{
    internal class ClueMap : IEnumerable<KeyValuePair<string, string>>
    {
        Dictionary<string, string> m_map = new Dictionary<string, string>();

        public bool IsDirty
        {
            get;
            set;
        }

        public void AddClue(string word, string clue)
        {
            if (m_map.ContainsKey(word))
            {
                clue = $"{m_map[word]}\n{clue}";
                m_map[word] = clue;
                IsDirty = true;
            }
        }

        public void RemoveClue(string word, string clue)
        {
            string value = null;

            foreach (string s in GetClues(word))
            {
                if (s != clue)
                {
                    value = (value != null) ? $"{value}\n{s}" : s;
                }
            }

            if (value != null)
            {
                m_map[word] = value;
            }
            else
            {
                m_map.Remove(word);
            }

            IsDirty = true;
        }

        public string[] GetClues(string word)
        {
            if (m_map.ContainsKey(word))
            {
                return m_map[word].Split('\n');
            }
            else
            {
                return new string[0];
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return m_map.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_map.GetEnumerator();
        }
    }
}
