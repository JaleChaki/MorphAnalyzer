# MorphAnalyzer
Morphological analyzer for RU and UK (in future) language. Written based on the ideas in the ![pymorphy2](https://github.com/kmike/pymorphy2).

## Features

1. Build normal form

```c#
var fishParse = Analyzer.Parse("рыбе").First();
var normalForm = fishParse.NormalForm; // рыба
```

2. Inflect word by specified parameters

```c#
var fishInPluralAndAccusative = Analyzer.Inflect(fishParse, 
  new InflectOptions {
    Number = Number.Plural,  // plural number and accusative case
    Case = Case.Accusative
  } 
); 
var s = fishInPluralAndAccusative.RawWord; // рыб
```

3. Get grammar info about word (part of speech, animacy, aspect, case, gender, involvement, mood, number, person, tense, transitivity and voice)

```c#
fishParse.PartofSpeech;         // PartofSpeech.Noun
fishParse.Animacy;		// Animacy.Animated
fishParse.Aspect;		// null (null value means that the aspect is not specified or not applicable for this word)
fishParse.Case;			// Case.Dative
fishParse.Gender;		// Gender.Feminine
fishParse.Number;		// Number.Single
```

## How it works

It uses OpenCorpora dictionary, for unknown words makes predictions, based on hyphens, known prefixes e.t.c. Most of the implementation is taken from ![pymorphy2](https://github.com/kmike/pymorphy2).

## Future plans

1. UK dictionary
2. Improve prediction for unknown words
3. Docs and Nuget package maybe...
