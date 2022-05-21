using MorphAnalyzer.AnalyzerUnits.Helpers;

namespace MorphAnalyzer.AnalyzerUnits {
    public class UnknownPrefixAnalyzerUnit : IMorphAnalyzerUnit {

        public UnknownPrefixAnalyzerUnit(ISimpleMorphAnalyzer nextAnalyzer, int maxPrefixLength = 5, int minSuffixLength = 3) {
            NextAnalyzer = nextAnalyzer;
            MaxPrefixLength = maxPrefixLength;
            MinSuffixLength = minSuffixLength;
        }
        
        private ISimpleMorphAnalyzer NextAnalyzer { get; }

        private int MaxPrefixLength { get; }
        
        private int MinSuffixLength { get; }
        
        public bool Terminal => false;
        
        public IReadOnlyList<MorphologicalSignificance> Parse(string word, IReadOnlyList<IMorphAnalyzerUnit> analyzerConveyor) {
            if(analyzerConveyor.Contains(this))
                return Array.Empty<MorphologicalSignificance>();
            
            var result = new List<MorphologicalSignificance>();
            for(int i = 1; i <= Math.Min(MaxPrefixLength, word.Length - MinSuffixLength); ++i) {
                var unprefixedWord = word[i..];
                if(unprefixedWord.Length < MinSuffixLength || unprefixedWord.StartsWith("-"))
                    continue;

                var prefix = word[0..^unprefixedWord.Length];
                var parses = NextAnalyzer.Parse(unprefixedWord, analyzerConveyor.Append(this).ToArray());
                result.AddRange(parses.Select(p => BuildMorphologicalSignificance(p, prefix)));
            }

            return result;
        }

        public IEnumerable<MorphologicalSignificance> GetLexemes(MorphologicalSignificance morphologicalSignificance) {
            var internalSignificance = (MorphologicalSignificance)morphologicalSignificance.InternalData;
            var prefix = morphologicalSignificance.RawWord[0..^internalSignificance.RawWord.Length];

            return internalSignificance.Method.GetLexemes(internalSignificance)
                .Select(s => BuildMorphologicalSignificance(s, prefix))
                .ToArray();
        }

        private MorphologicalSignificance BuildMorphologicalSignificance(MorphologicalSignificance internalSignificance, string prefix) {
            return new MorphologicalSignificance(
                prefix + internalSignificance.RawWord,
                prefix + internalSignificance.NormalForm,
                internalSignificance,
                internalSignificance.Tag,
                this,
                internalSignificance.Probability * 0.5d
            );
        }
    }
}