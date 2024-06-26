using System;

namespace Pools
{
    public interface IProjectilesCount
    {
        public static int Count { get; }
        public static event Action<int> CountChanged;

    }
}