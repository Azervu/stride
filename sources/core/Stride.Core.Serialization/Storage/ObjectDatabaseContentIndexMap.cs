// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Stride.Core.Serialization.Contents;

namespace Stride.Core.Storage;

/// <summary>
/// Content Index Map implementation which regroups all the asset index maps of every loaded file backend and asset bundle backends.
/// </summary>
public class ObjectDatabaseContentIndexMap : IContentIndexMap
{
    private readonly Dictionary<string, ObjectId> values = [];

    public IContentIndexMap WriteableContentIndexMap { get; set; }

    /// <summary>
    /// Merges the values from the given asset index map.
    /// </summary>
    /// <param name="contentIndexMap">The asset index map to merge.</param>
    public void Merge(IContentIndexMap contentIndexMap)
    {
        Merge(contentIndexMap.GetMergedIdMap());
    }

    /// <summary>
    /// Merges the values from the given assets.
    /// </summary>
    /// <param name="assets">The assets to merge.</param>
    public void Merge(IEnumerable<KeyValuePair<string, ObjectId>> assets)
    {
        lock (values)
        {
            foreach (var item in assets)
            {
                values[item.Key] = item.Value;
            }
        }
    }

    /// <summary>
    /// Unmerges the values from the given assets.
    /// </summary>
    /// <param name="assets">The assets to merge.</param>
    public void Unmerge(IEnumerable<KeyValuePair<string, ObjectId>> assets)
    {
        lock (values)
        {
            foreach (var item in assets)
            {
                values.Remove(item.Key);
            }
        }
    }

    public bool TryGetValue(string url, out ObjectId objectId)
    {
        lock (values)
        {
            return values.TryGetValue(url, out objectId);
        }
    }

    public IEnumerable<KeyValuePair<string, ObjectId>> SearchValues(Func<KeyValuePair<string, ObjectId>, bool> predicate)
    {
        lock (values)
        {
            return values.Where(predicate).ToArray();
        }
    }

    public bool Contains(string url)
    {
        lock (values)
        {
            return values.ContainsKey(url);
        }
    }

    public ObjectId this[string url]
    {
        get
        {
            lock (values)
            {
                return values[url];
            }
        }
        set
        {
            lock (values)
            {
                if (WriteableContentIndexMap != null)
                    WriteableContentIndexMap[url] = value;
                values[url] = value;
            }
        }
    }

    public IEnumerable<KeyValuePair<string, ObjectId>> GetMergedIdMap()
    {
        lock (values)
        {
            return [.. values];
        }
    }

    public void Dispose()
    {
    }
}
