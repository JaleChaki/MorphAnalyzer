using MorphAnalyzer.AnalyzerUnits;
using MorphAnalyzer.AnalyzerUnits.Helpers;

namespace MorphAnalyzer {

    public class MorphAnalyzer : ISimpleMorphAnalyzer {
        private LanguageDictionary Dictionary { get; }
        
        private List<IMorphAnalyzerUnit> AnalyzerUnits { get; }
        
        public MorphAnalyzer(string language = "ru") {
            Dictionary = LanguageDictionaryReader.Read(Path.Combine(@"Dictionaries\", language));
            var dictionaryAnalyzer = new DictionaryAnalyzer(Dictionary);
            AnalyzerUnits = new List<IMorphAnalyzerUnit> {
                dictionaryAnalyzer, 
                new KnownPrefixAnalyzer(this, Dictionary.KnownPrefixes),
                new HyphenSeparatedParticleAnalyzer(dictionaryAnalyzer, Dictionary.Hyphens),
                new HyphenAdverbAnalyzer(this),
                new UnknownPrefixAnalyzer(dictionaryAnalyzer)
            };
        }

        public IReadOnlyList<MorphologicalSignificance> Parse(string word) {
            return ((ISimpleMorphAnalyzer) this).Parse(word, Array.Empty<IMorphAnalyzerUnit>());
        }

        IReadOnlyList<MorphologicalSignificance> ISimpleMorphAnalyzer.Parse(string word, IReadOnlyList<IMorphAnalyzerUnit> analyzerConveyor) {
            var results = new List<MorphologicalSignificance>();
            foreach(var unit in AnalyzerUnits) {
                var unitResult = unit.Parse(word, analyzerConveyor);
                if(unitResult.Count > 0) {
                    results.AddRange(unitResult);
                    if(unit.Terminal)
                        break;
                }
            }
            
            PatchProbabilities(results);
            return results;
        }

        public IReadOnlyList<MorphologicalSignificance> Inflect(MorphologicalSignificance morphologicalSignificance, InflectOptions options) {
            bool Satisfies(MorphologicalSignificance lexeme) {
                var lexemeTagValues = new object?[] {
                    lexeme.Animacy, lexeme.Aspect, lexeme.Case, lexeme.Gender,
                    lexeme.Involvement, lexeme.Mood, lexeme.Number, lexeme.Person, lexeme.Tense, lexeme.Transitivity,
                    lexeme.Voice, lexeme.PartOfSpeech
                }.Concat(lexeme.Mics.Cast<object?>())
                    .Where(x => x != null).ToArray();

                var optionsTagValues = new object?[] {
                    options.Animacy, options.Aspect, options.Case, options.Gender,
                    options.Involvement, options.Mood, options.Number, options.Person, options.Tense, options.Transitivity,
                    options.Voice, options.PartOfSpeech
                }.Concat(options.Mics.Cast<object?>())
                    .Where(x => x != null);

                return !optionsTagValues.Except(lexemeTagValues).Any();
            }
            
            var lexemes = morphologicalSignificance.Method.GetLexemes(morphologicalSignificance);
            return lexemes.Where(Satisfies).ToArray();
        }

        private void PatchProbabilities(List<MorphologicalSignificance> primaryAnalyzeResults) {
            var sum = primaryAnalyzeResults.Sum(x => x.Probability);
            if(sum == 0) {
                for(int i = 0; i < primaryAnalyzeResults.Count; ++i) {
                    primaryAnalyzeResults[i].Probability = 1d / primaryAnalyzeResults.Count;
                }
                return;
            }
            for(int i = 0; i < primaryAnalyzeResults.Count; ++i) {
                primaryAnalyzeResults[i].Probability /= sum;
            }
            primaryAnalyzeResults.Sort((a, b) => b.Probability.CompareTo(a.Probability));
        }
    }
}