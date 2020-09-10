using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public class XChainGeneratorFactory
    {
        public Dictionary<XChains, IXChainGenerator> XChainGenerators { get; set; }

        public XOnChainGeneratorFactory XOnChainGeneratorFactory { get; set; }

        public XChainGeneratorFactory()
        {
            XChainGenerators = new Dictionary<XChains, IXChainGenerator>();
            XOnChainGeneratorFactory = new XOnChainGeneratorFactory();
        }

        public List<IXChainGenerator> Get(string xChain, string xOnChain = null)
        {
            if(Enum.TryParse(xChain, out XChains parsedXChain))
            {
                if (XChainGenerators.ContainsKey(parsedXChain))
                {
                    var xChainGenerators = new List<IXChainGenerator>();

                    switch (parsedXChain)
                    {
                        case XChains.XAll: xChainGenerators.Add(XChainGenerators[XChains.XOffChain]); break;
                        case XChains.XOnChain: xChainGenerators.AddRange(XOnChainGeneratorFactory.Get(xOnChain)); break;
                        case XChains.XOffChain: xChainGenerators.Add(XChainGenerators[parsedXChain]); break;
                        default: xChainGenerators.Add(XChainGenerators[parsedXChain]); break;
                    }

                    return xChainGenerators;
                }
                else
                {
                    var xChainGenerators = new List<IXChainGenerator>();

                    switch (parsedXChain)
                    {
                        case XChains.XAll:
                            Get(XChains.XOffChain.ToString());
                            xChainGenerators = XChainGenerators.Values.ToList();
                            xChainGenerators.AddRange(XOnChainGeneratorFactory.Get(XOnChains.XAll.ToString()));
                            break;

                        case XChains.XOnChain:
                            xChainGenerators.AddRange(XOnChainGeneratorFactory.Get(XOnChains.XAll.ToString()));
                            break;

                        case XChains.XOffChain:
                            xChainGenerators.Add(new XOffChainGenerator());
                            XChainGenerators.Add(parsedXChain, xChainGenerators.First());
                            break;

                        default:
                            xChainGenerators.Add(new XOffChainGenerator());
                            XChainGenerators.Add(parsedXChain, xChainGenerators.First());
                            break;
                    }

                    return xChainGenerators;
                }
            }
            else
            {
                throw new InvalidEnumArgumentException("Invalid XChain token!");
            }
        }

        public List<(string filename, string contents)> Consume()
        {
            var files = new List<(string filename, string contents)>();

            foreach (var xChainGenerator in XChainGenerators)
            {
                xChainGenerator.Value.Consume();
                files.Add((xChainGenerator.Key.ToString(), xChainGenerator.Value.ToString()));
            }

            return files;
        }
    }
}