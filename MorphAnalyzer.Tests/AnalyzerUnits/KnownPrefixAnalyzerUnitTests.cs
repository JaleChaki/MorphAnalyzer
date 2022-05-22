using System;
using System.Linq;
using MorphAnalyzer.AnalyzerUnits;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace MorphAnalyzer.Tests.AnalyzerUnits {
    public class KnownPrefixAnalyzerUnitTests : AnalyzerUnitTester<KnownPrefixAnalyzerUnit> {

        [Theory]
        [InlineData("Ru", "квазирыбой", 1, "квазирыба=NOUN,anim,femn sing,ablt")]
        [InlineData("Ru", "космодома", 4, 
            "космодома=ADVB;" + 
            "космодом=NOUN,inan,masc sing,gent;" + 
            "космодом=NOUN,inan,masc plur,nomn;" + 
            "космодом=NOUN,inan,masc plur,accs")]
        public void ParseKnownPrefix_MultipleSignificances(string language, string word, int expectedResultsCount, string expectedLexemesWithTags) {
            var lexemesAndTags = ExtractLexemesAndTags(expectedLexemesWithTags);
            TestParse(language, word, expectedResultsCount, lexemesAndTags.lexemes, lexemesAndTags.tags);
        }

        [Fact]
        public void ParseKnownPrefix_NotNoun() {
            TestParse("Ru", "квазичетыре", 0, Array.Empty<string>(), Array.Empty<string>());
        }

        [Theory]
        [InlineData("Ru", "супер-псевдо-сверх-экстра-эконом-квази-еврорыбой", "супер-псевдо-сверх-экстра-эконом-квази-еврорыба", "NOUN,anim,femn sing,ablt")]
        public void ParseMultiplePrefixes(string language, string word, string expectedNormalForm, string expectedTag) {
            TestParse(language, word, 3, Enumerable.Repeat(expectedNormalForm, 3).ToArray(), Enumerable.Repeat(expectedTag, 3).ToArray());
        }
        
        [Theory]
        [InlineData("Ru", "псевдо-рыба", 
            "псевдо-рыба=NOUN,anim,femn sing,nomn;" +
            "псевдо-рыбы=NOUN,anim,femn sing,gent;" +
            "псевдо-рыбе=NOUN,anim,femn sing,datv;" +
            "псевдо-рыбу=NOUN,anim,femn sing,accs;" +
            "псевдо-рыбой=NOUN,anim,femn sing,ablt;" +
            "псевдо-рыбою=NOUN,anim,femn sing,ablt,V-oy;" +
            "псевдо-рыбе=NOUN,anim,femn sing,loct;" +
            "псевдо-рыбы=NOUN,anim,femn plur,nomn;" +
            "псевдо-рыб=NOUN,anim,femn plur,gent;" +
            "псевдо-рыбам=NOUN,anim,femn plur,datv;" +
            "псевдо-рыб=NOUN,anim,femn plur,accs;" +
            "псевдо-рыбами=NOUN,anim,femn plur,ablt;" +
            "псевдо-рыбах=NOUN,anim,femn plur,loct")]
        public void GetLexemes(string language, string word, string expectedLexemesWithTags) {
            var lexemesAndTags = ExtractLexemesAndTags(expectedLexemesWithTags);
            TestGetLexemes(language, word, lexemesAndTags.lexemes, lexemesAndTags.tags);
        }
    }
}