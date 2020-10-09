using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnifiedModel.SourceGenerator.CommonModels;
using UnifiedModel.SourceGenerator.Helpers;
using UnifiedModel.SourceGenerator.OffChainModels.Desktop;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public class XOffChainDesktopGenerator : XChainGenerator
    {

        public XOffChainDesktopGenerator() : base()
        {
            FileExtension = ".cs";
        }

        public override string AddClass(ClassDetails classDetails, string parentHash)
        {
            Class @class = new Class(classDetails.Modifier, classDetails.Name, new ModelProperties(classDetails.IsModel, classDetails.ModelLocation), parentHash);
            @class.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@class))));
            Memory.Add(@class);

            return @class.Hash;
        }

        public override string AddField(FieldDetails fieldDetails, string parentHash)
        {
            Field field = new Field(fieldDetails.Modifier, fieldDetails.Type, fieldDetails.Name, parentHash);
            field.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(field))));
            Memory.Add(field);

            return field.Hash;
        }

        public override string AddMethod(MethodDetails methodDetails, string parentHash)
        {
            Method method = new Method(methodDetails.Modifier, methodDetails.ReturnType, methodDetails.Identifier, methodDetails.Parameters, methodDetails.ParameterAnchor, parentHash);
            method.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(method))));
            Memory.Add(method);

            return method.Hash;
        }

        public override string AddExpression(ExpressionDetails expressionDetails, string parentHash)
        {
            Expression expression = new Expression(expressionDetails.Statement, parentHash);
            expression.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(expression))));
            Memory.Add(expression);

            return expression.Hash;
        }

        public override string CreatePropertyArgument(string hash)
        {
            Field field = (Field)Memory.Where(item => item.Hash.Equals(hash)).FirstOrDefault();
            return $"{field.Type} {field.Name}";
        }

        public override void AddMethodParameters(MethodDetails methodDetails, string methodHash, Func<string, string, string, List<string>> generateParameters, string lastKnownBlockHash)
        {
            return;
        }
    }
}