namespace MorphAnalyzer {
    internal class NodeDictionary<T> where T : class {
        private string Alphabet { get; }
        private string Punctuation { get; }
        private string Numerals { get; }
        private Node<T> Root;

        public NodeDictionary(string alphabet, string punctuation, string numerals) {
            Alphabet = alphabet;
            Punctuation = punctuation;
            Numerals = numerals;
            Root = new Node<T>(this, false, null);
        }
        
        public void Add(string key, T value) {
            AddCore(Root, key.ToCharArray(), 0, value);
        }

        private void AddCore(Node<T> current, char[] key, int offset, T value) {
            if(offset == key.Length) {
                current.Value = value;
                current.IsTerminal = true;
                return;
            }

            var newNode = current.GetEdge(key[offset]);
            if(newNode == null) {
                newNode = new Node<T>(this, false, null);
                current.SetEdge(key[offset], newNode);
            }
            AddCore(newNode, key, offset + 1, value);
        }

        public bool ContainsKey(string key) {
            return TryGetKeyCore(Root, key.ToCharArray(), 0) != null;
        }

        private Node<T> TryGetKeyCore(Node<T> current, char[] key, int offset) {
            if(current == null) {
                return null;
            }

            if(offset == key.Length) {
                return current.IsTerminal ? current : null;
            }

            return TryGetKeyCore(current.GetEdge(key[offset]), key, offset + 1);
        }

        public T this[string key] => TryGetKeyCore(Root, key.ToCharArray(), 0)?.Value;

        public void WriteDictionary(BinaryWriter writer, Action<BinaryWriter, T> valueWriter) {
            var queue = new Queue<Node<T>>();
            queue.Enqueue(Root);
            var list = new List<Node<T>> { Root };

            var edgesSet = Alphabet + Numerals + Punctuation;
            while(queue.Count != 0) {
                var node = queue.Dequeue();
                writer.Write(node.IsTerminal);
                if(node.IsTerminal)
                    valueWriter.Invoke(writer, node.Value);
                
                var nextNodeChars = new List<char>();
                var nextNodeIndexes = new List<int>();
                foreach(var c in edgesSet) {
                    var v = node.GetEdge(c);
                    if(v == null)
                        continue;
                    
                    queue.Enqueue(v);
                    list.Add(v);
                    nextNodeChars.Add(c);
                    nextNodeIndexes.Add(list.Count - 1);
                }
                
                writer.Write(nextNodeIndexes.Count);
                for(int i = 0; i < nextNodeIndexes.Count - 1; ++i) {
                    writer.Write(nextNodeChars[i]);
                    writer.Write(nextNodeIndexes[i]);
                }
            }
        }

        public void ReadDictionary(BinaryReader reader, Func<BinaryReader, T> valueReader) {
            var nodeList = new List<Node<T>>();
            var nodeEdgeChars = new List<List<char>>();
            var nodeEdgeIndexes = new List<List<int>>();
            while(reader.BaseStream.Position != reader.BaseStream.Length) {
                var node = new Node<T>(this, false, null);
                var terminal = reader.ReadBoolean();
                if(terminal) {
                    node.IsTerminal = true;
                    node.Value = valueReader.Invoke(reader);
                }
                    
                nodeList.Add(node);
                nodeEdgeChars.Add(new List<char>());
                nodeEdgeIndexes.Add(new List<int>());
                
                var index = nodeEdgeChars.Count - 1;
                var size = reader.ReadInt32();
                for(int i = 0; i < size; ++i) {
                    nodeEdgeChars[index].Add(reader.ReadChar());
                    nodeEdgeIndexes[index].Add(reader.ReadInt32());
                }
            }

            for(int i = 0; i < nodeList.Count; ++i) {
                for(int j = 0; j < nodeEdgeChars[i].Count; ++i) {
                    nodeList[i].SetEdge(nodeEdgeChars[i][j], nodeList[nodeEdgeIndexes[i][j]]);
                }
            }
        }
        
        private class Node<T> where T : class {
            private NodeDictionary<T> Owner { get; }

            private Node<T>[] Edges { get; }
            
            public bool IsTerminal { get; set; }
            
            public T Value { get; set; }

            public Node(NodeDictionary<T> owner, bool hasValue, T value) {
                Owner = owner;
                Edges = Enumerable
                    .Repeat<Node<T>>(null, Owner.Alphabet.Length + Owner.Numerals.Length + Owner.Punctuation.Length)
                    .ToArray();
                if(hasValue) {
                    IsTerminal = true;
                    Value = value;
                }
            }

            public void SetEdge(char c, Node<T> to) {
                Edges[GetEdgeIndex(c)] = to;
            }

            public Node<T> GetEdge(char c) {
                var index = GetEdgeIndex(c, false);
                return index != -1 ? Edges[index] : null;
            }

            private int GetEdgeIndex(char c, bool throwException = true) {
                if(Owner.Alphabet.Contains(c))
                    return Owner.Alphabet.IndexOf(c);
                if(Owner.Numerals.Contains(c))
                    return Owner.Alphabet.Length + Owner.Numerals.IndexOf(c);
                if(Owner.Punctuation.Contains(c))
                    return Owner.Alphabet.Length + Owner.Numerals.Length + Owner.Punctuation.IndexOf(c);

                if(throwException) {
                    throw new Exception($"Char {c} is not supported in word graph");
                }
                return -1;
            }
        }
    }
}