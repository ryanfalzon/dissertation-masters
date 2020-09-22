using System.Collections.Generic;
using UnifiedModel.SourceGenerator.SourceGenerators;

namespace UnifiedModel.SourceGenerator.CommonModels
{
    public class NodeDetails
    {
        public string Attribute { get; set; }

        public string AttributeArgument { get; set; }

        public Dictionary<XChains, string> ParentHashes { get; set; }
    }
}