using System.Collections.Generic;
using MorphAnalyzer.AnalyzerUnits;
using MorphAnalyzer.AnalyzerUnits.Helpers;

namespace MorphAnalyzer.Tests {
    public class AnalyzerUnitComposite : ISimpleMorphAnalyzer {
        
        public List<IMorphAnalyzerUnit> AnalyzerUnits { get; set; }
        
        public IReadOnlyList<MorphologicalSignificance> Parse(string word, IReadOnlyList<IMorphAnalyzerUnit> analyzerConveyor) {
            var result = new List<MorphologicalSignificance>();
            foreach(var unit in AnalyzerUnits) {
                var parses = unit.Parse(word, analyzerConveyor);
                result.AddRange(parses);

                if(parses.Count > 0 && unit.Terminal)
                    break;
            }

            return result;
        }
    }
}