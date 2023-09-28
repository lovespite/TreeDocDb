using System;
using TreeDocDb.Core.Converting;

namespace TreeDocDb.Core.Models;

public class DataNode : TreeNodeBase
{  

    public DataNode(string name, DataType dataType) : base(name)
    {
        var ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); 
    }
}