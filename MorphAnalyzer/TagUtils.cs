// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
namespace MorphAnalyzer {
    public static class TagUtils {

        private static readonly object[] NonProductiveGrammemes = new object[] {
            PartOfSpeech.Numeral, 
            PartOfSpeech.PronounNoun,
            PartOfSpeech.Predicative, 
            PartOfSpeech.Conjunction,
            PartOfSpeech.Particle, 
            PartOfSpeech.Interjection
        };
        
        internal static bool IsProductive(this WordTag tag) {
            return !NonProductiveGrammemes.Contains(tag.PartOfSpeech); // TODO add Apronominal (местоимённое)
        }

        internal static IReadOnlyList<object?> ToArray(this WordTag tag) {
            return new object?[] { tag.Animacy, tag.Aspect, tag.Case, tag.Gender, tag.Involvement, tag.Mood, tag.Number,
                    tag.Person, tag.Tense, tag.Transitivity, tag.Voice, tag.PartOfSpeech }
                .Concat(tag.Mics.Cast<object?>())
                .Where(x => x != null)
                .ToArray();
        }

        public static bool IsProductive(this MorphologicalSignificance significance) => significance.Tag.IsProductive();
    }
}