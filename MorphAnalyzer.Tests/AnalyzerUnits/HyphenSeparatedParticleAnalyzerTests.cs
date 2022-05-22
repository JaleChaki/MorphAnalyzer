using System;
using MorphAnalyzer.AnalyzerUnits;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace MorphAnalyzer.Tests.AnalyzerUnits {
    public class HyphenSeparatedParticleAnalyzerTests : AnalyzerUnitTester<HyphenSeparatedParticleAnalyzer> {

        [Theory]
        [InlineData("Ru", "смотри-ка", 1, "смотреть-ка", "VERB,impf,tran sing,impr,excl")]
        [InlineData("Ru", "дома-таки", 4, "дома-таки;дом-таки;дом-таки;дом-таки", "ADVB;NOUN,inan,masc sing,gent;NOUN,inan,masc plur,nomn;NOUN,inan,masc plur,accs")]
        public void ParseKnownParticle(string language, string word, int expectedResultsCount, string expectedNormalForms, string expectedTags) {
            TestParse(language, word, expectedResultsCount, expectedNormalForms.Split(';'), expectedTags.Split(';'));
        }

        [Fact]
        public void ParseUnknownParticle() {
            TestParse("Ru", "два-ться", 0, Array.Empty<string>(), Array.Empty<string>());
        }

        [Theory]
        [InlineData("Ru", "рыба-таки", 
            "рыба-таки=NOUN,anim,femn sing,nomn;" +
            "рыбы-таки=NOUN,anim,femn sing,gent;" +
            "рыбе-таки=NOUN,anim,femn sing,datv;" +
            "рыбу-таки=NOUN,anim,femn sing,accs;" +
            "рыбой-таки=NOUN,anim,femn sing,ablt;" +
            "рыбою-таки=NOUN,anim,femn sing,ablt,V-oy;" +
            "рыбе-таки=NOUN,anim,femn sing,loct;" +
            "рыбы-таки=NOUN,anim,femn plur,nomn;" +
            "рыб-таки=NOUN,anim,femn plur,gent;" +
            "рыбам-таки=NOUN,anim,femn plur,datv;" +
            "рыб-таки=NOUN,anim,femn plur,accs;" +
            "рыбами-таки=NOUN,anim,femn plur,ablt;" +
            "рыбах-таки=NOUN,anim,femn plur,loct")]
        public void GetLexemes(string language, string word, string expectedLexemesWithTags) {
            var lexemesAndTags = ExtractLexemesAndTags(expectedLexemesWithTags);
            TestGetLexemes(language, word, lexemesAndTags.lexemes, lexemesAndTags.tags);
        }
        
    }
}