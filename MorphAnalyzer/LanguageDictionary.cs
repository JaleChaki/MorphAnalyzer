using System.Runtime.CompilerServices;
using DawgSharp;
using MorphAnalyzer.AnalyzerUnits;

#if DEBUG
[assembly: InternalsVisibleTo("MorphAnalyzer.Tests")]
#endif
namespace MorphAnalyzer {
    
    internal class LanguageDictionary {

        public IReadOnlyList<string> Hyphens { get; }
        public IReadOnlyList<string> KnownPrefixes { get; }
        private IReadOnlyList<string> Suffixes { get; }
        private IReadOnlyList<Paradigm> Paradigms { get; }
        private IReadOnlyList<string> ParadigmPrefixes { get; }
        private IReadOnlyList<WordTag> Tags { get; }
        private Dawg<int[]> Words { get; }
        private Dawg<int> Probabilities { get; }

        public LanguageDictionary(IReadOnlyList<string> suffixes, IReadOnlyList<Paradigm> paradigms,
            IReadOnlyList<string> paradigmPrefixes, IReadOnlyList<WordTag> tags, Dawg<int[]> words,
            Dawg<int> probabilities, IReadOnlyList<string> knownPrefixes, IReadOnlyList<string> hyphens) {
            
            Suffixes = suffixes;
            Paradigms = paradigms;
            ParadigmPrefixes = paradigmPrefixes;
            Tags = tags;
            Words = words;
            Probabilities = probabilities;
            KnownPrefixes = knownPrefixes;
            Hyphens = hyphens;
        }

        public string BuildNormalForm(ParadigmLink link, string word) {
            if(link.InternalIndex == 0)
                return word;

            var paradigm = Paradigms[link.Index];
            var paradigmCount = paradigm.Count / 3;
            
            var stem = BuildStem(paradigm, link.InternalIndex, word);

            var normalPrefixIndex = paradigm[paradigmCount * 2];
            var normalSuffixIndex = paradigm[0];

            return ParadigmPrefixes[normalPrefixIndex] + stem + Suffixes[normalSuffixIndex];
        }

        private string BuildStem(string word, ParadigmLink link) {
            return BuildStem(Paradigms[link.Index], link.InternalIndex, word);
        }
        
        private string BuildStem(Paradigm paradigm, int index, string word) {
            var paradigmCount = paradigm.Count / 3;
            
            var prefixIndex = paradigm[paradigmCount * 2 + index];
            var prefix = ParadigmPrefixes[prefixIndex];
            
            var suffixIndex = paradigm[index];
            var suffix = Suffixes[suffixIndex];

            if(string.IsNullOrEmpty(suffix)) {
                return word[prefix.Length..];
            } else {
                return word.Substring(prefix.Length, word.Length - prefix.Length - suffix.Length);
            }
        }

        public Word Similar(string word) {
            var result = Words[word];
            if(result == null)
                return null;

            var paradigmLinks = new List<ParadigmLink>(result.Length);
            for(int i = 0; i < result.Length; i += 2) {
                paradigmLinks.Add(new ParadigmLink(result[i], result[i + 1]));
            }

            return new Word(word, paradigmLinks);
        }

        private int GetTagIndex(ParadigmLink link) {
            var paradigm = Paradigms[link.Index];
            var offset = paradigm.Count / 3;
            return paradigm[offset + link.InternalIndex];
        }
        
        public WordTag GetTag(ParadigmLink link) {
            return Tags[GetTagIndex(link)];
        }

        public string InflectByParadigm(string word, ParadigmLink currentLink, ParadigmLink targetLink) {
            var paradigm = Paradigms[targetLink.Index];

            var paradigmCount = paradigm.Count / 3;
            var prefix = ParadigmPrefixes[paradigm[paradigmCount * 2 + targetLink.InternalIndex]];

            var suffix = Suffixes[paradigm[targetLink.InternalIndex]];

            var stem = BuildStem(word, currentLink);
            
            return prefix + stem + suffix;
        }
        
        public IEnumerable<ParadigmLink> GetParadigms(ParadigmLink link) {
            var paradigm = Paradigms[link.Index];
            var paradigmCount = paradigm.Count / 3;

            return Enumerable.Range(0, paradigmCount).Select(i => new ParadigmLink(link.Index, i)).ToArray();
        }

        public double CalculateProbability(string word, ParadigmLink link) {
            return Probabilities[word + ":" + GetTagIndex(link)];
        }
    }
}