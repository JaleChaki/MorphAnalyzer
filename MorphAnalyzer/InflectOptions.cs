// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable IdentifierTypo
namespace MorphAnalyzer {
    public class InflectOptions {
        
        public Animacy? Animacy { get; set; }
        
        public Aspect? Aspect { get; set; }
        
        public Case? Case { get; set; }
        
        public Gender? Gender { get; set; }
        
        public Involvement? Involvement { get; set; }
        
        public Mood? Mood { get; set; }
        
        public Number? Number { get; set; }
        
        public PartOfSpeech? PartOfSpeech { get; set; }
        
        public Person? Person { get; set; }
        
        public Tense? Tense { get; set; }
        
        public Transitivity? Transitivity { get; set; }
        
        public Voice? Voice { get; set; }
        
        public IEnumerable<MicsTags> Mics { get; set; } = Array.Empty<MicsTags>();

    }
}