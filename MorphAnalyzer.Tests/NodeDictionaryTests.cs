using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DawgSharp;
using Newtonsoft.Json;
using Xunit;

namespace MorphAnalyzer.Tests {
    public class NodeDictionaryTests {
        private NodeDictionary<string> Dictionary { get; }

        public NodeDictionaryTests() {
            Dictionary = new NodeDictionary<string>("абвгдеёжзийклмнопрстуфхцчшщъыьэюя", "0123456789", "!\"#$%&\\'()*+,-./:;<=>?@[\\\\]^_`{|}~’");
        }
        
        [Fact]
        public void CommonKeys() {
            var key1 = "абажур";
            var key2 = "абажуровый";
            
            Dictionary.Add(key1, "val");
            Dictionary.Add(key2, "val2");
            Assert.Equal("val", Dictionary[key1]);
            Assert.Equal("val2", Dictionary[key2]);
        }

        [Fact]
        public void SpecialSymbols() {
            var specialSymbolKey = "д1-=);,~`";
                
            Dictionary.Add(specialSymbolKey, "1");
            Assert.True(Dictionary.ContainsKey(specialSymbolKey));
        }

        [Fact]
        public void WrongAlphabet() {
            var addException = Record.Exception(delegate {
                Dictionary.Add("latin", string.Empty);
            });
            var getException = Record.Exception(delegate {
                Assert.False(Dictionary.ContainsKey("latin"));
            });
            
            Assert.Contains("not supported", addException.Message);
            Assert.Null(getException);
        }

        [Fact]
        public void ProbWrite() {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(@"probs.json"));

            
            var dawgBuilder = new DawgBuilder<int>();
            foreach(var kv in dict) {
                dawgBuilder.Insert(kv.Key, kv.Value);
            }
            using(var file = File.Open("probs.dawg", FileMode.OpenOrCreate)) {
                dawgBuilder.BuildDawg().SaveTo(file);
            }
        }
        
        [Fact]
        public void BinaryWrite() {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, int[]>>(
                File.ReadAllText(@"Dictionaries/Ru/words.json")
            );

            var dawgBuilder = new DawgBuilder<int[]>();
            foreach(var kv in dict) {

                /*for(int i = 0; i < kv.Value.Length; i += 2) {
                    if(i > 127)
                }*/
                if(kv.Value.Length > 126)
                    throw new Exception(kv.Key);
                
                dawgBuilder.Insert(kv.Key, kv.Value);
            }

            using(var file = File.Open("words.dawg6", FileMode.OpenOrCreate)) {
                dawgBuilder.BuildDawg().SaveTo(file, ArrayWriter);
            }

            void ArrayWriter(BinaryWriter writer, int[] array) {
                //var target = Compress(array);
                var target = array;
                /*if(CanCompressToByte(array)) {
                    target = array;
                    var lengthIdentifier = 128 + target.Length;
                    bool canCompressValues = lengthIdentifier > 2;
                    
                    for(int i = 2; i < array.Length; i += 2) {
                        if(array[i] != array[i - 2])
                            canCompressValues = false;
                    }

                    if(canCompressValues) {
                        lengthIdentifier += 1;
                        target = new int[array.Length / 2 + 1];
                        target[0] = array[0];
                        int y = 1;
                        for(int i = 1; i < array.Length; i += 2) {
                            target[y++] = array[i];
                        }
                    }
                    
                    writer.Write((byte)lengthIdentifier);
                    foreach(var e in target) {
                        writer.Write((byte)e);
                    }
                } else {*/
                    writer.Write((byte) target.Length);
                    foreach(var e in target) {
                        writer.Write((short) e);
                    }
                /*}*/
            }
        }

        [Fact]
        public void Check() {
            var a = new[] {10, 0, 10, 1, 10, 2, 20, 6, 20, 7, 30, 1};
            var c = Compress(a);
            var b = 0;
        }

        bool CanCompressToByte(int[] array) {
            return array.All(x => x < byte.MaxValue);
        }
        
        int[] Compress(int[] array) {
            int current = -1;
            var target = new List<int>();
            for(int i = 0; i < array.Length; ++i) {
                if(i % 2 == 0) {
                    if(array[i] != current) {
                        current = array[i];
                        target.Add(-current);
                    }
                    continue;
                }
                target.Add(array[i]);
            }
            return target.ToArray();
        }
        
        [Fact]
        public void BinaryRead() {
            
        }
    }
}