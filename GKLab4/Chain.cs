using System.Collections;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Utils;

public class ProcessingChain<Q, T>
{
    private readonly List<Func<T, T>> functions;
    private readonly Func<Q, ICollection<T>> preprocessor;
    public ProcessingChain(Func<Q, ICollection<T>> preproc, ICollection<Func<T, T>> items)
    {
        this.functions = new List<Func<T, T>>(items);
        this.preprocessor = preproc;

        ChainExecutionPolicy = (ICollection<Func<T, T>> funcstions, T arg) =>
        {
            T result = arg;
            foreach (var func in funcstions)
                result = func(result);
            return result;
        };
    }
    public Func<ICollection<Func<T, T>>, T, T> ChainExecutionPolicy { get; set; }
    public void Execute(Q args)
    {
        if (args is null)
            throw new ArgumentException("Invalid input type");

        ICollection<T> preprocessed = preprocessor(args);
        
        Parallel.ForEach(preprocessed, (item) =>
        {
            item = ChainExecutionPolicy(functions, item);
        });
    }
}
