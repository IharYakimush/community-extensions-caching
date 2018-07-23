using System;

namespace Comminity.Extensions.Caching
{
    [Flags]
    public enum AllowedCaches
    {
        None = 0,

        Memory = 1,

        Distributed = 2,

        All = 3
    }
}