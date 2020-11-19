using System;
using System.Collections.Generic;
using UnifiedModel.SourceGenerator.CommonModels;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public interface IXChainGenerator
    {
        string FileExtension { get; set; }

        string AddClass(ClassDetails classDetails, string parentHash);

        string AddField(FieldDetails fieldDetails, string parentHash);

        string AddConstructor(ConstructorDetails constructorDetails, string parentHash);

        string AddMethod(MethodDetails methodDetails, string parentHash);

        string AddMethodParameters(BaseMethodDetails baseMethodDetails, string methodHash, Func<string, string, string, List<string>> generateParameters, List<string> modelTypes);
        
        void AddMethodReturnTypes(MethodDetails methodDetails, string methodHash, Func<string, string, List<string>> generateReturnTypes, List<string> modelTypes);

        string AddExpression(ExpressionDetails expressionDetails, string parentHash, List<string> modelTypes = null);

        string CreatePropertyArgument(string hash);

        string CreateReturnType(string hash);

        void TypeMapper(ref Types type);

        void StringMapper(ref string text);

        void Consume();
    }
}