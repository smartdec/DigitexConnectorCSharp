using System.Collections.Generic;
using DigitexConnector.Enums;
using DigitexConnector.Interfaces;

namespace DigitexConnector.Trading
{
    /// <summary>
    /// Main class for algo trading.
    /// </summary>
    public abstract class TradingAlgorithm : TradingAdapter
    {
        /// <summary>
        /// Use this constructor for set hostName and token directly.
        /// </summary>
        /// <param name="hostName">Address of exchange without prefix.</param>
        /// <param name="token">API-token.</param>
        /// <param name="secureConnection">Use (true) ssh or not (false).</param>
        public TradingAlgorithm(string hostName, string token, bool secureConnection)
            : base(hostName, token, secureConnection)
        { }

        public TradingAlgorithm(IAggregator account)
            : base(account)
        { }

        // <summary>
        /// Use this constructor for set one of two servers and set token directly.
        /// </summary>
        /// <param name="server"><see cref="Servers"/></param>
        /// <param name="token">API-token.</param>
        public TradingAlgorithm(Servers? server, string token)
            : base(server, token)
        { }

        /// <summary>
        /// Use this constructor for set token directly and if <see cref="Configuration.Server"/> is set.
        /// </summary>
        /// <param name="token">API-token.</param>
        public TradingAlgorithm(string token)
            : base(token)
        { }

        /// <summary>
        /// Use this constructor if <see cref="Configuration.Server"/> and <see cref="Configuration.Token"/> are set.
        /// </summary>
        public TradingAlgorithm()
            : base()
        { }

        /// <summary>
        /// Method that called before trading.
        /// </summary>
        public abstract void Prepare();

        /// <summary>
        /// Method for setting algorithm arguments.
        /// </summary>
        /// <param name="algoArguments"></param>
        public abstract void SetAlgoArguments(Dictionary<string, string> algoArguments);

        /// <summary>
        /// Method for extracting arguments from algorithm.
        /// </summary>
        /// <returns></returns>
        public abstract List<ModuleState> GetParams();

        /// <summary>
        /// Method that called when dispose.
        /// </summary>
        public abstract void OnDispose();

        public abstract void Stop();

        /// <summary>
        /// Dispose method.
        /// </summary>
        public new void Dispose()
        {
            OnDispose();
            (this as TradingAdapter).Dispose();
        }
    }
}
