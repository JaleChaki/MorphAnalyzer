using System;
using MorphAnalyzer.AnalyzerUnits;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace MorphAnalyzer.Tests.AnalyzerUnits {
    public class DictionaryAnalyzerUnitTests : AnalyzerUnitTester<DictionaryAnalyzerUnit> {
        
        [Theory]
        [InlineData("Ru", "рыбой", 1, "рыба", "NOUN,anim,femn sing,ablt")]
        [InlineData("Ru", "пересекая", 1, "пересекать", "GRND,impf,tran pres")]
        [InlineData("Ru", "три", 3, "три;три;тереть", "NUMR nomn;NUMR inan,accs;VERB,impf,tran sing,impr,excl")]
        public void ParseKnownWord(string language, string word, int expectedResultsCount, string expectedNormalForms, string expectedTags) {
            TestParse(language, word, expectedResultsCount, expectedNormalForms.Split(';'), expectedTags.Split(';'));
        }
        
        [Fact]
        public void ParseUnknownWord() {
            TestParse("Ru", "брюзеглявый", 0, Array.Empty<string>(), Array.Empty<string>());
        }

        [Theory]
        [InlineData("Ru", "рыба", 
            "рыба=NOUN,anim,femn sing,nomn;" +
            "рыбы=NOUN,anim,femn sing,gent;" +
            "рыбе=NOUN,anim,femn sing,datv;" +
            "рыбу=NOUN,anim,femn sing,accs;" +
            "рыбой=NOUN,anim,femn sing,ablt;" +
            "рыбою=NOUN,anim,femn sing,ablt,V-oy;" +
            "рыбе=NOUN,anim,femn sing,loct;" +
            "рыбы=NOUN,anim,femn plur,nomn;" +
            "рыб=NOUN,anim,femn plur,gent;" +
            "рыбам=NOUN,anim,femn plur,datv;" +
            "рыб=NOUN,anim,femn plur,accs;" +
            "рыбами=NOUN,anim,femn plur,ablt;" +
            "рыбах=NOUN,anim,femn plur,loct")]
        public void GetLexemes(string language, string word, string expectedLexemesWithTags) {
            var lexemesAndTags = ExtractLexemesAndTags(expectedLexemesWithTags);
            TestGetLexemes(language, word, lexemesAndTags.lexemes, lexemesAndTags.tags);
        }
    }
}