using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace TreeDocDb.Core.DiskIO;

public class ZipTreeIndex : IIndexProvider
{
    private readonly Stream _stream;
    private readonly ZipArchive _index;

    public ZipTreeIndex(FileInfo indexFile)
    {
        _stream = indexFile.Open(FileMode.OpenOrCreate);
        _index = new ZipArchive(_stream, ZipArchiveMode.Update, false, System.Text.Encoding.UTF8);
    }

    public IEnumerable<KeyValuePair<string, Guid>> Query(IEnumerable<string> entryNames)
    {
        lock (_index)
        {
            foreach (var entryName in entryNames)
            {
                var entry = _index.GetEntry(entryName);
                if (entry is null)
                {
                    yield return new KeyValuePair<string, Guid>(entryName, Guid.Empty);
                    continue;
                }

                var buffer = new byte[16];
                using var stream = entry.Open();
                var read = stream.Read(buffer, 0, buffer.Length);
                if (read != buffer.Length) throw new Exception("Read error: length is not met");

                yield return new KeyValuePair<string, Guid>(entryName, new Guid(buffer));
            }
        }
    }

    public long Create(IEnumerable<KeyValuePair<string, Guid>> items)
    {
        lock (_index)
        {
            foreach (var item in items)
            {
                var entry = _index.CreateEntry(item.Key);

                using var stream = entry.Open();
                stream.Write(item.Value.ToByteArray(), 0, 16);
            }

            _stream.Flush();
            return _index.Entries.Count;
        }
    }

    public void Update(IEnumerable<KeyValuePair<string, Guid>> items)
    {
        lock (_index)
        {
            foreach (var item in items)
            {
                var entry = _index.GetEntry(item.Key) ?? throw new Exception($"Entry {item.Key} not found");

                using var stream = entry.Open();
                stream.Write(item.Value.ToByteArray(), 0, 16);
            }

            _stream.Flush();
        }
    }

    public long UpdateOrCreate(IEnumerable<KeyValuePair<string, Guid>> items)
    {
        lock (_index)
        {
            foreach (var item in items)
            {
                var entry = _index.GetEntry(item.Key) ?? _index.CreateEntry(item.Key);

                using var stream = entry.Open();
                stream.Write(item.Value.ToByteArray(), 0, 16);
            }

            _stream.Flush();
            return _index.Entries.Count;
        }
    }

    public long Count()
    {
        lock (_index)
        {
            return _index.Entries.Count;
        }
    }

    public void Dispose()
    {
        _index.Dispose();
    }
}