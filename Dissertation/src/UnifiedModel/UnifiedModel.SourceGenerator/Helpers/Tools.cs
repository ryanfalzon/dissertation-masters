﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using UnifiedModel.SourceGenerator.SourceGenerators;

namespace UnifiedModel.SourceGenerator.Helpers
{
    public static class Tools
    {
        public static byte[] GetSha256Hash(byte[] data)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                return sha256Hash.ComputeHash(data);
            }
        }

        public static string ByteToHex(byte[] data)
        {
            return string.Concat(data.Select(x => x.ToString("x2")));
        }

        public static byte[] HexToByte(string data, int length = 0)
        {
            if (!(data == null || data.Equals("0")))
            {
                var bytes = new List<byte>();

                for (int i = 0; i < data.Length; i += 2)
                {
                    bytes.Add(Convert.ToByte(data.Substring(i, 2), 16));
                }

                return bytes.ToArray();
            }
            else
            {
                return new byte[length];
            }
        }

        public static XChains GetEnumeratedType(this IXChainGenerator generator)
        {
            if (generator.GetType() == typeof(XOffChainGenerator))
            {
                return XChains.XOffChain;
            }
            else if (generator.GetType() == typeof(XOnChainGenerator))
            {
                return XChains.XOnChain;
            }
            else
            {
                throw new InvalidExpressionException("Invalid annotated code");
            }
        }
    }
}