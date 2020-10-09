using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnifiedModel.SourceGenerator.CommonModels;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public static class XChainGeneratorFactory
    {
        public static Dictionary<XChains, IXChainGenerator> XChainGenerators = new Dictionary<XChains, IXChainGenerator>();

        public static List<IXChainGenerator> Get(string attribute, string argument = null)
        {
            if (!string.IsNullOrEmpty(attribute))
            {
                var xChainGenerators = new List<IXChainGenerator>();

                switch (attribute)
                {
                    case Constants.XAll:
                        {
                            foreach (XChains currentXChain in (XChains[])Enum.GetValues(typeof(XChains)))
                            {
                                switch (currentXChain)
                                {
                                    case XChains.Desktop:
                                        {
                                            xChainGenerators.AddRange(Get(Constants.XOn, Constants.XOnDesktop));
                                            break;
                                        }

                                    case XChains.Ethereum:
                                        {
                                            xChainGenerators.AddRange(Get(Constants.XOn, Constants.XOnEthereumChain));
                                            break;
                                        }
                                }
                            }
                            break;
                        }

                    case Constants.XOn:
                        {
                            if (!string.IsNullOrEmpty(argument))
                            {
                                argument = argument.Contains("\"") ? argument.Replace("\"", "") : argument;

                                switch (argument)
                                {
                                    case Constants.XOnDesktop:
                                        {
                                            EnsureExists<XOffChainDesktopGenerator>(XChains.Desktop);
                                            xChainGenerators.Add(XChainGenerators[XChains.Desktop]);
                                            break;
                                        }

                                    case Constants.XOnEthereumChain:
                                        {
                                            EnsureExists<XOnChainEthereumGenerator>(XChains.Ethereum);
                                            xChainGenerators.Add(XChainGenerators[XChains.Ethereum]);
                                            break;
                                        }

                                    default: throw new InvalidEnumArgumentException("Invalid XChain token!");
                                }
                            }
                            else
                            {
                                throw new ArgumentNullException("No XChain provided!");
                            }

                            break;
                        }

                    default: throw new InvalidEnumArgumentException("Invalid XChain token!");
                }

                return xChainGenerators;
            }
            else
            {
                throw new ArgumentNullException("No XChain provided!");
            }
        }

        public static void EnsureExists<T>(XChains xChain)
        {
            if (!XChainGenerators.ContainsKey(xChain))
            {
                XChainGenerators.Add(xChain, (IXChainGenerator)Activator.CreateInstance(typeof(T)));
            }
        }

        public static List<(string filename, string contents)> Consume()
        {
            var files = new List<(string filename, string contents)>();

            foreach (var xChainGenerator in XChainGenerators)
            {
                xChainGenerator.Value.Consume();
                files.Add(($"{xChainGenerator.Key}{xChainGenerator.Value.FileExtension}", xChainGenerator.Value.ToString()));
            }

            return files;
        }
    }
}