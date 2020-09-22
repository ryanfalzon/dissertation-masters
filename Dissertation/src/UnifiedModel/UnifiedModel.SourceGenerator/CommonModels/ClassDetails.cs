using System.Collections.Generic;

namespace UnifiedModel.SourceGenerator.CommonModels
{
    public class ClassDetails : NodeDetails
    {
        public Modifiers Modifier { get; set; }

        public string Name { get; set; }

        public bool IsModel { get; set; }

        public string ModelLocation { get; set; }
    }
}