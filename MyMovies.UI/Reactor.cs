using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MyMovies
{
    public static class Reactor
    {
        public static BackgroundWorker Run<T, TResult>(T param,
            Func<T, TResult> async, Action<TResult> onSuccess, Action<Exception> onError, Action<TResult, Exception> onBoth)
        {
            var w = new BackgroundWorker {WorkerSupportsCancellation = true};
            w.DoWork += (s, args) => {
                args.Result = async((T) args.Argument);
                if (w.CancellationPending)
                    throw new CancelledException();
            };
            w.RunWorkerCompleted += (s, args) =>{
                if (args.Error != null)
                {
                    if (onError != null)
                        onError(args.Error);
                }
                else if (onSuccess != null)
                {
                    onSuccess((TResult) args.Result);
                }
                if (onBoth != null)
                    onBoth(args.Error != null ? default(TResult) : (TResult) args.Result, args.Error);
            };
            w.RunWorkerAsync(param);
            return w;
        }
    }

    public class CancelledException : Exception
    {
    }
}
