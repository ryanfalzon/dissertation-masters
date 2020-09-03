using System.Collections.Generic;
using System.Linq;
using UnifiedModel.SourceGenerator.OffChainModels;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public class XOffChainGenerator : XChainGenerator
    {
        public List<Class> Classes { get; set; }

        public void AddClass(Modifiers modifier, string name)
        {
            throw new System.NotImplementedException();
        }

        public void AddField(Modifiers modifier, Types type, string name)
        {
            throw new System.NotImplementedException();
        }

        public string Consume()
        {
            return $"{string.Join("\n", Classes.Select(@class => @class.ToString()))}";
        }
    }
}