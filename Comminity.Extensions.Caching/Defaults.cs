using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Comminity.Extensions.Caching
{
    public static class Defaults
    {
        private static readonly ConcurrentDictionary<Type,string> Hashes = new ConcurrentDictionary<Type, string>();

        private static string GetHash(Type type)
        {
            string text = type.FullName + type.AssemblyQualifiedName;

            using (var sha = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        public static MemoryCacheEntryOptions MemoryCacheEntryOptions { get; } = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(1)
        };

        public static DistributedCacheEntryOptions DistributedCacheEntryOptions { get; } =
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            };

        public static Func<Type, Type, string, string> FinalKeyFactory { get; } = (t1, t2, key) =>
        {
            return
                $"{Hashes.GetOrAdd(t1, GetHash)}_{Hashes.GetOrAdd(t2, GetHash)}_{key}";
        };

        public static Func<object, byte[]> Serializer { get; } = obj =>
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        };

        public static Func<byte[], object> Deserializer { get; } = data =>
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                return formatter.Deserialize(ms);                
            }
        };
    }
}