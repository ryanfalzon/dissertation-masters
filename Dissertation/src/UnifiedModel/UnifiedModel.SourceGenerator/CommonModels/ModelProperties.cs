namespace UnifiedModel.SourceGenerator.CommonModels
{
    public class ModelProperties
    {
        public bool IsModel { get; set; }

        public string Location { get; set; }

        public ModelProperties(bool isModel, string location)
        {
            IsModel = isModel;
            Location = location;
        }
    }
}