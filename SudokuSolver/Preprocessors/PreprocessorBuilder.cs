namespace SudokuSolver.Preprocessors
{
    [Flags]
    public enum PreprocessorOptions { None, Full, Unoptimised }

    public class PreprocessorBuilder
    {
        private static readonly Dictionary<PreprocessorOptions, Func<byte, IPreprocessor>> _solvers = new Dictionary<PreprocessorOptions, Func<byte, IPreprocessor>>()
        {
            { PreprocessorOptions.Full, (b) => new BasicPreprocessor.Preprocessor(
                new BasicPreprocessor.BasicPreprocessorOptions(),
                b) },
            { PreprocessorOptions.Unoptimised, (b) => new BasicPreprocessor.Preprocessor(
                new BasicPreprocessor.BasicPreprocessorOptions()
                {
                    GroundLegalCandidatesOnly = false,
                    PruneCertains = false,
                    PruneHiddenPairs = false,
                    PruneNakedPairs = false
                },
                b) },
        };

        public static IPreprocessor GetPreprocessor(PreprocessorOptions preprocessor, byte boardSize) => _solvers[preprocessor](boardSize);
    }
}
