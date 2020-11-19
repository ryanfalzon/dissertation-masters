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
            var type = fieldDetails.Type;
            TypeMapper(ref type);

            Field field = new Field(fieldDetails.Modifier, type, fieldDetails.Name, parentHash);
            field.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(field))));
            Memory.Add(field);

            return field.Hash;
        }

        public override string AddConstructor(ConstructorDetails constructorDetails, string parentHash)
        {
            var parameters = constructorDetails.Parameters;
            StringMapper(ref parameters);

            var parameterAnchor = constructorDetails.ParameterAnchor;
            StringMapper(ref parameterAnchor);

            Constructor constructor = new Constructor(constructorDetails.Modifier, constructorDetails.Identifier, parameters, parameterAnchor, parentHash);
            constructor.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(constructor))));
            Memory.Add(constructor);

            return constructor.Hash;
        }

        public override string AddMethod(MethodDetails methodDetails, string parentHash)
        {
            var parameters = methodDetails.Parameters;
            StringMapper(ref parameters);

            var parameterAnchor = methodDetails.ParameterAnchor;
            StringMapper(ref parameterAnchor);

            Method method = new Method(methodDetails.Modifier, methodDetails.ReturnType, methodDetails.Identifier, parameters, parameterAnchor, parentHash);
            method.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(method))));
            Memory.Add(method);

            return method.Hash;
        }

        public override string AddMethodParameters(BaseMethodDetails baseMethodDetails, string methodHash, Func<string, string, string, List<string>> generateParameters, List<string> modelTypes)
        {
            return string.Empty;
        }

        public override void AddMethodReturnTypes(MethodDetails methodDetails, string methodHash, Func<string, string, List<string>> generateReturnTypes, List<string> modelTypes)
        {
            return;
        }

        public override string AddExpression(ExpressionDetails expressionDetails, string parentHash, List<string> modelTypes = null)
        {
            var statement = expressionDetails.Statement;
            StringMapper(ref statement);

            Expression expression = new Expression(statement, parentHash);
            expression.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(expression))));
            Memory.Add(expression);

            return expression.Hash;
        }

        public override string CreatePropertyArgument(string hash)
        {
            Field field = (Field)Memory.Where(item => item.Hash.Equals(hash)).FirstOrDefault();

            var type = field.Type;
            TypeMapper(ref type);

            return $"{type} {field.Name}";
        }

        public override string CreateReturnType(string hash)
        {
            Field field = (Field)Memory.Where(item => item.Hash.Equals(hash)).FirstOrDefault();

            var type = field.Type;
            TypeMapper(ref type);

            return type.ToString();
        }

        public override void TypeMapper(ref Types type)
        {
            type = type switch
            {
                Types.address => Types.@string,
                Types.bytes32 => Types.@string,
                Types.uint8 => Types.@short,
                Types.uint128 => Types.@int,
                Types.uint256 => Types.@long,
                _ => type
            };
        }

        public override void StringMapper(ref string text)
        {
            if(text == null)
            {
                return;
            }

            foreach(var keyword in Constants.DesktopMapperKeywords)
            {
                var convertTo = keyword switch
                {
                    "address" => "string",
                    "bytes32" => "string",
                    "uint8" => "short",
                    "uint128" => "int",
                    "uint256" => "long",
                    _ => throw new InvalidCastException("Invalid keyword passed...")
                };

                text.Replace(keyword, convertTo);
            }
        }
    }
}