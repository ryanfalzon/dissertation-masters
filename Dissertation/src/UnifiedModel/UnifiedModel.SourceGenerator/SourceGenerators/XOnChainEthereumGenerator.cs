using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnifiedModel.SourceGenerator.CommonModels;
using UnifiedModel.SourceGenerator.Helpers;
using UnifiedModel.SourceGenerator.OnChainModels.Ethereum;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public class XOnChainEthereumGenerator : XChainGenerator
    {
        public XOnChainEthereumGenerator() : base()
        {
            FileExtension = ".sol";
        }

        public override string AddClass(ClassDetails classDetails, string parentHash)
        {
            if (classDetails.IsModel)
            {
                Struct @struct = new Struct(classDetails.Name, new ModelProperties(classDetails.IsModel, classDetails.ModelLocation), parentHash);
                @struct.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@struct))));
                Memory.Add(@struct);

                return @struct.Hash;
            }
            else
            {
                Contract contract = new Contract(classDetails.Name, parentHash);
                contract.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(contract))));
                Memory.Add(contract);

                return contract.Hash;
            }
        }

        public override string AddField(FieldDetails fieldDetails, string parentHash)
        {
            Property property = new Property(fieldDetails.Type, fieldDetails.Name, parentHash);
            property.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(property))));
            Memory.Add(property);

            return property.Hash;
        }

        public override string AddMethod(MethodDetails methodDetails, string parentHash)
        {
            Function function = new Function(methodDetails.Identifier, methodDetails.Modifier, methodDetails.Parameters, methodDetails.ParameterAnchor, parentHash);
            function.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(function))));
            Memory.Add(function);

            return function.Hash;
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
            Property property = (Property)Memory.Where(item => item.Hash.Equals(hash)).FirstOrDefault();
            return $"{property.Type} {property.Name}";
        }

        public override void Consume()
        {
            Memory.Where(model => model.GetType() == typeof(Struct)).ToList().ForEach(@struct =>
            {
                var parsedStruct = (Struct)@struct;
                @struct.ParentHash = Memory.Where(model => model.GetType() == typeof(Contract) && ((Contract)model).Name.Equals(parsedStruct.ModelProperties.Location)).FirstOrDefault()?.Hash;
            });

            base.Consume();

            var functions = Models.Where(model => model.GetType() == typeof(Contract)).SelectMany(model => ((Contract)model).Functions);

            foreach (var function in functions)
            {
                if (!string.IsNullOrEmpty(function.ParameterAnchor))
                {
                    foreach (var expression in function.Expressions)
                    {
                        expression.Statement = expression.Statement.Contains($"{function.ParameterAnchor}.") ? expression.Statement.Replace($"{function.ParameterAnchor}.", "") : expression.Statement;
                    }
                }
            }
        }
    }
}