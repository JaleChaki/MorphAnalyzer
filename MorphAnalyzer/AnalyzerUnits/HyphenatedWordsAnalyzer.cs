using MorphAnalyzer.AnalyzerUnits.Helpers;

namespace MorphAnalyzer.AnalyzerUnits {
    public class HyphenatedWordsAnalyzer : IMorphAnalyzerUnit {

        public HyphenatedWordsAnalyzer(ISimpleMorphAnalyzer nextAnalyzer, IReadOnlyList<string> knownPrefixes) {
            NextAnalyzer = nextAnalyzer;
            KnownPrefixes = knownPrefixes;
        }
        
        public bool Terminal => true;
        
        private IReadOnlyList<string> KnownPrefixes { get; }

        private ISimpleMorphAnalyzer NextAnalyzer { get; }

        // ReSharper disable once IdentifierTypo
        private static readonly IReadOnlyDictionary<object, object> EqualGrammemes = new Dictionary<object, object> {
            { "V-oy", "V-ey" },
            { Case.Locative1, Case.Locative },
            { Case.Genitive1, Case.Genitive }
        };

        public IReadOnlyList<MorphologicalSignificance> Parse(string word, IReadOnlyList<IMorphAnalyzerUnit> analyzerConveyor) {
            if(!ShouldParse(word))
                return Array.Empty<MorphologicalSignificance>();

            var split = word.Split('-');
            var left = split[0];
            var right = split[1];
            var leftParses = NextAnalyzer.Parse(left, analyzerConveyor.Append(this).ToArray());
            var rightParses = NextAnalyzer.Parse(right, analyzerConveyor.Append(this).ToArray());

            return ParseAsVariableBoth(leftParses, rightParses);
        }

        private bool ShouldParse(string word) {
            if(!word.Contains('-'))
                return false;
            if(word.Count(c => c == '-') > 1)
                return false;
            if(HasSkipPrefix(word))
                return false;

            return true;
        }

        private bool HasSkipPrefix(string word) {
            return KnownPrefixes.Any(word.StartsWith);
        }

        private IReadOnlyList<MorphologicalSignificance> ParseAsVariableBoth(IReadOnlyList<MorphologicalSignificance> leftParses, IReadOnlyList<MorphologicalSignificance> rightParses) {
            var rightTags = rightParses.Select(parse => ReplaceGrammemes(parse.Tag)).ToArray();
            var result = new List<MorphologicalSignificance>();
            foreach(var leftParse in leftParses) {
                var leftTag = leftParse.Tag;
                
                // TODO skip if Tag unknown

                leftTag = ReplaceGrammemes(leftTag);

                for(int i = 0; i < rightParses.Count; ++i) {
                    var rightTag = rightTags[i];

                    if(!TagEquals(leftTag, rightTag))
                        continue;

                    result.Add(BuildMorphologicalSignificanceAsVariableBoth(leftParse, rightParses[i]));
                }
            }

            return result;
        }

        // ReSharper disable once IdentifierTypo
        private WordTag ReplaceGrammemes(WordTag original) {
            T SafeGetDictionaryKey<T>(T key) {
                if(key == null)
                    return default;
                
                return EqualGrammemes.ContainsKey(key) ? (T)EqualGrammemes[key] : key;
            }

            var partOfSpeech = SafeGetDictionaryKey(original.PartOfSpeech);
            // ReSharper disable once IdentifierTypo
            var animacy = SafeGetDictionaryKey(original.Animacy);
            var aspect = SafeGetDictionaryKey(original.Aspect);
            var @case = SafeGetDictionaryKey(original.Case);
            var gender = SafeGetDictionaryKey(original.Gender);
            var involvement = SafeGetDictionaryKey(original.Involvement);
            var mood = SafeGetDictionaryKey(original.Mood);
            var number = SafeGetDictionaryKey(original.Number);
            var person = SafeGetDictionaryKey(original.Person);
            var tense = SafeGetDictionaryKey(original.Tense);
            var transitivity = SafeGetDictionaryKey(original.Transitivity);
            var voice = SafeGetDictionaryKey(original.Voice);
            var mics = original.Mics.Select(SafeGetDictionaryKey).ToArray();

            return new WordTag(partOfSpeech, animacy, aspect, @case, gender, involvement, mood, number, person, tense,
                transitivity, voice, mics
#if DEBUG
                , "DebugValue for temporary WordTag not supported"
#endif
            );
        }

        private bool TagEquals(WordTag a, WordTag b) {
            return a.PartOfSpeech == b.PartOfSpeech &&
                   a.Case == b.Case &&
                   a.Number == b.Number &&
                   a.Person == b.Person &&
                   a.Tense == b.Tense;
        }

        private MorphologicalSignificance BuildMorphologicalSignificanceAsVariableBoth(MorphologicalSignificance left, MorphologicalSignificance right, double? customProbability = null) {
            return new MorphologicalSignificance(left.RawWord + '-' + right.RawWord,
                left.NormalForm + '-' + right.NormalForm,
                new InternalData(left, right, false),
                left.Tag,
                this,
                customProbability ?? left.Probability * 0.75d
            );
        }

        private record InternalData(MorphologicalSignificance Left, MorphologicalSignificance Right, bool LeftFixed);
        
        public IEnumerable<MorphologicalSignificance> GetLexemes(MorphologicalSignificance morphologicalSignificance) {
            var internalData = (InternalData) morphologicalSignificance.InternalData;

            if(internalData.LeftFixed) {
                // TODO
                throw new NotImplementedException();
            } else {
                var leftSignificance = internalData.Left;
                var rightSignificance = internalData.Right;
                var leftLexemes = leftSignificance.Method.GetLexemes(leftSignificance);
                var rightLexemes = rightSignificance.Method.GetLexemes(rightSignificance);

                return MergeLexemes(leftLexemes, rightLexemes);
            }
        }

        private IEnumerable<MorphologicalSignificance> MergeLexemes(IEnumerable<MorphologicalSignificance> leftLexemes,
            IEnumerable<MorphologicalSignificance> rightLexemes) {

            leftLexemes = leftLexemes.ToArray();
            foreach(var right in rightLexemes) {
                var rightTagFeatures = ReplaceGrammemes(right.Tag).ToArray();
                int minDistance = 1000000;
                MorphologicalSignificance closestLexeme = null;
                foreach(var left in leftLexemes) {
                    var leftTagFeatures = ReplaceGrammemes(left.Tag).ToArray();
                    var symmetricDistance = rightTagFeatures.Except(leftTagFeatures).Count() +
                                            leftTagFeatures.Except(rightTagFeatures).Count();
                    if(symmetricDistance < minDistance) {
                        minDistance = symmetricDistance;
                        closestLexeme = left;
                    }
                }
                yield return BuildMorphologicalSignificanceAsVariableBoth(closestLexeme, 
                    right, 
                    (closestLexeme.Probability + right.Probability) / 2 * 0.75d);
            }
        }
    }
}