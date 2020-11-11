using System.Data;

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

        public const string XOnRegex = "@XOn\\(\"[a-zA-Z]+\"(,{1} +[a-zA-Z]+)*\\)";

        public const string XCallExpression = "XCall(\"{0}\", \"{1}\", {2})";

        public const string XModelRegex = "XModel\\(\"[a-zA-Z]+\"(,{1} +[a-zA-Z]+)*\\)";
    }
}