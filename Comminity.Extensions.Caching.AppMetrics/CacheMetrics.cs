using System;

namespace Comminity.Extensions.Caching.AppMetrics
{
    [Flags]
    public enum CacheMetrics
    {
        None = 0b0000_0000_0000_0000,

        TotalCount = 0b0000_0000_0000_0001,

        HitCount = 0b0000_0000_0000_0010,

        ErrorCount = 0b0000_0000_0000_0100,

        HitRatio = 0b0000_0000_0001_0011,

        ErrorRatio = 0b0000_0000_0010_0101,

        ReadTime = 0b0000_0001_0000_0000,

        WriteTime = 0b0000_0010_0000_0000,

        FactoryTime = 0b0000_0100_0000_0000,

        AllTime = 0b0000_0111_0000_0000
    }
}