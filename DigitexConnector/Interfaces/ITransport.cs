using System;

namespace DigitexConnector.Interfaces
{
    public interface ITransport : IDisposable
    {
        /// <summary>
        /// Event that happens when incoming message is received.
        /// </summary>
        event Action<byte[]> MessageReceived;

        /// <summary>
        /// Event that happens when transport is connected to data channel.
        /// </summary>
        event Action DataConnected;

        event Action DataReconnected;

        /// <summary>
        /// Event that happens when transport is disconnected from data channel.
        /// </summary>
        event Action DataDisconnected;

        /// <summary>
        /// Event that happens when transport is connected to control channel.
        /// </summary>
        event Action ControlConnected;

        event Action ControlReconnected;

        /// <summary>
        /// Event that happens when transport is desconnected from control channel.
        /// </summary>
        event Action ControlDisconnected;

        /// <summary>
        /// True if transport is connected to data channel. Else false.
        /// </summary>
        bool IsDataConnected { get; }

        /// <summary>
        /// True if transport is connected to control channel. Else false.
        /// </summary>
        bool IsControlConnected { get; }

        /// <summary>
        /// Send message to exchange.
        /// </summary>
        /// <param name="message">Byte message in proto format. <see cref="DigitexWire.Message"/></param>
        /// <returns></returns>
        bool Send(byte[] message);

        void Connect();
    }
}
