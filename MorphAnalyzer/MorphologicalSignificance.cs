using MorphAnalyzer.AnalyzerUnits;

namespace MorphAnalyzer {
    public class MorphologicalSignificance {
        
        public string RawWord { get; }
        
        public string NormalForm { get; }

        public PartOfSpeech PartOfSpeech => Tag.PartOfSpeech;

        public Case? Case => Tag.Case;

        public Gender? Gender => Tag.Gender;

        public Number? Number => Tag.Number;

        // ReSharper disable once IdentifierTypo
        public Animacy? Animacy => Tag.Animacy;

        public Aspect? Aspect => Tag.Aspect;

        public Involvement? Involvement => Tag.Involvement;

        public Mood? Mood => Tag.Mood;

        public Person? Person => Tag.Person;

        public Tense? Tense => Tag.Tense;

        public Transitivity? Transitivity => Tag.Transitivity;

        public Voice? Voice => Tag.Voice;

        public IReadOnlyList<string> Mics => Tag.Mics;

        public double Probability { get; internal set; }
        
        internal object InternalData { get; }
        
        internal IMorphAnalyzerUnit Method { get; }
        
        internal WordTag Tag { get; }
        
        internal MorphologicalSignificance(string rawWord, string normalForm, object internalData, WordTag tag, IMorphAnalyzerUnit method, double probability) {
            RawWord = rawWord;
            NormalForm = normalForm;
            InternalData = internalData;
            Tag = tag;
            Method = method;
            Probability = probability;
        }
        
    }
}