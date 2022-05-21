namespace MorphAnalyzer.AnalyzerUnits.Helpers {
    
    /// <summary>
    /// basic interface for analyzer unit or for passing as analyzer unit params
    /// </summary>
    public interface ISimpleMorphAnalyzer {

        /// <summary>
        /// get all possible morphological significances of word 
        /// </summary>
        /// <param name="word"> word for analysis </param>
        /// <param name="analyzerConveyor"> list of previously used analyzers </param>
        /// <returns> all possible morphological significances of word (that this analyzer supports) </returns>
        IReadOnlyList<MorphologicalSignificance> Parse(string word, IReadOnlyList<IMorphAnalyzerUnit> analyzerConveyor);

    }
}