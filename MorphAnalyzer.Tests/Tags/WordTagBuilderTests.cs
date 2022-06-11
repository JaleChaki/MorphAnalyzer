using Xunit;

// ReSharper disable StringLiteralTypo
namespace MorphAnalyzer.Tests.Tags {
    public class WordTagBuilderTests {

        [Theory]
        [InlineData("NOUN,anim,femn sing,ablt", 
            PartOfSpeech.Noun, Animacy.Animated, null, Gender.Feminine, Number.Single, Case.Ablative, 
            null, null, null, null, null, null)]
        [InlineData("VERB,impf,tran sing,impr,excl",
            PartOfSpeech.Verb, null, Aspect.Imperfect, null, Number.Single, null, 
            Transitivity.Transitive, null, null, Involvement.Exclude, Mood.Imperative, null)]
        public void Build(string tagInString,
            PartOfSpeech expectedPartOfSpeech, 
            // ReSharper disable once IdentifierTypo
            Animacy? expectedAnimacy = null,
            Aspect? expectedAspect = null,
            Gender? expectedGender = null,
            Number? expectedNumber = null,
            Case? expectedCase = null,
            Transitivity? expectedTransitivity = null,
            Person? expectedPerson = null,
            Tense? expectedTense = null,
            Involvement? expectedInvolvement = null,
            Mood? expectedMood = null,
            Voice? expectedVoice = null) {
            
            var tag = WordTagBuilder.Build(tagInString);
            Assert.Equal(expectedPartOfSpeech, tag.PartOfSpeech);
            Assert.Equal(expectedAnimacy, tag.Animacy);
            Assert.Equal(expectedAspect, tag.Aspect);
            Assert.Equal(expectedGender, tag.Gender);
            Assert.Equal(expectedNumber, tag.Number);
            Assert.Equal(expectedCase, tag.Case);
            Assert.Equal(expectedTransitivity, tag.Transitivity);
            Assert.Equal(expectedPerson, tag.Person);
            Assert.Equal(expectedTense, tag.Tense);
            Assert.Equal(expectedInvolvement, tag.Involvement);
            Assert.Equal(expectedMood, tag.Mood);
            Assert.Equal(expectedVoice, tag.Voice);
        }
        
        [Fact]
        public void ThrowsExceptionIfPartOfSpeechNotExists() {
            var exception = Record.Exception(delegate {
                WordTagBuilder.Build("anim,masc");
            });
            Assert.NotNull(exception);
            Assert.Contains("Part of speech not found", exception.Message);
        }
    }
}