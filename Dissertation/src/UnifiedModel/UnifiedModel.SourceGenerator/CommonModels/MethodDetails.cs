namespace UnifiedModel.SourceGenerator.CommonModels
{
    public class MethodDetails : NodeDetails
    {
        public Modifiers Modifier { get; set; }

        public string ReturnType { get; set; }

        public string Identifier { get; set; }

        public string Parameters { get; set; }

        public string ParameterAnchor { get; set; }
    }
}