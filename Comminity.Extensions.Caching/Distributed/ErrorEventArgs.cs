using System;
using System.Collections.Generic;
using System.Text;

namespace Comminity.Extensions.Caching.Distributed
{
    public class ErrorEventArgs : EventArgs
    {
        public Exception Exception { get; }

        public ErrorEventArgs(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }
    }
}
