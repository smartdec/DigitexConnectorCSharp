using System.Collections.Generic;
using DigitexConnector.EngineAPI;
using DigitexConnector.Interfaces;

namespace DigitexConnector.Trading
{
    /// <summary>
    /// Main class for algo trading.
    /// </summary>
    public abstract class TradingAlgorithm : TradingAdapter
    {
        public TradingAlgorithm(string hostName, string token, bool secureConnection)
            : base(hostName, token, secureConnection)
        { }

        public TradingAlgorithm(IAggregator account)
            : base(account)
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
