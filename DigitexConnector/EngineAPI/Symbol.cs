using System.Runtime.Serialization;
using System.Collections.Generic;
using System;

namespace DigitexConnector.EngineAPI
{
    [DataContract]
    public class SymbolBox
    {
        [DataMember]
        public readonly List<Symbol> Symbols = new List<Symbol>();

        public SymbolBox(List<Symbol> symbols)
        {
            Symbols = symbols;
        }

        public SymbolBox()
        { }
    }
}

namespace DigitexConnector.EngineAPI
{
    /// <summary>
    /// Symbol.
    /// </summary>
    [DataContract]
    public class Symbol : IEquatable<Symbol>
    {
        /// <summary>
        /// Id of symbol at exchange.
        /// </summary>
        [DataMember]
        public readonly int MarketId;

        /// <summary>
        /// Name of symbol at exchange.
        /// </summary>
        [DataMember]
        public readonly string Name;

        /// <summary>
        /// Symbol's price step.
        /// </summary>
        [DataMember]
        public readonly decimal PriceStep;

        /// <summary>
        /// Symbols quantity step.
        /// </summary>
        [DataMember]
        public readonly decimal QuantityStep;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="marketId">Id of symbol at exchange.</param>
        /// <param name="name">Name of symbol at exchange.</param>
        /// <param name="priceStep">Symbol's price step</param>
        /// <param name="quantityStep">Symbol's quantity step</param>
        public Symbol(int marketId, string name, decimal priceStep, decimal quantityStep)
        {
            MarketId = marketId;
            Name = name;
            PriceStep = priceStep;
            QuantityStep = quantityStep;
        }

        public Symbol()
        { }

        public override bool Equals(object other)
        {
            if (other is null)
            { return false; }
            if (ReferenceEquals(this, other))
            { return true; }
            if (GetType() != other.GetType())
            { return false; }
            return Equals(other as Symbol);
        }

        public bool Equals(Symbol other)
        {
            if (other is null)
            { return false; }
            if (ReferenceEquals(this, other))
            { return true; }
            if (string.Compare(Name, other.Name, StringComparison.CurrentCulture) == 0 &&
                MarketId == other.MarketId &&
                PriceStep == other.PriceStep &&
                QuantityStep == other.QuantityStep)
            { return true; }
            else
            { return false; }
        }

        public override int GetHashCode()
        {
            return MarketId.GetHashCode();
        }
    }
}
