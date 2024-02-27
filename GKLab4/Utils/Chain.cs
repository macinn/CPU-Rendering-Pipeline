namespace Utils;

public class ProcessingChain<Q, T>
    where T : struct
{
    private readonly List<Func<T, T?>> functions;
    private readonly Func<Q, IEnumerable<T>> preprocessor;
    private Func<T, T?> ChainExecutionPolicy { get; set; }
    public ProcessingChain(Func<Q, IEnumerable<T>> preproc, ICollection<Func<T, T?>> items)
    {
        this.functions = new List<Func<T, T?>>(items);
        this.preprocessor = preproc;

        ChainExecutionPolicy = (T arg) =>
        {
            T? result = arg;

            for (int i = 0; i < functions.Count && result != null; i++)
                result = functions[i](result.Value);

            return result;
        };
    }
    public IEnumerable<T> Execute(Q arg)
    {
        return preprocessor(arg)
            .AsParallel()
            .Select(x => ChainExecutionPolicy(x))
            .Where(x => x.HasValue)
            .Select(x => x!.Value);
    }
}
