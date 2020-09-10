using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public class XChainGeneratorFactory
    {
        public Dictionary<XChains, IXChainGenerator> XChainGenerators { get; set; }

        public XChainGeneratorFactory()
        {
            XChainGenerators = new Dictionary<XChains, IXChainGenerator>();
        }

        public List<IXChainGenerator> Get(string xChain, string xOnChain = null)
        {
            if (Enum.TryParse(xChain, out XChains parsedXChain))
            {
                xOnChain = xOnChain != null && xOnChain.Contains("\"") ? xOnChain.Replace("\"", "") : xOnChain;
                Enum.TryParse(xOnChain, out XChains parsedXOnChain);

                if(parsedXChain == XChains.XAll)
                {
                    var xChainGenerators = new List<IXChainGenerator>();

                    foreach (XChains currentXChain in (XChains[])Enum.GetValues(typeof(XChains)))
                    {
                        if(currentXChain != XChains.XAll && currentXChain != XChains.XOnChain)
                        {
                            if (XChainGenerators.ContainsKey(currentXChain))
                            {
                                xChainGenerators.Add(XChainGenerators[currentXChain]);
                            }
                            else
                            {
                                xChainGenerators.AddRange(Get(currentXChain.ToString()));
                            }
                        }
                    }

                    return xChainGenerators;
                }
                else if (XChainGenerators.ContainsKey(parsedXChain) || ((parsedXChain == XChains.XOnChain) && XChainGenerators.ContainsKey(parsedXOnChain)))
                {
                    var xChainGenerators = new List<IXChainGenerator>();

                    if(parsedXChain == XChains.XOnChain)
                    {
                        xChainGenerators.Add(XChainGenerators[parsedXOnChain]);
                    }
                    else
                    {
                        xChainGenerators.Add(XChainGenerators[parsedXChain]);
                    }

                    return xChainGenerators;
                }
                else
                {
                    var xChainGenerators = new List<IXChainGenerator>();

                    switch (parsedXChain)
                    {
                        case XChains.XOnChain:
                            switch (parsedXOnChain)
                            {
                                case XChains.Ethereum:
                                    xChainGenerators.Add(new XOnChainEthereumGenerator());
                                    XChainGenerators.Add(parsedXOnChain, xChainGenerators.First());
                                    break;

                                default:
                                    xChainGenerators.Add(new XOnChainEthereumGenerator());
                                    XChainGenerators.Add(parsedXOnChain, xChainGenerators.First());
                                    break;
                            }
                            break;

                        case XChains.Ethereum:
                            xChainGenerators.Add(new XOnChainEthereumGenerator());
                            XChainGenerators.Add(parsedXChain, xChainGenerators.First());
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