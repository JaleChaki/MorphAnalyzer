using System;
using System.Linq;
using MorphAnalyzer.AnalyzerUnits;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace MorphAnalyzer.Tests.AnalyzerUnits {
    public class DictionaryAnalyzerUnitTests {
        
        [Theory]
        [InlineData("Ru", "рыбой", "рыба", "NOUN,anim,femn sing,ablt")]
        [InlineData("Ru", "пересекая", "пересекать", "GRND,impf,tran pres")]
        public void ParseKnownWord_SingleSignificance(string language, string word, string expectedNormalForm, string expectedTag) {
            var analyzerUnit = Utils.GetAnalyzerUnit<DictionaryAnalyzerUnit>(language);
            var significances = analyzerUnit.Parse(word, Array.Empty<IMorphAnalyzerUnit>());
            Assert.Equal(1, significances.Count);

            var singleSignificance = significances[0];
            Assert.Equal(expectedNormalForm, singleSignificance.NormalForm);
            Assert.Equal(expectedTag, singleSignificance.Tag.DebugValue);
        }

        [Theory]
        [InlineData("Ru", "три", 3, "три;три;тереть", "NUMR nomn;NUMR inan,accs;VERB,impf,tran sing,impr,excl")]
        public void ParseKnownWord_MultipleSignificances(string language, string word, int expectedResultsCount, string expectedNormalForms, string expectedTags) {
            
            var analyzerUnit = Utils.GetAnalyzerUnit<DictionaryAnalyzerUnit>(language);
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
        public void ParseUnknownWord() {
            var analyzerUnit = Utils.GetAnalyzerUnit<DictionaryAnalyzerUnit>("Ru");
            Assert.Empty(analyzerUnit.Parse("брюзеглявый", Array.Empty<IMorphAnalyzerUnit>()));
        }

        [Theory]
        [InlineData("Ru", "рыба", "рыба;рыбы;рыбе;рыбу;рыбой;рыбою;рыбе;рыбы;рыб;рыбам;рыб;рыбами;рыбах")]
        public void GetLexemes(string language, string word, string expectedLexemes) {
            var analyzerUnit = Utils.GetAnalyzerUnit<DictionaryAnalyzerUnit>(language);
            var significances = analyzerUnit.Parse(word, Array.Empty<IMorphAnalyzerUnit>());
            var lexemes = analyzerUnit.GetLexemes(significances[0]);
            
            Assert.Equal(expectedLexemes, string.Join(';', lexemes.Select(lexeme => lexeme.RawWord)));
        }
    }
}