using System.Net;
using System.Net.Sockets;

public abstract class Packet
{
    public Packet(byte[] data)
    {
        _data = data;
    }

    public byte[] GetData()
    {
        return _data;
    }

    public abstract void BuildPacket();

    protected byte[] _data;
}

public class PlayerPacket : Packet
{
    public PlayerPacket(byte[] playerData) : base(playerData) {}

    public override void BuildPacket()
    {
        _data = new byte[1024];
    }
}

public class ProjectilePacket : Packet
{
    public ProjectilePacket(byte[] projectileData) : base(projectileData) { }
    public override void BuildPacket()
    {
        _data = new byte[1024];
    }
}