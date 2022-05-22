using System;
using System.Collections.Generic;
using System.Linq;
using MorphAnalyzer.AnalyzerUnits;
using Xunit;

namespace MorphAnalyzer.Tests.AnalyzerUnits {
    public abstract class AnalyzerUnitTester<T> where T : IMorphAnalyzerUnit {
        
        protected void TestParse(string language, string word, int expectedResultsCount, IReadOnlyList<string> expectedNormalForms, IReadOnlyList<string> expectedTags) {

            var analyzerUnit = GetAnalyzerUnit(language);
            var significances = analyzerUnit.Parse(word, Array.Empty<IMorphAnalyzerUnit>());
            
            Assert.Equal(expectedResultsCount, significances.Count);
            Assert.Equal(expectedResultsCount, expectedNormalForms.Count);
            Assert.Equal(expectedResultsCount, expectedTags.Count);

            for(int i = 0; i < expectedResultsCount; ++i) {
                Assert.Equal(expectedNormalForms[i], significances[i].NormalForm);
                Assert.Equal(expectedTags[i], significances[i].Tag.DebugValue);
                Assert.Equal(analyzerUnit, significances[i].Method);
            }
        }

        protected void TestGetLexemes(string language, string word, IReadOnlyList<string> expectedLexemes, IReadOnlyList<string> expectedTags) {
            IMorphAnalyzerUnit analyzerUnit = GetAnalyzerUnit(language);
            var significances = analyzerUnit.Parse(word, Array.Empty<IMorphAnalyzerUnit>());
            var lexemes = analyzerUnit.GetLexemes(significances[0]).ToArray();
            
            Assert.True(expectedLexemes.SequenceEqual(lexemes.Select(lexeme => lexeme.RawWord)));
            Assert.True(expectedTags.SequenceEqual(lexemes.Select(lexeme => lexeme.Tag.DebugValue)));
            Assert.True(lexemes.All(lexeme => lexeme.Method == analyzerUnit));
        }

        protected virtual T GetAnalyzerUnit(string language) {
            return Utils.GetAnalyzerUnit<T>(language);
        }

        protected static (string[] lexemes, string[] tags) ExtractLexemesAndTags(string lexemesWithTags) {
            var lexemesWithTagsSplit = lexemesWithTags.Split(';');
            var lexemes = lexemesWithTagsSplit.Select(x => x.Split('=')[0]).ToArray();
            var tags = lexemesWithTagsSplit.Select(x => x.Split('=')[1]).ToArray();
            return (lexemes, tags);
        } 
    }
}