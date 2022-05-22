using System;
using MorphAnalyzer.AnalyzerUnits;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace MorphAnalyzer.Tests.AnalyzerUnits {
    public class HyphenAdverbAnalyzerTests : AnalyzerUnitTester<HyphenAdverbAnalyzer> {

        [Theory]
        [InlineData("Ru", "по-западному", "ADVB")]
        public void ParseHyphenAdverb(string language, string word, string expectedTag) {
            TestParse(language, word, 1, new []{ word }, new []{ expectedTag });
        }

        [Theory]
        [InlineData("Ru", "по-западным")]
        [InlineData("Ru", "по-западные")]
        [InlineData("Ru", "по-запад")]
        public void Parse_NotApplicableWord(string language, string word) {
            TestParse(language, word, 0, Array.Empty<string>(), Array.Empty<string>());
        }
        
        [Theory]
        [InlineData("Ru", "по-западному", "ADVB")]
        public void GetLexemes(string language, string word, string expectedTag) {
            TestGetLexemes(language, word, new[] { word }, new [] { expectedTag });
        }
    }
}