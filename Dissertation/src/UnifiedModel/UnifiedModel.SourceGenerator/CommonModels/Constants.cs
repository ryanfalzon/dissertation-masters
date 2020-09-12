namespace UnifiedModel.SourceGenerator.CommonModels
{
    public static class Constants
    {
        public const string XOffChain = "XOffChain";

        public const string XOnChain = "XOnChain";

        public const string XAll = "XAll";

        public const string XOnEthereumChain = "Ethereum";

        //public const string XOnChainRegex = "@XOnChain\\(\"[a-zA-Z]+\",\\)";
        public const string XOnChainRegex = "@XOnChain\\(\"[a-zA-Z]+\"(,{1} +[a-zA-Z]+)*\\)";

        public const string XCallExpression = "XCall(\"{0}\"\"{1}\"\"{2}\");";
    }
}