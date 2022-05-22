using MorphAnalyzer.AnalyzerUnits.Helpers;

// ReSharper disable CommentTypo
namespace MorphAnalyzer.AnalyzerUnits {
    
    /// <summary>
    /// Analyzer unit, that detects particle after hyphen
    /// Example: "смотри-ка" = "смотри" + "-ка"
    /// </summary>
    public class HyphenSeparatedParticleAnalyzer : IMorphAnalyzerUnit {
        
        public HyphenSeparatedParticleAnalyzer(ISimpleMorphAnalyzer nextAnalyzer, IReadOnlyList<string> particlesAfterHyphen) {
            ParticlesAfterHyphen = particlesAfterHyphen;
            NextAnalyzer = nextAnalyzer;
        }
        
        public bool Terminal => true;

        private IReadOnlyList<string> ParticlesAfterHyphen { get; }
        private ISimpleMorphAnalyzer NextAnalyzer { get; }
        
        public IReadOnlyList<MorphologicalSignificance> Parse(string word, IReadOnlyList<IMorphAnalyzerUnit> analyzerConveyor) {
            if(analyzerConveyor.Contains(this))
                return Array.Empty<MorphologicalSignificance>();
            
            var result = new List<MorphologicalSignificance>();
            foreach(var particle in ParticlesAfterHyphen) {
                if(!word.EndsWith(particle))
                    continue;

                var prefixedWord = word[..^particle.Length];
                var parses = NextAnalyzer.Parse(prefixedWord, analyzerConveyor.Append(this).ToArray());
                if(parses == null)
                    continue;
                
                result.AddRange(
                    parses.Select(parse => BuildMorphologicalSignificance(parse, particle))
                );
                break;
            }

            return result;
        }

        public IEnumerable<MorphologicalSignificance> GetLexemes(MorphologicalSignificance morphologicalSignificance) {
            var internalSignificance = (MorphologicalSignificance)morphologicalSignificance.InternalData;
            var prefix = internalSignificance.RawWord;
            var particle = morphologicalSignificance.RawWord[prefix.Length..];
            return internalSignificance.Method.GetLexemes(internalSignificance)
                .Select(parse => BuildMorphologicalSignificance(parse, particle))
                .ToArray();
        }

        private MorphologicalSignificance BuildMorphologicalSignificance(MorphologicalSignificance internalSignificance, string particle) {
            return new MorphologicalSignificance(
                internalSignificance.RawWord + particle,
                internalSignificance.NormalForm + particle,
                internalSignificance,
                internalSignificance.Tag,
                this,
                internalSignificance.Probability * 0.9d
            );
        }
    }
}