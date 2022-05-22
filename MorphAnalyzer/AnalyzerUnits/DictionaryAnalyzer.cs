namespace MorphAnalyzer.AnalyzerUnits {
    
    /// <summary>
    /// Analyzer unit that analyzes word using dictionary
    /// </summary>
    public class DictionaryAnalyzer : IMorphAnalyzerUnit {

        internal DictionaryAnalyzer(LanguageDictionary dictionary) {
            Dictionary = dictionary;
        }

        public bool Terminal => true;
        
        private LanguageDictionary Dictionary { get; }
        
        public IReadOnlyList<MorphologicalSignificance> Parse(string word, IReadOnlyList<IMorphAnalyzerUnit> analyzerConveyor) {
            word = word.ToLower();

            var match = Dictionary.Similar(word);
            if(match == null) {
                return Array.Empty<MorphologicalSignificance>();
            }
            
            var result = new List<MorphologicalSignificance>(match.Paradigms.Count);
            foreach(var link in match.Paradigms) {
                var normalForm = Dictionary.BuildNormalForm(link, match.Value);
                result.Add(BuildMorphologicalSignificance(match.Value, normalForm, link));
            }
            
            return result;
        }

        public IEnumerable<MorphologicalSignificance> GetLexemes(MorphologicalSignificance morphologicalSignificance) {
            var link = (ParadigmLink)morphologicalSignificance.InternalData;
            var paradigms = Dictionary.GetParadigms(link);

            foreach(var paradigmLink in paradigms) {
                var form = Dictionary.InflectByParadigm(morphologicalSignificance.RawWord, link, paradigmLink); 
                yield return BuildMorphologicalSignificance(form, morphologicalSignificance.NormalForm, paradigmLink);
            }
        }

        private MorphologicalSignificance BuildMorphologicalSignificance(string word, string normalForm, ParadigmLink link) {
            return new MorphologicalSignificance(word, normalForm, link, Dictionary.GetTag(link), this, Dictionary.CalculateProbability(word, link));
        }
    }
}