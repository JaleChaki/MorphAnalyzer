using MorphAnalyzer.AnalyzerUnits;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace MorphAnalyzer.Tests.AnalyzerUnits {
    public class HyphenatedWordsAnalyzerTests : AnalyzerUnitTester<HyphenatedWordsAnalyzer> {

        [Theory]
        [InlineData("Ru", "человеком-горой", "человек-гора", "NOUN,anim,masc sing,ablt")]
        public void Parse_AsVariableBoth(string language, string word, string expectedResult, string expectedTag) {
            TestParse(language, word, 1, new [] { expectedResult }, new [] { expectedTag });
        }

        [Theory]
        [InlineData("Ru", "человек-гора", 
            "человек-гора=NOUN,anim,masc sing,nomn;" +
            "человека-горы=NOUN,anim,masc sing,gent;" +
            "человеку-горе=NOUN,anim,masc sing,datv;" +
            "человека-гору=NOUN,anim,masc sing,accs;" +
            "человеком-горой=NOUN,anim,masc sing,ablt;" +
            "человеком-горою=NOUN,anim,masc sing,ablt;" +
            "человеке-горе=NOUN,anim,masc sing,loct;" +
            "люди-горы=NOUN,anim,masc plur,nomn;" +
            "людей-гор=NOUN,anim,masc plur,gent;" +
            "людям-горам=NOUN,anim,masc plur,datv;" +
            "людей-горы=NOUN,anim,masc plur,accs;" +
            "людьми-горами=NOUN,anim,masc plur,ablt;" +
            "людях-горах=NOUN,anim,masc plur,loct")]
        public void GetLexemes_AsVariableBoth(string language, string word, string expectedLexemesAndTags) {
            var lexemesAndTags = ExtractLexemesAndTags(expectedLexemesAndTags);
            TestGetLexemes(language, word, lexemesAndTags.lexemes, lexemesAndTags.tags);
        }

        [Theory]
        [InlineData("Ru", "воздушно-горного", 
            "воздушно-горный=ADJF masc,sing,gent;" +
            "воздушно-горный=ADJF anim,masc,sing,accs;" +
            "воздушно-горный=ADJF neut,sing,gent")]
        public void Parse_AsFixedLeft(string language, string word, string expectedLexemesAndTags) {
            var lexemesAndTags = ExtractLexemesAndTags(expectedLexemesAndTags);
            TestParse(language, word, lexemesAndTags.lexemes.Length, lexemesAndTags.lexemes, lexemesAndTags.tags);
        }
        
    }
}