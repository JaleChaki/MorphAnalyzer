using System;
using System.Linq;
using MorphAnalyzer.AnalyzerUnits;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace MorphAnalyzer.Tests.AnalyzerUnits {
    public class HyphenSeparatedParticleAnalyzerTests {

        [Theory]
        [InlineData("Ru", "смотри-ка", "смотреть", "VERB,impf,tran sing,impr,excl")]
        public void ParseKnownParticle_SingleSignificance(string language, string word, string expectedNormalForm, string expectedTag) {
            var analyzerUnit = Utils.GetAnalyzerUnit<HyphenSeparatedParticleAnalyzer>(language);
            var significances = analyzerUnit.Parse(word, Array.Empty<IMorphAnalyzerUnit>());
            Assert.Equal(1, significances.Count);

            var singleSignificance = significances[0];
            Assert.Equal(expectedNormalForm, singleSignificance.NormalForm);
            Assert.Equal(expectedTag, singleSignificance.Tag.DebugValue);
        }
        
        [Theory]
        [InlineData("Ru", "дома-таки", 4, "дома;дом;дом;дом", "ADVB;NOUN,inan,masc sing,gent;NOUN,inan,masc plur,nomn;NOUN,inan,masc plur,accs")]
        public void ParseKnownParticle_MultipleSignificances(string language, string word, int expectedResultsCount, string expectedNormalForms, string expectedTags) {
            
            var analyzerUnit = Utils.GetAnalyzerUnit<HyphenSeparatedParticleAnalyzer>(language);
            var significances = analyzerUnit.Parse(word, Array.Empty<IMorphAnalyzerUnit>());
            Assert.Equal(expectedResultsCount, significances.Count);

            var normalForms = expectedNormalForms.Split(';');
            var tags = expectedTags.Split(';');

            Assert.Equal(expectedResultsCount, normalForms.Length);
            Assert.Equal(expectedResultsCount, tags.Length);
            
            for(int i = 0; i < expectedResultsCount; ++i) {
                Assert.Equal(normalForms[i], significances[i].NormalForm);
                Assert.Equal(tags[i], significances[i].Tag.DebugValue);
            }
        }

        [Fact]
        public void ParseUnknownParticle() {
            var analyzerUnit = Utils.GetAnalyzerUnit<HyphenSeparatedParticleAnalyzer>("Ru");
            var significances = analyzerUnit.Parse("два-ться", Array.Empty<IMorphAnalyzerUnit>());
            Assert.Empty(significances);
        }

        [Theory]
        [InlineData("Ru", "рыба-таки", "рыба-таки;рыбы-таки;рыбе-таки;рыбу-таки;рыбой-таки;рыбою-таки;рыбе-таки;рыбы-таки;рыб-таки;рыбам-таки;рыб-таки;рыбами-таки;рыбах-таки")]
        public void GetLexemes(string language, string word, string expectedLexemes) {
            var analyzerUnit = Utils.GetAnalyzerUnit<HyphenSeparatedParticleAnalyzer>(language);
            var significances = analyzerUnit.Parse(word, Array.Empty<IMorphAnalyzerUnit>());
            var lexemes = analyzerUnit.GetLexemes(significances[0]);
            
            Assert.Equal(expectedLexemes, string.Join(';', lexemes.Select(lexeme => lexeme.RawWord)));
        }
        
    }
}