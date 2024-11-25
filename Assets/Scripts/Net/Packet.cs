using System;
using System.Net;
using System.Net.Sockets;

public abstract class Packet
{
    public Packet()
    {
        _data = new byte[1024];
    }

    public byte[] GetData()
    {
        return _data;
    }

    public abstract Packet Build();

    public abstract Packet UnPack();

    protected byte[] _data;
    protected UInt32 _frame;
}

public class PlayerPacket : Packet
{
    public override Packet Build()
    {
        throw new System.NotImplementedException();
    }

    public override Packet UnPack()
    {
        throw new System.NotImplementedException();
    }
}

public class ProjectilePacket : Packet
{
    public override Packet Build()
    {
        throw new System.NotImplementedException();
    }

    public override Packet UnPack()
    {
        throw new System.NotImplementedException();
    }
}