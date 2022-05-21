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

        public static bool IsProductive(this MorphologicalSignificance significance) => significance.Tag.IsProductive();
    }
}