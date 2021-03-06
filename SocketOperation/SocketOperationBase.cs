﻿using System;
using uNet2.Channel;
using uNet2.Exceptions.SocketOperation;
using uNet2.Network;
using uNet2.Packet;
using uNet2.Packet.Events;

namespace uNet2.SocketOperation
{
    public abstract class SocketOperationBase : ISocketOperation
    {
        public PacketEvents.OnOperationPacketReceived OnPacketReceived { get; set; }

        public abstract int OperationId { get; }
        public Guid OperationGuid { get; set; }
        public Guid ConnectionGuid { get; set; }
        public IChannel HostChannel { get; set; }
        internal bool IsReady { get; set; }
        internal OperationSocket OperationSocket { get; set; }

        protected SocketOperationBase()
        {
            OperationGuid = SocketIdentity.GenerateGuid();
        }

        public virtual void Initialize()
        {
            IsReady = true;
        }

        public void SendPacket(IDataPacket data)
        {
          //  if (!IsReady)
          //      throw new SocketOperationNotInitializedException();
            if (HostChannel is TcpServerChannel)
                ((TcpServerChannel)HostChannel).OperationSend(data, ConnectionGuid, OperationGuid);
            else if (HostChannel is TcpClientChannel)
                ((TcpClientChannel) HostChannel).OperationSend(data, ConnectionGuid, OperationGuid);
        }   

        public void SendSequence(SequenceContext seqCtx)
        {
            seqCtx.InitPacket.IsOperation = true;
            seqCtx.InitPacket.OperationGuid = OperationGuid;
            if (HostChannel is TcpClientChannel)
                ((TcpClientChannel) HostChannel).SendSequence(seqCtx);
        }

        public abstract void PacketReceived(IDataPacket packet, IChannel sender);
        public abstract void PacketSent(IDataPacket packet, IChannel targetChannel);
        public abstract void SequenceFragmentReceived(SequenceFragmentInfo fragmentInfo);
        public abstract void Disconnected();

        public virtual void CloseOperation()
        {
            HostChannel.UnregisterOperation(this);
        }
    }
}
