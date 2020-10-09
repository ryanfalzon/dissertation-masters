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

        string AddMethod(MethodDetails methodDetails, string parentHash);

        void AddMethodParameters(MethodDetails methodDetails, string methodHash, Func<string, string, string, List<string>> generateParameters, string lastKnownBlockHash);

        string AddExpression(ExpressionDetails expressionDetails, string parentHash);

        string CreatePropertyArgument(string hash);

        void Consume();
    }
}