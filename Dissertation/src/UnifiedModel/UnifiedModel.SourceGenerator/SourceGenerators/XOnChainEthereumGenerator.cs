using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            var type = fieldDetails.Type;
            TypeMapper(ref type);

            Property property = new Property(type, fieldDetails.Name, parentHash);
            property.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(property))));
            Memory.Add(property);

            return property.Hash;
        }

        public override string AddConstructor(ConstructorDetails constructorDetails, string parentHash)
        {
            var parameters = constructorDetails.Parameters;
            StringMapper(ref parameters);

            var parameterAnchor = constructorDetails.ParameterAnchor;
            StringMapper(ref parameterAnchor);

            Constructor constructor = new Constructor(constructorDetails.Modifier, parameters, parameterAnchor, parentHash);
            constructor.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(constructor))));
            Memory.Add(constructor);

            return constructor.Hash;
        }

        public override string AddMethod(MethodDetails methodDetails, string parentHash)
        {
            var parameters = methodDetails.Parameters;
            StringMapper(ref parameters);

            Function function = new Function(methodDetails.Identifier, methodDetails.Modifier, string.Empty, parameters, methodDetails.ReturnType, parentHash);
            function.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(function))));
            Memory.Add(function);

            return function.Hash;
        }

        public override string AddMethodParameters(BaseMethodDetails baseMethodDetails, string methodHash, Func<string, string, string, List<string>> generateParameters, List<string> modelTypes)
        {
            var mainMethodParameters = baseMethodDetails.Parameters.Split(',').Select(parameter => parameter.Trim());

            List<string> parameterList = new List<string>();
            foreach (var argument in baseMethodDetails.Arguments)
            {
                var mainMethodParameter = mainMethodParameters.Where(parameter => parameter.Contains(argument)).First().Split(' ');
                parameterList.AddRange(generateParameters(Constants.XOnEthereumChain, mainMethodParameter.First(), mainMethodParameter.Last()).Select(parameter => parameter.CheckRequiresMemorySyntax(modelTypes)));
            }

            var xCallParameters = string.Join(", ", parameterList.Select(parameter => parameter.ToString()));
            StringMapper(ref xCallParameters);

            var xCallArguments = string.Join(", ", parameterList.Select(parameter => parameter.Split(' ').Last()));

            Memory.ForEach(@object =>
            {
                if (@object.Hash.Equals(methodHash))
                {
                    ((Function)@object).Parameters = xCallParameters;
                }
            });

            return xCallArguments;
        }

        public override void AddMethodReturnTypes(MethodDetails methodDetails, string methodHash, Func<string, string, List<string>> generateReturnTypes, List<string> modelTypes)
        {
            var mainMethodReturnType = methodDetails.ReturnType;
            var returnTypesList = generateReturnTypes(Constants.XOnEthereumChain, mainMethodReturnType);

            var xCallReturnTypes = string.Join(", ", returnTypesList.Select(returnType => returnType.ToString().CheckRequiresMemorySyntax(modelTypes)));
            StringMapper(ref xCallReturnTypes);

            Memory.ForEach(@object =>
            {
                if (@object.Hash.Equals(methodHash))
                {
                    ((Function)@object).ReturnTypes = xCallReturnTypes;
                }
            });
        }

        public override string AddExpression(ExpressionDetails expressionDetails, string parentHash, List<string> modelTypes = null)
        {
            if(expressionDetails.SyntaxKind == SyntaxKind.LocalDeclarationStatement)
            {
                expressionDetails.Statement = expressionDetails.Statement.Split(" ", 2).Select((content, count) => count == 0 ? content.CheckRequiresMemorySyntax(modelTypes) : content).Aggregate((i, j) => i + " " + j);
            }

            var statement = expressionDetails.Statement;
            StringMapper(ref statement);

            Expression expression = new Expression(statement, parentHash);
            expression.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(expression))));
            Memory.Add(expression);

            return expression.Hash;
        }

        public override string CreatePropertyArgument(string hash)
        {
            Property property = (Property)Memory.Where(item => item.Hash.Equals(hash)).FirstOrDefault();

            var type = property.Type;
            TypeMapper(ref type);

            return $"{type.CheckRequiresMemorySyntax()} {property.Name}";
        }

        public override string CreateReturnType(string hash)
        {
            Property property = (Property)Memory.Where(item => item.Hash.Equals(hash)).FirstOrDefault();

            var type = property.Type;
            TypeMapper(ref type);

            return type.CheckRequiresMemorySyntax();
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
                    var originalParmeterNames = function.ParameterAnchor.Split(",").Select(parameter => parameter.Trim().Split(" ").Last()).ToList();
                    var regexPattern = new Regex(string.Join("|", originalParmeterNames.Select(parameterName => Regex.Escape($"{parameterName}.")).ToArray()));

                    foreach (var expression in function.Expressions)
                    {
                        expression.Statement = regexPattern.Replace(expression.Statement, string.Empty);
                    }
                }
            }
        }

        public override void TypeMapper(ref Types type)
        {
            type = type switch
            {
                Types.@short => Types.uint8,
                Types.@int => Types.uint128,
                Types.@long => Types.uint256,
                _ => type,
            };
        }

        public override void StringMapper(ref string text)
        {
            foreach (var keyword in Constants.EthereumChainMapperKeywords)
            {
                var pattern = @$"\b{keyword}\b";
                var replace = keyword switch
                {
                    "short" => "uint8",
                    "int" => "uint128",
                    "long" => "uint256",
                    _ => throw new InvalidCastException("Invalid keyword passed..."),
                };

                text = Regex.Replace(text, pattern, replace);
            }
        }
    }
}