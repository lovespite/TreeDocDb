using TreeDocDb.Core.Models;

namespace TreeDocDb.IO.Models;

public interface INodeWriter
{
    int WriteNode(TreeNodeBase node);
}