namespace MorphAnalyzer.AnalyzerUnits {
    internal struct ParadigmLink {
        public int Index { get; }
        public int InternalIndex { get; }
        
        public ParadigmLink(int index, int internalIndex) {
            Index = index;
            InternalIndex = internalIndex;
        }
    }
}