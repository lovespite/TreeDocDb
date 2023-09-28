using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TreeDocDb.Core.Models;

namespace TreeDocDb.Core.Converting;

public class DataPresent : IDisposable
{
    private readonly byte[] _bytes;

    public ReadOnlyMemory<byte> Memory => _bytes.ToArray().AsMemory();
    public DataType DataType { get; }
    
    public static DataPresent Empty => new(Array.Empty<byte>(), DataType.Unknown);

    private DataPresent(byte[] bytes, DataType type)
    {
        _bytes = bytes;
        DataType = type;
    }

    public bool IsEmpty => _bytes.Length == 0; 
    
    public void Dispose()
    {
        Array.Clear(_bytes, 0, _bytes.Length); // prevent memory leak
    }
    
    public object ToDataObj() => !IsEmpty
        ? DataType switch
        {
            DataType.Text => Encoding.UTF8.GetString(_bytes),
            DataType.Number => BitConverter.ToInt64(_bytes, 0),
            DataType.Boolean => BitConverter.ToBoolean(_bytes, 0),
            DataType.DateTime => DateTimeOffset.FromUnixTimeMilliseconds(BitConverter.ToInt64(_bytes, 0)),
            DataType.Bytes => _bytes.ToArray(),
            DataType.Unknown => throw new NotSupportedException($"{DataType} is not supported"),
            _ => throw new ArgumentOutOfRangeException()
        }
        : throw new InvalidOperationException("DataPresent is empty");

    public T ToDataObj<T>() => (T)ToDataObj();

    public void CopyTo(Stream stream) => stream.Write(_bytes, 0, _bytes.Length);

    public static DataType QueryType(Type t) => t switch
    {
        not null when t == typeof(string) => DataType.Text,
        not null when t == typeof(long) || t == typeof(int) => DataType.Number,
        not null when t == typeof(bool) => DataType.Boolean,
        not null when t == typeof(DateTimeOffset) => DataType.DateTime,
        not null when t == typeof(byte[]) => DataType.Bytes,
        _ => DataType.Unknown,
    };
    private static DataType QueryType(object dataObj) => QueryType(dataObj.GetType());
    private static DataType QueryType<T>() => QueryType(typeof(T));
    
    public static DataPresent FromText(string text) => new(Encoding.UTF8.GetBytes(text), DataType.Text);
    public static DataPresent FromNumber(long number) => new(BitConverter.GetBytes(number), DataType.Number);
    public static DataPresent FromBoolean(bool boolean) => new(BitConverter.GetBytes(boolean), DataType.Boolean);
    public static DataPresent FromDateTime(DateTimeOffset dateTime) =>
        new(BitConverter.GetBytes(dateTime.ToUnixTimeMilliseconds()), DataType.DateTime); 
    public static DataPresent FromBytes(byte[] bytes) => new(bytes, DataType.Bytes);

    public static DataPresent ConsumeStream(Stream stream, DataType type, int length = 0)
    {
        if (length <= 0 || length > stream.Length) length = (int)stream.Length;

        var buffer = new byte[length];
        var read = stream.Read(buffer, 0, length);
        Debug.Assert(read == length);
        return new DataPresent(buffer, type);
    }

    public static implicit operator byte[](DataPresent dataPresent) => dataPresent._bytes;
}