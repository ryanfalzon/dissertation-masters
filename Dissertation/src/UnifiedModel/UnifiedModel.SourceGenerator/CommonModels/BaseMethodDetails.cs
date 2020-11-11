using System.Collections.Generic;

namespace UnifiedModel.SourceGenerator.CommonModels
{
    public class BaseMethodDetails : NodeDetails
    {
        public Modifiers Modifier { get; set; }

        public string Identifier { get; set; }

        public string Parameters { get; set; }

        public string ParameterAnchor { get; set; }

        public List<string> Arguments { get; set; }
    }
}