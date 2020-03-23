using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Halogen.Core
{
    public class TagSet : IDictionary<string, string>
    {
        private readonly IDictionary<string, string> _tagValues;

        public TagSet()
        {
            _tagValues = new Dictionary<string, string>();
        }

        public TagSet(string rawTag)
        {
            var dict = rawTag.Split(Constants.KeyDelimiter).Select(SplitTag);
            _tagValues = new Dictionary<string, string>(dict);
        }

        public TagSet(IEnumerable<string> tags)
        {
            _tagValues = new Dictionary<string, string>(tags.Select(SplitTag));
        }

        private static KeyValuePair<string, string> SplitTag(string s)
        {
            return new KeyValuePair<string, string>(s.Split(Constants.KeyValueDelimiter).First(),
                s.Split(Constants.KeyValueDelimiter).LastOrDefault() ?? "");
        }

        public override string ToString()
        {
            return string.Join(Constants.KeyDelimiter,this.ToList());
        }

        public IEnumerable<string> ToStringList()
        {
            return this.Select(kv => $"{kv.Key}{Constants.KeyValueDelimiter}{kv.Value}");
        }


        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _tagValues.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _tagValues).GetEnumerator();
        }

        public void Add(KeyValuePair<string, string> item)
        {
            _tagValues.Add(item);
        }

        public void Clear()
        {
            _tagValues.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return _tagValues.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            _tagValues.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return _tagValues.Remove(item);
        }

        public int Count => _tagValues.Count;

        public bool IsReadOnly => _tagValues.IsReadOnly;

        public void Add(string key, string value)
        {
            _tagValues.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _tagValues.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _tagValues.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return _tagValues.TryGetValue(key, out value);
        }

        public string this[string key]
        {
            get => _tagValues[key];
            set => _tagValues[key] = value;
        }

        public ICollection<string> Keys => _tagValues.Keys;

        public ICollection<string> Values => _tagValues.Values;
    }
}