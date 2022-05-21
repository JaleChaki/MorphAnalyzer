using MorphAnalyzer.AnalyzerUnits.Helpers;

// ReSharper disable CommentTypo
namespace MorphAnalyzer.AnalyzerUnits {
    
    /// <summary>
    /// interface for analyzer units
    /// </summary>
    public interface IMorphAnalyzerUnit : ISimpleMorphAnalyzer {

        /// <summary>
        /// if true, analysis stops at this unit (if this unit returned a non-empty result)
        /// </summary>
        bool Terminal { get; }
        
        /// <summary>
        /// get all possible lexemes of word
        /// </summary>
        /// <param name="morphologicalSignificance"> significance, that contains inflective word </param>
        /// <returns> enumerable of all possible lexemes of a word </returns>
        IEnumerable<MorphologicalSignificance> GetLexemes(MorphologicalSignificance morphologicalSignificance);

    }
}