#define USE_UNMANAGED
// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
#if !USE_UNMANAGED
using System.Runtime.InteropServices;
#endif
using Stride.Core.IO;

namespace Stride.Core.Streaming;

/// <summary>
/// Content storage data chunk.
/// </summary>
[DebuggerDisplay("Content Chunk; IsLoaded: {IsLoaded}; Size: {Size}")]
public sealed class ContentChunk
{
    private IntPtr data;
#if !USE_UNMANAGED
        private GCHandle handle;
#endif

    /// <summary>
    /// Gets the parent storage container.
    /// </summary>
    public ContentStorage Storage { get; }

    /// <summary>
    /// Gets the chunk location in file (adress of the first byte).
    /// </summary>
    public int Location { get; }

    /// <summary>
    /// Gets the chunk size in file (in bytes).
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// Gets the last access time.
    /// </summary>
    public DateTime LastAccessTime { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this chunk is loaded.
    /// </summary>
    public bool IsLoaded => data != IntPtr.Zero;

    /// <summary>
    /// Gets a value indicating whether this chunk is not loaded.
    /// </summary>
    public bool IsMissing => !IsLoaded;

    /// <summary>
    /// Gets a value indicating whether this exists in file.
    /// </summary>
    public bool ExistsInFile => Size > 0;

    internal ContentChunk(ContentStorage storage, int location, int size)
    {
        Debug.Assert(size >= 0);
        Storage = storage;
        Location = location;
        Size = size;
    }

    /// <summary>
    /// Registers the usage operation of chunk data.
    /// </summary>
    public void RegisterUsage()
    {
        LastAccessTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Loads chunk data from the storage container.
    /// </summary>
    /// <param name="fileProvider">Database file provider.</param>
    public unsafe IntPtr GetData(DatabaseFileProvider fileProvider)
    {
        if (IsLoaded)
            return data;

        if (fileProvider == null)
            throw new ContentStreamingException("Missing file provider.", Storage);

        using (var stream = fileProvider.OpenStream(Storage.Url, VirtualFileMode.Open, VirtualFileAccess.Read, VirtualFileShare.Read, StreamFlags.Seekable))
        {
            stream.Position = Location;

#if USE_UNMANAGED
            var chunkBytes = Utilities.AllocateMemory(Size);

            var bufferCapacity = Math.Min(8192u, (uint)Size);
            var buffer = new byte[bufferCapacity];

            var count = (uint)Size;
            fixed (byte* bufferStart = buffer) // null if array is empty or null
            {
                var chunkBytesPtr = (byte*)chunkBytes;
                do
                {
                    var read = (uint)stream.Read(buffer, 0, (int)Math.Min(count, bufferCapacity));
                    if (read <= 0)
                        break;
                    Unsafe.CopyBlockUnaligned(chunkBytesPtr, bufferStart, read);
                    chunkBytesPtr += read;
                    count -= read;
                } while (count > 0);
            }
#else
                var bytes = new byte[Size];
                stream.Read(bytes, 0, Size);

                handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                var chunkBytes = handle.AddrOfPinnedObject();
#endif

            data = chunkBytes;
        }

        RegisterUsage();

        return data;
    }

    internal void Unload()
    {
        if (data != IntPtr.Zero)
        {
#if USE_UNMANAGED
            Utilities.FreeMemory(data);
#else
                handle.Free();
#endif
            data = IntPtr.Zero;
        }
    }
}
