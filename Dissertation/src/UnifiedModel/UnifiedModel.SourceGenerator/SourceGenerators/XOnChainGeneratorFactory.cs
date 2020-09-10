using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public class XOnChainGeneratorFactory
    {
        public Dictionary<XOnChains, XOnChainGenerator> XOnChainGenerators { get; set; }

        public XOnChainGeneratorFactory()
        {
            XOnChainGenerators = new Dictionary<XOnChains, XOnChainGenerator>();
        }

        public List<XOnChainGenerator> Get(string xOnChain)
        {
            if (Enum.TryParse(xOnChain, out XOnChains parsedXOnChain))
            {
                if (XOnChainGenerators.ContainsKey(parsedXOnChain))
                {
                    var xOnChainGenerators = new List<XOnChainGenerator>();

                    if (parsedXOnChain == XOnChains.XAll)
                    {
                        xOnChainGenerators.Add(XOnChainGenerators[XOnChains.Ethereum]);
                    }
                    else
                    {
                        xOnChainGenerators.Add(XOnChainGenerators[parsedXOnChain]);
                    }

                    return xOnChainGenerators;
                }
                else
                {
                    var xOnChainGenerators = new List<XOnChainGenerator>();

                    switch (parsedXOnChain)
                    {
                        case XOnChains.Ethereum:
                            xOnChainGenerators.Add(new XOnChainEthereumGenerator());
                            XOnChainGenerators.Add(parsedXOnChain, xOnChainGenerators.First());
                            break;

                        case XOnChains.XAll:
                            Get(XOnChains.Ethereum.ToString());
                            xOnChainGenerators = XOnChainGenerators.Values.ToList();
                            break;

                        default:
                            xOnChainGenerators.Add(new XOnChainEthereumGenerator());
                            XOnChainGenerators.Add(parsedXOnChain, xOnChainGenerators.First());
                            break;
                    }

                    return xOnChainGenerators;
                }
            }
            else
            {
                throw new InvalidEnumArgumentException("Invalid XOnChain token!");
            }
        }

        public List<(string filename, string contents)> Consume()
        {
            var files = new List<(string filename, string contents)>();

            foreach(var xOnChainGenerator in XOnChainGenerators)
            {
                xOnChainGenerator.Value.Consume();
                files.Add((xOnChainGenerator.Key.ToString(), xOnChainGenerator.Value.ToString()));
            }

            return files;
        }
    }
}