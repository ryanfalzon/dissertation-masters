using Newtonsoft.Json;
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

        public override string AddClass(Modifiers modifier, string name, bool isModel, string parentHash)
        {
            if (isModel)
            {
                Struct @struct = new Struct(name, parentHash);
                @struct.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@struct))));
                Memory.Add(@struct);

                return @struct.Hash;
            }
            else
            {
                Contract contract = new Contract(name, parentHash);
                contract.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(contract))));
                Memory.Add(contract);

                return contract.Hash;
            }
        }

        public override string AddField(Modifiers modifier, Types type, string name, string parentHash)
        {
            Property property = new Property(modifier, type, name, parentHash);
            property.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(property))));
            Memory.Add(property);

            return property.Hash;
        }

        public override string AddMethod(Modifiers modifier, string returnType, string identifier, string parameters, string parameterAnchor, string parentHash)
        {
            Function function = new Function(identifier, modifier, parameters, parameterAnchor, parentHash);
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

        public override string CreatePropertyArgument(string hash)
        {
            Property property = (Property)Memory.Where(item => item.Hash.Equals(hash)).FirstOrDefault();
            return $"{property.Type} {property.Name}";
        }

        public override void Consume()
        {
            base.Consume();

            var functions = Models.Where(model => model.GetType() == typeof(Contract)).SelectMany(model => ((Contract)model).Functions);

            foreach(var function in functions)
            {
                if (!string.IsNullOrEmpty(function.ParameterAnchor))
                {
                    foreach(var expression in function.Expressions)
                    {
                        expression.Statement = expression.Statement.Contains($"{function.ParameterAnchor}.") ? expression.Statement.Replace($"{function.ParameterAnchor}.", "") : expression.Statement;
                    }
                }
            }
        }
    }
}