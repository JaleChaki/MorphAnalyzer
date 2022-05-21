using MorphAnalyzer.AnalyzerUnits.Helpers;

namespace MorphAnalyzer.AnalyzerUnits {
    public class KnownPrefixAnalyzerUnit : IMorphAnalyzerUnit {

        public KnownPrefixAnalyzerUnit(ISimpleMorphAnalyzer nextAnalyzer, IEnumerable<string> knownPrefixes, int minSuffixLength = 3) {
            NextAnalyzerUnit = nextAnalyzer;
            KnownPrefixes = knownPrefixes.OrderBy(prefix => -prefix.Length).ToHashSet();
            MinSuffixLength = minSuffixLength;
        }

        private ISimpleMorphAnalyzer NextAnalyzerUnit { get; }

        private IReadOnlySet<string> KnownPrefixes { get; }
        
        private int MinSuffixLength { get; }

        public bool Terminal => true;
        
        public IReadOnlyList<MorphologicalSignificance> Parse(string word, IReadOnlyList<IMorphAnalyzerUnit> analyzerConveyor) {
            var splits = GetPossibleSplits(word);
            var result = new List<MorphologicalSignificance>();
            foreach(var split in splits) {
                var parses = NextAnalyzerUnit.Parse(split.UnprefixedWord, analyzerConveyor.Append(this).ToArray());
                result.AddRange(parses.Where(p => p.Tag.IsProductive()).Select(p => BuildMorphologicalSignificance(p, split)));
            }

            return result;
        }

        public IEnumerable<MorphologicalSignificance> GetLexemes(MorphologicalSignificance morphologicalSignificance) {
            var internalSignificance = (MorphologicalSignificance)morphologicalSignificance.InternalData;
            var prefix = morphologicalSignificance.RawWord[0..^internalSignificance.RawWord.Length];
            return internalSignificance.Method.GetLexemes(internalSignificance)
                .Select(s => BuildMorphologicalSignificance(s, new PossibleSplit(prefix, internalSignificance.RawWord)))
                .ToArray();
        }

        private IEnumerable<PossibleSplit> GetPossibleSplits(string word) {
            foreach(var prefix in KnownPrefixes) {
                if(!word.StartsWith(prefix))
                    continue;
                
                var unprefixedWord = word[prefix.Length..];
                if(unprefixedWord.Length < MinSuffixLength || unprefixedWord.StartsWith("-"))
                    continue;
                yield return new PossibleSplit(prefix, unprefixedWord);
            }
        }

        private MorphologicalSignificance BuildMorphologicalSignificance(MorphologicalSignificance internalSignificance, PossibleSplit split) {
            return new MorphologicalSignificance(
                split.Prefix + internalSignificance.RawWord,
                split.Prefix + internalSignificance.NormalForm,
                internalSignificance,
                internalSignificance.Tag,
                this,
                internalSignificance.Probability * 0.75d);
        }

        private struct PossibleSplit {
            public string Prefix { get; }
            public string UnprefixedWord { get; }

            public PossibleSplit(string prefix, string unprefixedWord) {
                Prefix = prefix;
                UnprefixedWord = unprefixedWord;
            }
        }
    }
}