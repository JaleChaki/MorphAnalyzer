using System;
using System.Linq;
using MorphAnalyzer.AnalyzerUnits;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace MorphAnalyzer.Tests.AnalyzerUnits {
    public class KnownPrefixAnalyzerUnitTests {

        [Theory]
        [InlineData("Ru", "квазирыбой", "квазирыба", "NOUN,anim,femn sing,ablt")]
        public void ParseKnownPrefix_SingleSignificance(string language, string word, string expectedNormalForm, string expectedTag) {
            var analyzerUnit = Utils.GetAnalyzerUnit<KnownPrefixAnalyzerUnit>(language);
            var significances = analyzerUnit.Parse(word, Array.Empty<IMorphAnalyzerUnit>());
            
            Assert.Equal(1, significances.Count);
            Assert.Equal(expectedNormalForm, significances[0].NormalForm);
            Assert.Equal(expectedTag, significances[0].Tag.DebugValue);
        }

        [Theory]
        [InlineData("Ru", "космодома", 4, "космодома;космодом;космодом;космодом", "ADVB;NOUN,inan,masc sing,gent;NOUN,inan,masc plur,nomn;NOUN,inan,masc plur,accs")]
        public void ParseKnownPrefix_MultipleSignificances(string language, string word, int expectedResultsCount, string expectedNormalForms, string expectedTags) {
            var analyzerUnit = Utils.GetAnalyzerUnit<KnownPrefixAnalyzerUnit>(language);
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
        public void ParseKnownPrefix_NotNoun() {
            var analyzerUnit = Utils.GetAnalyzerUnit<KnownPrefixAnalyzerUnit>("Ru");
            var significances = analyzerUnit.Parse("квазичетыре", Array.Empty<IMorphAnalyzerUnit>());
            Assert.Empty(significances);
        }

        [Theory]
        [InlineData("Ru", "супер-псевдо-сверх-экстра-эконом-квази-еврорыбой", "супер-псевдо-сверх-экстра-эконом-квази-еврорыба", "NOUN,anim,femn sing,ablt")]
        public void ParseMultiplePrefixes(string language, string word, string expectedNormalForm, string expectedTag) {
            var analyzerUnit = Utils.GetAnalyzerUnit<KnownPrefixAnalyzerUnit>(language);
            var significances = analyzerUnit.Parse(word, Array.Empty<IMorphAnalyzerUnit>());
            
            Assert.Equal(expectedNormalForm, significances[0].NormalForm);
            Assert.Equal(expectedTag, significances[0].Tag.DebugValue);
        }
        
        [Theory]
        [InlineData("Ru", "псевдо-рыба", "псевдо-рыба;псевдо-рыбы;псевдо-рыбе;псевдо-рыбу;псевдо-рыбой;псевдо-рыбою;псевдо-рыбе;псевдо-рыбы;псевдо-рыб;псевдо-рыбам;псевдо-рыб;псевдо-рыбами;псевдо-рыбах")]
        public void GetLexemes(string language, string word, string expectedLexemes) {
            var analyzerUnit = Utils.GetAnalyzerUnit<KnownPrefixAnalyzerUnit>(language);
            var significances = analyzerUnit.Parse(word, Array.Empty<IMorphAnalyzerUnit>());
            var lexemes = analyzerUnit.GetLexemes(significances[0]);
            
            Assert.Equal(expectedLexemes, string.Join(';', lexemes.Select(lexeme => lexeme.RawWord)));
        }
    }
}