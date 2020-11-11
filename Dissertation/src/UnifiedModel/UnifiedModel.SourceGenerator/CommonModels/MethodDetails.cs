using System.Collections.Generic;

namespace UnifiedModel.SourceGenerator.CommonModels
{
    public class MethodDetails : BaseMethodDetails
    {
        public string ReturnType { get; set; }

        public bool IsAsynchronous { get; set; }
    }
}