// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo
// ReSharper disable once CheckNamespace
namespace MorphAnalyzer {
    internal static class WordTagBuilder {
        
#region Mappers
        private static readonly IReadOnlyDictionary<string, PartOfSpeech> PartOfSpeechMapper = new Dictionary<string, PartOfSpeech>() {
            { "NOUN", PartOfSpeech.Noun },
            { "ADJF", PartOfSpeech.AdjectiveFull },
            { "ADJS", PartOfSpeech.AdjectiveShort },
            { "COMP", PartOfSpeech.Comparative },
            { "VERB", PartOfSpeech.Verb },
            { "INFN", PartOfSpeech.Infinitive },
            { "PRTF", PartOfSpeech.ParticipleFull },
            { "PRTS", PartOfSpeech.ParticipleShort },
            { "GRND", PartOfSpeech.Gerund },
            { "NUMR", PartOfSpeech.Numeral },
            { "ADVB", PartOfSpeech.Adverb },
            { "NPRO", PartOfSpeech.PronounNoun },
            { "PRED", PartOfSpeech.Predicative },
            { "PREP", PartOfSpeech.Preposition },
            { "CONJ", PartOfSpeech.Conjunction },
            { "PRCL", PartOfSpeech.Particle },
            { "INTJ", PartOfSpeech.Interjection }
        };

        private static readonly IReadOnlyDictionary<string, Animacy> AnimacyMapper = new Dictionary<string, Animacy>() {
            { "anim", Animacy.Animated },
            { "inan", Animacy.Inanimated }
        };
        
        private static readonly IReadOnlyDictionary<string, Aspect> AspectMapper = new Dictionary<string, Aspect>() {
            { "perf", Aspect.Perfect },
            { "impf", Aspect.Imperfect }
        };

        private static readonly IReadOnlyDictionary<string, Gender> GenderMapper = new Dictionary<string, Gender>() {
            { "GNdr", Gender.GNdr },
            { "masc", Gender.Masculine },
            { "femn", Gender.Feminine },
            { "neut", Gender.Neutral },
            { "ms-f", Gender.MsF }
        };

        private static readonly IReadOnlyDictionary<string, Number> NumberMapper = new Dictionary<string, Number>() {
            { "sing", Number.Single },
            { "plur", Number.Plural }
        };

        private static readonly IReadOnlyDictionary<string, Case> CaseMapper = new Dictionary<string, Case>() {
            { "nomn", Case.Nominative },
            { "gent", Case.Genitive },
            { "datv", Case.Dative },
            { "accs", Case.Accusative },
            { "ablt", Case.Ablative },
            { "loct", Case.Locative },
            { "voct", Case.Vocative },
            { "gen1", Case.Genitive1 },
            { "gen2", Case.Genitive2 },
            { "acc2", Case.Accusative2 },
            { "loc1", Case.Locative1 },
            { "loc2", Case.Locative2 }
        };
        
        private static readonly IReadOnlyDictionary<string, Transitivity> TransitivityMapper = new Dictionary<string, Transitivity>() {
            { "tran", Transitivity.Transitive },
            { "intr", Transitivity.Intransitive }
        };

        private static readonly IReadOnlyDictionary<string, Person> PersonMapper = new Dictionary<string, Person>() {
            { "1per", Person.First },
            { "2per", Person.Second },
            { "3per", Person.Third }
        };
        
        private static readonly IReadOnlyDictionary<string, Tense> TenseMapper = new Dictionary<string, Tense>() {
            { "pres", Tense.Present },
            { "past", Tense.Past },
            { "futr", Tense.Future }
        };
        
        private static readonly IReadOnlyDictionary<string, Involvement> InvolvementMapper = new Dictionary<string, Involvement>() {
            { "incl", Involvement.Include },
            { "excl", Involvement.Exclude }
        };
        
        private static readonly IReadOnlyDictionary<string, Mood> MoodMapper = new Dictionary<string, Mood>() {
            { "indc", Mood.Indicative },
            { "impr", Mood.Imperative }
        };
        
        private static readonly IReadOnlyDictionary<string, Voice> VoiceMapper = new Dictionary<string, Voice>() {
            { "actv", Voice.Active },
            { "pssv", Voice.Passive }
        };
        
        private static IReadOnlyList<string> KnownTags { get; }
#endregion

        static WordTagBuilder() {
            KnownTags = PartOfSpeechMapper.Keys
                .Concat(AnimacyMapper.Keys)
                .Concat(AspectMapper.Keys)
                .Concat(GenderMapper.Keys)
                .Concat(NumberMapper.Keys)
                .Concat(CaseMapper.Keys)
                .Concat(TransitivityMapper.Keys)
                .Concat(PersonMapper.Keys)
                .Concat(TenseMapper.Keys)
                .Concat(InvolvementMapper.Keys)
                .Concat(MoodMapper.Keys)
                .Concat(VoiceMapper.Keys)
                .ToArray();
        }

        public static WordTag Build(string tagDescriptor) {
            var tags = tagDescriptor.Replace(' ', ',').Split(',');
            
            var partOfSpeech = SelectRequiredTagFrom(tags, PartOfSpeechMapper);
            var animacy = SelectTagFrom(tags, AnimacyMapper);
            var aspect = SelectTagFrom(tags, AspectMapper);
            var gender = SelectTagFrom(tags, GenderMapper);
            var number = SelectTagFrom(tags, NumberMapper);
            var @case = SelectTagFrom(tags, CaseMapper);
            var transitivity = SelectTagFrom(tags, TransitivityMapper);
            var person = SelectTagFrom(tags, PersonMapper);
            var tense = SelectTagFrom(tags, TenseMapper);
            var involvement = SelectTagFrom(tags, InvolvementMapper);
            var mood = SelectTagFrom(tags, MoodMapper);
            var voice = SelectTagFrom(tags, VoiceMapper);

            return new WordTag(partOfSpeech, animacy, aspect, @case, gender, involvement, mood, number, person, tense,
                transitivity, voice, SelectUnknownTags(tags)
#if DEBUG
                , tagDescriptor
#endif
            );
        }

        private static T SelectRequiredTagFrom<T>(string[] tags, IReadOnlyDictionary<string, T> dictionary) {
            foreach(var tag in tags) {
                if(!dictionary.ContainsKey(tag)) 
                    continue;
                return dictionary[tag];
            }

            throw new Exception("Part of speech not found");
        }
        
        private static T? SelectTagFrom<T>(string[] tags, IReadOnlyDictionary<string, T> dictionary) where T : struct {
            foreach(var tag in tags) {
                if(!dictionary.ContainsKey(tag)) 
                    continue;
                return dictionary[tag];
            }

            return null;
        }

        private static string[] SelectUnknownTags(string[] tags) {
            return tags.Except(KnownTags).ToArray();
        }
        
    }
}