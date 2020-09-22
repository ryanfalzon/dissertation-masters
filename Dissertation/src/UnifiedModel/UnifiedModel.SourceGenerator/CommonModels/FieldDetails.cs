namespace UnifiedModel.SourceGenerator.CommonModels
{
    public class FieldDetails : NodeDetails
    {
        public Modifiers Modifier { get; set; }

        public Types Type { get; set; }

        public string Name { get; set; }
    }
}