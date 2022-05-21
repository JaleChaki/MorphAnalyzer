using DawgSharp;
#if NEWTONSOFT_JSON
using Newtonsoft.Json;
#else
using System.Text.Json;
#endif

namespace MorphAnalyzer {
    internal class LanguageDictionaryReader {
        private const string PARADIGM_PREFIXES_FILENAME = "paradigm_prefixes.json";
        private const string PARADIGMS_FILENAME = "paradigms.json";
        private const string SUFFIX_FILENAME = "suffixes.json";
        private const string WORDS_FILENAME = "words.dawg";
        // ReSharper disable once IdentifierTypo
        private const string GRAMTAB_FILENAME = "gramtab-opencorpora.json";
        private const string PROBABILITIES_FILENAME = "probs.dawg";
        private const string KNOWN_PREFIXES_FILENAME = "prefixes.json";
        private const string HYPHENS_FILENAME = "hyphens.json";
        
        public static LanguageDictionary Read(string path) {
            var paradigmPrefixes =
                JsonConvert.DeserializeObject<string[]>(
                    File.ReadAllText(Path.Combine(path, PARADIGM_PREFIXES_FILENAME))
                );

            var paradigms = 
                ReadAndDeserializeJson<List<List<int>>>(Path.Combine(path, PARADIGMS_FILENAME))
                    .Select(x => new Paradigm(x))
                    .ToList();

            var suffixes = ReadAndDeserializeJson<string[]>(Path.Combine(path, SUFFIX_FILENAME));

            var tags = 
                ReadAndDeserializeJson<string[]>(Path.Combine(path, GRAMTAB_FILENAME))
                    .Select(WordTagBuilder.Build)
                    .ToArray();

            var words = 
                Dawg<int[]>.Load(File.Open(Path.Combine(path, WORDS_FILENAME), FileMode.Open), ReadArray);
            
            var probabilities = Dawg<int>.Load(File.Open(Path.Combine(path, PROBABILITIES_FILENAME), FileMode.Open));

            var knownPrefixes = ReadAndDeserializeJson<string[]>(Path.Combine(path, KNOWN_PREFIXES_FILENAME));

            var hyphens = ReadAndDeserializeJson<string[]>(Path.Combine(path, HYPHENS_FILENAME));
            
            return new LanguageDictionary(suffixes, paradigms, paradigmPrefixes, tags, words, probabilities, knownPrefixes, hyphens);
        }

        private static T ReadAndDeserializeJson<T>(string path) {
            if(!File.Exists(path))
                throw new FileNotFoundException("File not found", path);

            var content = File.ReadAllText(path);
            try {
#if NEWTONSOFT_JSON
                return JsonConvert.DeserializeObject<T>(content);
#else
                return JsonSerializer.Deserialize<T>(content);
#endif
            }
            catch(Exception e) {
                throw new Exception($"Error at parsing file {path}. See inner exception for details", e);
            }
        }
        
        private static int[] ReadArray(BinaryReader reader) {
            int arrayLength = reader.ReadByte();
            return DecompressShorts2(arrayLength);

            int[] DecompressShorts(int length) {
                var result = new List<int>(length * 2);
                var current = -1;
                for(int i = 0; i < length; ++i) {
                    var value = reader.ReadInt16();
                    if(value < 0) {
                        current = -value;
                    }
                    result.Add(current);
                    result.Add(reader.ReadInt16());
                }

                return result.ToArray();
            }
            
            int[] DecompressShorts2(int length) {
                var result = new List<int>(length * 2);
                for(int i = 0; i < length; ++i) {
                    result.Add(reader.ReadInt16());
                }

                return result.ToArray();
            }

            int[] DecompressBytes(int length) {
                var result = new List<int>(length);
                for(int i = 0; i < length; ++i) {
                    result.Add(reader.ReadByte());
                }

                return result.ToArray();
            }

            int[] DecompressBytesWithDuplicates(int length) {
                var primaryValue = reader.ReadByte();
                var result = new List<int>(length * 2);
                for(int i = 0; i < length; ++i) {
                    result.Add(primaryValue);
                    result.Add(reader.ReadByte());
                }

                return result.ToArray();
            }
        }
    }
}