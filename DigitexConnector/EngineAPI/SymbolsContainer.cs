using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;

namespace DigitexConnector.EngineAPI
{
    public class SymbolsContainer
    {
        private static SymbolBox SymbolBox;

        static SymbolsContainer()
        {
            try
            {
                UpdateSymbols();
            }
            catch (FileNotFoundException)
            {
                SymbolBox = new SymbolBox();
            }
        }

        /// <summary>
        /// Update symbols.
        /// </summary>
        /// <param name="path">Path to json-file with list of symbols. Default Symbols.json.</param>
        public static void UpdateSymbols(string path = "Symbols.json")
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SymbolBox));
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                SymbolBox = (SymbolBox)jsonSerializer.ReadObject(fileStream);
            }
        }

        /// <summary>
        /// Returns all symbol names that are contained in the SymbolsContainer.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSymbolNames()
        {
            return (from _ in SymbolBox.Symbols select _.Name).ToList();
        }

        /// <summary>
        /// Returns instance of Symbol.
        /// </summary>
        /// <param name="name">Name of requested symbol.</param>
        /// <returns></returns>
        public static Symbol GetSymbol(string name)
        {
            if (SymbolBox == null)
            { return null; }
            return (from _ in SymbolBox.Symbols where _.Name == name select _).SingleOrDefault();
        }

        /// <summary>
        /// Returns instance of Symbol.
        /// </summary>
        /// <param name="marketId">MarketId of requested symbol.</param>
        /// <returns></returns>
        public static Symbol GetSymbol(uint marketId)
        {
            return (from _ in SymbolBox.Symbols where _.MarketId == marketId select _).SingleOrDefault();
        }

        public static bool Contains(Symbol symbol)
        {
            return SymbolBox.Symbols.Contains(symbol);
        }
    }
}
