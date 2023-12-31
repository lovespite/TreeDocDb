using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TreeDocDb.Core.Models;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public struct AcPermission
{
    public bool Owner____Read { get; set; }
    public bool Owner___Write { get; set; }
    public bool Owner__Delete { get; set; }
    public bool OwnerReserved { get; set; }

    public bool Group____Read { get; set; }
    public bool Group___Write { get; set; }
    public bool Group__Delete { get; set; }
    public bool GroupReserved { get; set; }

    public bool Other____Read { get; set; }
    public bool Other___Write { get; set; }
    public bool Other__Delete { get; set; }
    public bool OtherReserved { get; set; }

    public bool Reserved0 { get; set; }
    public bool Reserved1 { get; set; }
    public bool Reserved2 { get; set; }
    public bool Reserved3 { get; set; }


    public override string ToString()
    {
        var sb = new StringBuilder();
        
        sb.Append(Owner____Read ? "r" : "-");
        sb.Append(Owner___Write ? "w" : "-");
        sb.Append(Owner__Delete ? "d" : "-");
        
        sb.Append(Group____Read ? "r" : "-");
        sb.Append(Group___Write ? "w" : "-");
        sb.Append(Group__Delete ? "d" : "-");
        
        sb.Append(Other____Read ? "r" : "-");
        sb.Append(Other___Write ? "w" : "-");
        sb.Append(Other__Delete ? "d" : "-");

        return sb.ToString();
    }

    public int ToInt32()
    {
        // [  0000     0000     0000     0000  ]
        //    ____     RWD_     RWD_     RWD_  
        //    Reserved Everyone Group    Owner         

        var ret = 0b0000_0000_0000_0000;

        if (Owner____Read) ret |= 0b0000_0000_0000_0001;
        if (Owner___Write) ret |= 0b0000_0000_0000_0010;
        if (Owner__Delete) ret |= 0b0000_0000_0000_0100;
        if (OwnerReserved) ret |= 0b0000_0000_0000_1000;

        if (Group____Read) ret |= 0b0000_0000_0001_0000;
        if (Group___Write) ret |= 0b0000_0000_0010_0000;
        if (Group__Delete) ret |= 0b0000_0000_0100_0000;
        if (GroupReserved) ret |= 0b0000_0000_1000_0000;

        if (Other____Read) ret |= 0b0000_0001_0000_0000;
        if (Other___Write) ret |= 0b0000_0010_0000_0000;
        if (Other__Delete) ret |= 0b0000_0100_0000_0000;
        if (OtherReserved) ret |= 0b0000_1000_0000_0000;

        if (Reserved0) ret |= 0b0001_0000_0000_0000;
        if (Reserved1) ret |= 0b0010_0000_0000_0000;
        if (Reserved2) ret |= 0b0100_0000_0000_0000;
        if (Reserved3) ret |= 0b1000_0000_0000_0000;

        return ret;
    }

    public static AcPermission FromInt32(int value)
    {
        var ret = new AcPermission
        {
            Owner____Read = (value & 0b0000_0000_0000_0001) != 0,
            Owner___Write = (value & 0b0000_0000_0000_0010) != 0,
            Owner__Delete = (value & 0b0000_0000_0000_0100) != 0,
            OwnerReserved = (value & 0b0000_0000_0000_1000) != 0,

            Group____Read = (value & 0b0000_0000_0001_0000) != 0,
            Group___Write = (value & 0b0000_0000_0010_0000) != 0,
            Group__Delete = (value & 0b0000_0000_0100_0000) != 0,
            GroupReserved = (value & 0b0000_0000_1000_0000) != 0,

            Other____Read = (value & 0b0000_0001_0000_0000) != 0,
            Other___Write = (value & 0b0000_0010_0000_0000) != 0,
            Other__Delete = (value & 0b0000_0100_0000_0000) != 0,
            OtherReserved = (value & 0b0000_1000_0000_0000) != 0,

            Reserved0 = (value & 0b0001_0000_0000_0000) != 0,
            Reserved1 = (value & 0b0010_0000_0000_0000) != 0,
            Reserved2 = (value & 0b0100_0000_0000_0000) != 0,
            Reserved3 = (value & 0b1000_0000_0000_0000) != 0
        };

        return ret;
    }

    public static implicit operator int(AcPermission permission) => permission.ToInt32();
    public static implicit operator AcPermission(int value) => FromInt32(value);
}