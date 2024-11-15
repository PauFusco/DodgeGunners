using System.Net;
using System.Net.Sockets;

public static class Packet(byte[] data)
{
    public byte[] GetData()
    {
        return _data;
    }

    public virtual void BuildPacket();

    private readonly byte[] _data = data;
}

public static class PlayerPacket(byte[] playerData) : Packet
{
    public override byte[] BuildPacket()
    {
        _data = new byte[1024];
    }
}

public static class ProjectilePacket(byte[] projectileData) : Packet
{
    public override byte[] BuildPacket()
    {
        _data = new byte[1024];
    }
}