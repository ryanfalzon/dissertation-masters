using Newtonsoft.Json;
using System.Text;
using UnifiedModel.SourceGenerator.CommonModels;
using UnifiedModel.SourceGenerator.Helpers;
using UnifiedModel.SourceGenerator.OnChainModels.Ethereum;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public class XOnChainEthereumGenerator : XOnChainGenerator
    {
        public XOnChainEthereumGenerator() : base()
        {

        }

        public override string AddClass(Modifiers modifier, string name, string parentHash)
        {
            Contract contract = new Contract(name, parentHash);
            contract.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(contract))));
            Memory.Add(contract);

            return contract.Hash;
        }

        public override string AddField(Modifiers modifier, Types type, string name, string parentHash)
        {
            Property property = new Property(modifier, type, name, parentHash);
            property.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(property))));
            Memory.Add(property);

            return property.Hash;
        }

        public override string AddMethod(Modifiers modifier, string returnType, string identifier, string parentHash)
        {
            Function function = new Function(identifier, modifier, parentHash);
            function.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(function))));
            Memory.Add(function);

            return function.Hash;
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