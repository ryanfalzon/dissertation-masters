using System;
using System.Collections.Generic;
using System.Linq;

namespace UnifiedModel.SourceGenerator.OffChainModels
{
    public class Class
    {
        public Modifiers Modifier { get; set; }

        public string Name { get; set; }

        public List<Field> Fields { get; set; }

        public Class()
        {
            Fields = new List<Field>();
        }

        public Class(Modifiers modifier, string name)
        {
            Modifier = modifier;
            Name = name;
        }

        public override string ToString()
        {
            return $"{Modifier} class {Name} {{ \n{string.Join("\n", Fields.Select(field => field.ToString()))} }}";
        }
    }
}