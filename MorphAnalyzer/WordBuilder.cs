using MorphAnalyzer.AnalyzerUnits;

namespace MorphAnalyzer {
    internal static class WordBuilder {

        public static Word Build(string value, IReadOnlyList<int> indexes) {
            var paradigms = new List<ParadigmLink>();
            for(int i = 0; i < indexes.Count; i += 2) {
                paradigms.Add(new ParadigmLink(indexes[i], indexes[i + 1]));
            }

            return new Word(value, paradigms);
        }
        
    }
}