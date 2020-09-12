using Newtonsoft.Json;
using System.Text;
using UnifiedModel.SourceGenerator.CommonModels;
using UnifiedModel.SourceGenerator.Helpers;
using UnifiedModel.SourceGenerator.OffChainModels;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public class XOffChainGenerator : XChainGenerator
    {
        public XOffChainGenerator() : base()
        {
            
        }

        public override string AddClass(Modifiers modifier, string name, bool isModel, string parentHash)
        {
            Class @class = new Class(modifier, name, isModel, parentHash);
            @class.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@class))));
            Memory.Add(@class);

            return @class.Hash;
        }

        public override string AddField(Modifiers modifier, Types type, string name, string parentHash)
        {
            Field field = new Field(modifier, type, name, parentHash);
            field.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(field))));
            Memory.Add(field);

            return field.Hash;
        }

        public override string AddMethod(Modifiers modifier, string returnType, string identifier, string parentHash)
        {
            Method method = new Method(modifier, returnType, identifier, parentHash);
            method.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(method))));
            Memory.Add(method);

            return method.Hash;
        }

        public override string AddExpression(string statement, string parentHash)
        {
            Expression expression = new Expression(statement, parentHash);
            expression.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(expression))));
            Memory.Add(expression);

            return expression.Hash;
        }
    }
}