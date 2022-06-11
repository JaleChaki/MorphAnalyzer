using MorphAnalyzer.AnalyzerUnits;
using MorphAnalyzer.AnalyzerUnits.Helpers;

namespace MorphAnalyzer {

    public class MorphAnalyzer : ISimpleMorphAnalyzer {
        private LanguageDictionary Dictionary { get; }
        
        private List<IMorphAnalyzerUnit> AnalyzerUnits { get; }
        
        public MorphAnalyzer(string language = "ru") : this(LanguageDictionaryReader.Read(Path.Combine(@"Dictionaries\", language))) { }

        internal MorphAnalyzer(LanguageDictionary dictionary) {
            Dictionary = dictionary;
            var dictionaryAnalyzer = new DictionaryAnalyzer(Dictionary);
            AnalyzerUnits = new List<IMorphAnalyzerUnit> {
                dictionaryAnalyzer, 
                new KnownPrefixAnalyzer(this, Dictionary.KnownPrefixes),
                new HyphenSeparatedParticleAnalyzer(dictionaryAnalyzer, Dictionary.Hyphens),
                new HyphenAdverbAnalyzer(this),
                new UnknownPrefixAnalyzer(dictionaryAnalyzer)
            };
        }

        internal MorphAnalyzer(LanguageDictionary dictionary, IEnumerable<IMorphAnalyzerUnit> analyzerUnits) {
            Dictionary = dictionary;
            AnalyzerUnits = analyzerUnits.ToList();
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
                var lexemeTagValues = lexeme.Tag.ToArray();

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