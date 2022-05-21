using MorphAnalyzer.AnalyzerUnits.Helpers;

namespace MorphAnalyzer.AnalyzerUnits {
    public class HyphenAdverbAnalyzer : IMorphAnalyzerUnit {
        public bool Terminal => true;

        private const string PREFIX = "по-";

        private ISimpleMorphAnalyzer NextAnalyzer { get; }

        internal HyphenAdverbAnalyzer(ISimpleMorphAnalyzer nextAnalyzer) {
            NextAnalyzer = nextAnalyzer;
        }
        
        public IReadOnlyList<MorphologicalSignificance> Parse(string word, IReadOnlyList<IMorphAnalyzerUnit> analyzerConveyor) {
            if(!word.StartsWith(PREFIX) || word.Length < 5)
                return Array.Empty<MorphologicalSignificance>();

            if(analyzerConveyor.Contains(this))
                return Array.Empty<MorphologicalSignificance>();
            

            var unprefixedWord = word[PREFIX.Length..];
            var significances = NextAnalyzer.Parse(unprefixedWord, analyzerConveyor.Append(this).ToArray());
            var result = new List<MorphologicalSignificance>();
            foreach(var parse in significances) {
                if(!IsAdjective(parse.PartOfSpeech) || parse.Number != Number.Single || parse.Case != Case.Dative)
                    continue;
                
                result.Add(BuildMorphologicalSignificance(parse));
            }

            return result;

            static bool IsAdjective(PartOfSpeech partOfSpeech) {
                // ReSharper disable once MergeIntoLogicalPattern
                return partOfSpeech == PartOfSpeech.AdjectiveFull || partOfSpeech == PartOfSpeech.AdjectiveShort;
            }
            
            MorphologicalSignificance BuildMorphologicalSignificance(MorphologicalSignificance internalSignificance) {
                return new MorphologicalSignificance(
                    word,
                    word,
                    null,
                    internalSignificance.Tag,
                    this,
                    internalSignificance.Probability * 0.7d
                );
            }
        }

        public IEnumerable<MorphologicalSignificance> GetLexemes(MorphologicalSignificance morphologicalSignificance) {
            yield return morphologicalSignificance;
        }
    }
}