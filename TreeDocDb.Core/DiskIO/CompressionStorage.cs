using System.IO.Compression;
using System.IO;
using System;
using System.Collections.Generic;

namespace TreeDocDb.Core.DiskIO;

public class CompressionStorage : IStorageProvider
{
    protected DirectoryInfo StorageDirectory { get; }
    protected DirectoryInfo DataStorageDirectory { get; } 
    protected IIndexProvider SectionIndex { get; private set; }


    private CompressionStorage(DirectoryInfo storageDirectory, Type indexType)
    {
        StorageDirectory = storageDirectory;
        DataStorageDirectory = new DirectoryInfo(Path.Combine(storageDirectory.FullName, "data"));
    }

    private void Initialize()
    {
        if (!StorageDirectory.Exists)
        {
            StorageDirectory.Create();
            DataStorageDirectory.Create();
            var index = new FileInfo(Path.Combine(StorageDirectory.FullName, "index"));
            SectionIndex = LoadIndex(typeof(ZipTreeIndex), index);
        }
        else
        {
        }
    }

    protected Guid CreateSection()
    {
        Guid id; // section id
        
        tryCreate:
        id = Guid.NewGuid();
        var dataFile = new FileInfo(Path.Combine(DataStorageDirectory.FullName, id.ToString()));

        // if file exists, try again. Though it is unlikely to happen.
        if (dataFile.Exists) goto tryCreate;

        dataFile.Create().Dispose(); // create empty file

        return id;
    }


    private static IIndexProvider LoadIndex(Type indexProviderInterface, FileInfo indexFile)
    {
        if (indexProviderInterface.GetInterface(nameof(IIndexProvider)) is null)
            throw new Exception("Index provider interface is not valid");

        var indexProvider = Activator.CreateInstance(indexProviderInterface, indexFile);

        if (indexProvider is null)
            throw new Exception("Index provider is not available");

        return (IIndexProvider)indexProvider;
    }
}