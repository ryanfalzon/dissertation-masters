using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public class XChainGeneratorFactory
    {
        public Dictionary<XChains, XChainGenerator> XChainGenerators { get; set; }

        public XChainGeneratorFactory()
        {
            XChainGenerators = new Dictionary<XChains, XChainGenerator>();
        }

        public XChainGenerator Get(string xChain)
        {
            bool parsed = Enum.TryParse(xChain, out XChains parsedXChain);

            if (!parsed)
            {
                throw new InvalidEnumArgumentException("Invalid XChain token!");
            }
            else if (!XChainGenerators.ContainsKey(parsedXChain))
            {
                XChainGenerator xChainGenerator;
                switch (parsedXChain)
                {
                    case XChains.XOffChain:
                        xChainGenerator = new XOffChainGenerator();
                        break;

                    default:
                        xChainGenerator = new XOffChainGenerator();
                        break;
                }

                XChainGenerators.Add(parsedXChain, xChainGenerator);
            }

            return XChainGenerators[parsedXChain];
        }

        public List<(string filename, string contents)> Consume()
        {
            List<(string filename, string contents)> files = new List<(string filename, string contents)>();

            foreach (var xChainGenerator in XChainGenerators)
            {
                xChainGenerator.Value.Consume();
                files.Add((xChainGenerator.Key.ToString(), xChainGenerator.Value.ToString()));
            }

            return files;
        }
    }
}