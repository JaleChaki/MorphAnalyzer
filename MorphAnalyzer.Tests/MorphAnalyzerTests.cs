using System.Collections.Generic;
using System.Linq;
using MorphAnalyzer.AnalyzerUnits;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace MorphAnalyzer.Tests {
    public class MorphAnalyzerTests {
        private MorphAnalyzer Analyzer { get; }

        public MorphAnalyzerTests() {
            Analyzer = Utils.GetMorphAnalyzer("Ru");
        }

        [Theory]
        [InlineData("рыбой", "рыба")]
        [InlineData("дома", "дом")]
        [InlineData("пересекая", "пересекать")]
        [InlineData("псевдокошкой", "псевдокошка")]
        [InlineData("байт-рыбой", "байт-рыба")]
        [InlineData("по-западному", "по-западному")]
        [InlineData("по-псевдозападному", "по-псевдозападному")]
        public void Parse_NormalForm(string input, string expectedNormalForm) {
            var parses = Analyzer.Parse(input);
            var result = parses[0];
            Assert.Equal(expectedNormalForm, result.NormalForm);
        }

        [Fact]
        public void StopsIfTerminalNodeReached() {
            var analyzerUnits = new IMorphAnalyzerUnit[] {
                new DummyWordAnalyzer(),
                Utils.GetAnalyzerUnit<DictionaryAnalyzer>("Ru")
            };
            var analyzer = new MorphAnalyzer(Utils.GetLanguageDictionary("Ru"), analyzerUnits);
            var result = analyzer.Parse("кот");
            Assert.Single(result, DummyWordAnalyzer.Result);
        }

        [Fact]
        public void ResultsSortedByProbability() {
            var results = Analyzer.Parse("дома");
            Assert.True(results.Count > 1);
            Assert.True(results.SequenceEqual(results.OrderByDescending(x => x.Probability)));
        }

        private class DummyWordAnalyzer : IMorphAnalyzerUnit {
            public bool Terminal => true;

            public static MorphologicalSignificance Result =
                new MorphologicalSignificance("test", "test", null, null, null, 1);

            public IReadOnlyList<MorphologicalSignificance> Parse(string word, IReadOnlyList<IMorphAnalyzerUnit> analyzerConveyor) {
                return new[] { Result };
            }

            public IEnumerable<MorphologicalSignificance> GetLexemes(MorphologicalSignificance morphologicalSignificance) {
                throw new System.NotImplementedException();
            }
        }
        
    }
}