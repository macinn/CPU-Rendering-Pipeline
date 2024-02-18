using System.Collections;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Utils;

public class ProcessingChain<Q, T>
{
    private readonly List<Func<T, T?>> functions;
    private readonly Func<Q, IEnumerable<T>> preprocessor;
    public ProcessingChain(Func<Q, IEnumerable<T>> preproc, ICollection<Func<T, T?>> items)
    {
        this.functions = new List<Func<T, T?>>(items);
        this.preprocessor = preproc;

        ChainExecutionPolicy = (T arg) =>
        {
            T? result = arg;

            for(int i = 0; i < functions.Count && result != null; i++)
                result = functions[i](result);

            return result;
        };
    }
    public Func<T, T?> ChainExecutionPolicy { get; set; }
    public IEnumerable<T?> Execute(Q arg)
    {
        return preprocessor(arg)
            .Select(x => ChainExecutionPolicy(x));
    }
}
