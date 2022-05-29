using System.Collections.Generic;
using System.IO;
using System.Linq;
using MorphAnalyzer.AnalyzerUnits;

namespace MorphAnalyzer.Tests {
    internal static class Utils {

        private const string DICTIONARIES_PATH = "Dictionaries";
        
        private static Dictionary<string, List<IMorphAnalyzerUnit>> AnalyzerUnits { get; set; }
        
        private static Dictionary<string, LanguageDictionary> Dictionaries { get; set; }

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static object _syncRoot = new object();

        public static T GetAnalyzerUnit<T>(string language) where T : IMorphAnalyzerUnit {
            lock(_syncRoot) {
                language = language.ToLower();
                AnalyzerUnits ??= new Dictionary<string, List<IMorphAnalyzerUnit>>();
                if(!AnalyzerUnits.ContainsKey(language))
                    CreateAnalyzerUnitsForSpecificLanguage(language);

                var result = AnalyzerUnits[language].First(x => x.GetType() == typeof(T));
                return (T) result;
            }
        }

        public static LanguageDictionary GetLanguageDictionary(string language) {
            lock(_syncRoot) {
                Dictionaries ??= new Dictionary<string, LanguageDictionary>();
                language = language.ToLower();
                if(!Dictionaries.ContainsKey(language))
                    Dictionaries.Add(language,
                        LanguageDictionaryReader.Read(Path.Combine(DICTIONARIES_PATH, language)));
                return Dictionaries[language];
            }
        }
        
        private static void CreateAnalyzerUnitsForSpecificLanguage(string language) {
            language = language.ToLower();
            var dictionary = GetLanguageDictionary(language);
            AnalyzerUnits ??= new Dictionary<string, List<IMorphAnalyzerUnit>>();
            var dictionaryAnalyzer = new DictionaryAnalyzer(dictionary);
            var composite = new AnalyzerUnitComposite();
            var list = new List<IMorphAnalyzerUnit> {
                dictionaryAnalyzer,
                new KnownPrefixAnalyzer(composite, dictionary.KnownPrefixes),
                new HyphenSeparatedParticleAnalyzer(composite, dictionary.Hyphens),
                new HyphenAdverbAnalyzer(composite),
                new HyphenatedWordsAnalyzer(composite, dictionary.KnownPrefixes),
                new UnknownPrefixAnalyzer(composite)
            };
            composite.AnalyzerUnits = list;
            
            AnalyzerUnits.Add(language, list);
        }
        
    }
}