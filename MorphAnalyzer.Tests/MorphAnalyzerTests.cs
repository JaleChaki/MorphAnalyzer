using Xunit;

namespace MorphAnalyzer.Tests {
    public class MorphAnalyzerTests {
        private MorphAnalyzer Analyzer { get; }
        
        public MorphAnalyzerTests() {
            Analyzer = new MorphAnalyzer();
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
        
    }
}