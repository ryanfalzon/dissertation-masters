namespace UnifiedModel.SourceGenerator.OffChainModels
{
    public class Field
    {
        public Modifiers Modifier { get; set; }

        public Types Type { get; set; }

        public string Name { get; set; }

        public Field(Modifiers modifier, Types type, string name)
        {
            Modifier = modifier;
            Type = type;
            Name = name;
        }

        public override string ToString()
        {
            return $"{Modifier} {Type} {Name} {{ get; set; }}\n";
        }
    }
}