using System;

namespace NetPacket
{
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

        public abstract void UnPack();

        protected byte[] _data;
        protected UInt32 _frame;
    }

    public class ProjectilePacket : Packet
    {
        public override Packet Build()
        {
            throw new System.NotImplementedException();
        }

        public override void UnPack()
        {
            throw new System.NotImplementedException();
        }
    }
}