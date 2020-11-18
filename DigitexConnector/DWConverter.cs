using System;
using DigitexWire;
using Google.Protobuf;

namespace DigitexConnector.Extentions
{
    /// <summary>
    /// Class with methods of conversion.
    /// </summary>
    public static class DWConverter
    {
        /// <summary>
        /// Create message for send to exchange.
        /// </summary>
        /// <param name="oneOfContent"><see cref="Message"/></param>
        /// <param name="marketId">Id of current market.</param>
        /// <param name="clientId">Id of message to send.</param>
        /// <param name="traderId">Identificator of trader (ain't necessary in stock version of connector - set to 0)</param>
        /// <returns>Message to send. <see cref="Message"/></returns>
        static public Message BuildMessage(IMessage oneOfContent, uint marketId, Guid clientId, uint traderId)
        {
            Message masterMessage = new Message();
            //switch (slaveMessage.GetType().ToString())
            if (oneOfContent.GetType() == typeof(PlaceOrderMessage))
            { masterMessage.PlaceOrderMsg = ((PlaceOrderMessage)oneOfContent).Clone(); }
            else if (oneOfContent.GetType() == typeof(CancelOrderMessage))
            { masterMessage.CancelOrderMsg = ((CancelOrderMessage)oneOfContent).Clone(); }
            else if (oneOfContent.GetType() == typeof(CancelAllOrdersMessage))
            { masterMessage.CancelAllOrdersMsg = ((CancelAllOrdersMessage)oneOfContent).Clone(); }
            else if (oneOfContent.GetType() == typeof(OrderStatusMessage))
            { masterMessage.OrderStatusMsg = ((OrderStatusMessage)oneOfContent).Clone(); }
            else if (oneOfContent.GetType() == typeof(OrderFilledMessage))
            { masterMessage.OrderFilledMsg = ((OrderFilledMessage)oneOfContent).Clone(); }
            else if (oneOfContent.GetType() == typeof(ChangeLeverageAllMessage))
            { masterMessage.ChangeLeverageAllMsg = ((ChangeLeverageAllMessage)oneOfContent).Clone(); }
            else if (oneOfContent.GetType() == typeof(GetTraderStatusMessage))
            { masterMessage.GetTraderStatusMsg = ((GetTraderStatusMessage)oneOfContent).Clone(); }
            else if (oneOfContent.GetType() == typeof(OrderBookRequestMessage))
            { masterMessage.OrderBookRequestMsg = ((OrderBookRequestMessage)oneOfContent).Clone(); }
            else if (oneOfContent.GetType() == typeof(GetMarketStateMessage))
            { masterMessage.GetMarketStateMsg = ((GetMarketStateMessage)oneOfContent).Clone(); }
            else
            { throw new FormatException($"Message Type \"{oneOfContent.GetType()}\" does not exist"); }

            masterMessage.Timestamp = DateTime.Now.Ticks;
            masterMessage.Serial = 0;
            masterMessage.MarketId = marketId;
            masterMessage.TraderId = traderId;
            masterMessage.ClientId = ByteString.CopyFrom(clientId.ToByteArray());
            return masterMessage;
        }

        /// <summary>
        /// Convert System.Decimal type to DigitexWire.Decimal.
        /// </summary>
        /// <param name="decValue">Original value.</param>
        /// <returns>Value converted to DigitexWire.Decimal. <see cref="DigitexWire.Decimal"/></returns>
        static public DigitexWire.Decimal ToProtoDecimal(decimal decValue)
        {
            uint scale = 0;
            decimal reminder = decValue % 1;
            long value64 = (long)(decValue - reminder);
            while (reminder != 0)
            {
                reminder *= 10;
                value64 *= 10;
                scale += 1;
                value64 += (long)(reminder - (reminder % 1));
                reminder %= 1;
            }
            DigitexWire.Decimal value = new DigitexWire.Decimal();
            value.Value64 = value64;
            value.Scale = scale;
            return value;
        }

        /// <summary>
        /// Convert DigitexWire.Decimal to System.Decimal.
        /// </summary>
        /// <param name="value">Original DigitexWire.Decimal value. <see cref="DigitexWire.Decimal"/></param>
        /// <returns>Value converted to System.Decimal.</returns>
        static public decimal FromProtoDecimal(DigitexWire.Decimal value)
        {
            return value == null ? 0 : (decimal)(value.Value64 * Math.Pow(10, -value.Scale));
        }

        /// <summary>
        /// Convert Google.Protobuf.ByteString guid, using in DigitexWire, to System.Guid.
        /// </summary>
        /// <param name="guid">Original Google.Protobuf.ByteString value.</param>
        /// <returns>Value converted to System.Guid.</returns>
        static public Guid FromProtoUuid(ByteString uuid)
        {
            if (uuid.IsEmpty)
            {
                return Guid.Empty;
            }
            byte[] bytes = uuid.ToByteArray();
            byte[] newBytes = new byte[]
            {
                    bytes[3],
                    bytes[2],
                    bytes[1],
                    bytes[0],
                    bytes[5],
                    bytes[4],
                    bytes[7],
                    bytes[6],
                    bytes[8],
                    bytes[9],
                    bytes[10],
                    bytes[11],
                    bytes[12],
                    bytes[13],
                    bytes[14],
                    bytes[15],
            };
            return new Guid(newBytes);
        }

        static public Guid FromOpenAPIUUID(string uuid)
        {
            char[] charArray = uuid.ToCharArray();
            char[] resultArray = new char[]
            {
                charArray[6],
                charArray[7],
                charArray[4],
                charArray[5],
                charArray[2],
                charArray[3],
                charArray[0],
                charArray[1],
                charArray[8],
                charArray[11],
                charArray[12],
                charArray[9],
                charArray[10],
                charArray[13],
                charArray[16],
                charArray[17],
                charArray[14],
                charArray[15],
                charArray[18],
                charArray[19],
                charArray[20],
                charArray[21],
                charArray[22],
                charArray[23],
                charArray[24],
                charArray[25],
                charArray[26],
                charArray[27],
                charArray[28],
                charArray[29],
                charArray[30],
                charArray[31],
                charArray[32],
                charArray[33],
                charArray[34],
                charArray[35]
            };
            string guid = new string(charArray);
            return new Guid(guid);
        }

        static public Guid GuidToUuid(Guid guid)
        {
            byte[] bytes = guid.ToByteArray();
            byte[] newBytes = new byte[]
            {
                bytes[3],
                bytes[2],
                bytes[1],
                bytes[0],
                bytes[5],
                bytes[4],
                bytes[7],
                bytes[6],
                bytes[8],
                bytes[9],
                bytes[10],
                bytes[11],
                bytes[12],
                bytes[13],
                bytes[14],
                bytes[15],
            };
            return new Guid(newBytes);
        }

        /// <summary>
        /// Convert long Timestamp value to System.DateTime./>
        /// </summary>
        /// <param name="time">Original long Timestamp value.</param>
        /// <returns>Value converted to System.DateTime.</returns>
        static public DateTime FromLongDateTime(long time)
        {
            return (new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(time / 1000)).ToLocalTime();
        }
    }
}
