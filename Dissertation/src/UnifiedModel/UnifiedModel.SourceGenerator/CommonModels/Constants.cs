using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace UnifiedModel.SourceGenerator.CommonModels
{
    public static class Constants
    {
        public const string NotParameter = "NotParameter";

        public const string XOn = "XOn";

        public const string XAll = "XAll";

        public const string XModel = "XModel";

        public const string XOnDesktop = "Desktop";

        public const string XOnEthereumChain = "Ethereum";

        public const string XOnRegex = "(return )?@XOn\\(\"[a-zA-Z]+\"(,{1} +[a-zA-Z]+)*\\)";

        public const string XCallExpression = "XCall(\"{0}\", \"{1}\", {2})";

        public const string XModelRegex = "XModel\\(\"[a-zA-Z]+\"(,{1} +[a-zA-Z]+)*\\)";

        public const string MappingRegex = "mapping\\([a-zA-Z]+ *=> *[a-zA-Z]+\\) +((public)|(private)) +[a-zA-Z]+;";

        public const string EnumerableRegex = "IEnumerable<[a-zA-Z0-9]+>";

        public const string DesktopUsingStatements = "using UnifiedModel.Connectors;\n" +
            "using UnifiedModel.Connectors.Ethereum;\n";

        public static ReadOnlyCollection<string> EthereumChainMapperKeywords = new ReadOnlyCollection<string>(new List<string>()
        {
            "short", "int", "long"
        });

        public static ReadOnlyCollection<string> DesktopMapperKeywords = new ReadOnlyCollection<string>(new List<string>()
        {
            "address", "bytes32", "uint8", "uint128", "uint256"
        });
    }
}