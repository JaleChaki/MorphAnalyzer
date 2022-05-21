using MorphAnalyzer.AnalyzerUnits;

namespace MorphAnalyzer {
    internal class Word {
        
        public string Value { get; }
        
        public IReadOnlyList<ParadigmLink> Paradigms { get; }

        public Word(string value, IReadOnlyList<ParadigmLink> paradigms) {
            Value = value;
            Paradigms = paradigms;
        }
        
    }
}