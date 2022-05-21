// ReSharper disable IdentifierTypo
// ReSharper disable once CheckNamespace
namespace MorphAnalyzer {
    internal class WordTag {
        
        public PartOfSpeech PartOfSpeech { get; }
        
        public Animacy? Animacy { get; }
        
        public Aspect? Aspect { get; }
        
        public Case? Case { get; }
        
        public Gender? Gender { get; }
        
        public Involvement? Involvement { get; }
        
        public Mood? Mood { get; }
        
        public Number? Number { get; }
        
        public Person? Person { get; }
        
        public Tense? Tense { get; }
        
        public Transitivity? Transitivity { get; }
        
        public Voice? Voice { get; }
        
        public IReadOnlyList<MicsTags> Mics { get; }

#if DEBUG
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public string DebugValue { get; }
#endif
        
        internal WordTag(PartOfSpeech partOfSpeech, Animacy? animacy, Aspect? aspect, Case? @case, Gender? gender,
            Involvement? involvement, Mood? mood, Number? number, Person? person, Tense? tense,
            Transitivity? transitivity, Voice? voice, IReadOnlyList<MicsTags> mics
#if DEBUG
            , string debugValue            
#endif
            ) {

            PartOfSpeech = partOfSpeech;
            Animacy = animacy;
            Aspect = aspect;
            Case = @case;
            Gender = gender;
            Involvement = involvement;
            Mood = mood;
            Number = number;
            Person = person;
            Tense = tense;
            Transitivity = transitivity;
            Voice = voice;
            Mics = mics;
#if DEBUG
            DebugValue = debugValue;
#endif
        }
        
    }
}