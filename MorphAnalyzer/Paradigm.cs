using System.Collections;

namespace MorphAnalyzer {
    
    internal class Paradigm : IReadOnlyList<int> {
        private IReadOnlyList<int> Values { get; }

        public Paradigm(IReadOnlyList<int> internalList) {
            Values = internalList;
        }
        
#region IReadOnlyList members
        public IEnumerator<int> GetEnumerator() {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable) Values).GetEnumerator();
        }

        public int Count => Values.Count;

        public int this[int index] => Values[index];
#endregion
    }
}