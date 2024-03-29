﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnifiedModel.SourceGenerator.CommonModels;
using UnifiedModel.SourceGenerator.SourceGenerators;

namespace UnifiedModel.SourceGenerator.Helpers
{
    public static class Tools
    {
        public static int IndentationLevel = 0;

        public static byte[] GetSha256Hash(byte[] data)
        {
            using SHA256 sha256Hash = SHA256.Create();
            return sha256Hash.ComputeHash(data);
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
            if (generator.GetType() == typeof(XOffChainDesktopGenerator))
            {
                return XChains.Desktop;
            }
            else if (generator.GetType() == typeof(XOnChainEthereumGenerator))
            {
                return XChains.Ethereum;
            }
            else
            {
                throw new InvalidExpressionException("Invalid annotated code");
            }
        }

        public static string Tabulate(this string content)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for(int i = 0; i < IndentationLevel; i++)
            {
                stringBuilder.Append("\t");
            }

            stringBuilder.Append(content);
            return stringBuilder.ToString();
        }

        public static bool IsPrimitiveType(this string content)
        {
            var primitiveTypes = Enum.GetNames(typeof(Types)).ToList();
            if (primitiveTypes.Contains(content.Split(' ').First()))
            {
                return true;
            }

            return false;
        }

        public static bool IsEnumerableType(this string content)
        {
            if(Regex.IsMatch(content, Constants.EnumerableRegex))
            {
                return true;
            }

            return false;
        }

        public static string CheckRequiresMemorySyntax(this Types type)
        {
            var required = type switch
            {
                Types.@string => true,
                _ => false
            };

            return required ? $"{type} memory" : type.ToString();
        }

        //public static string CheckRequiresMemorySyntax(this string content, List<string> modelTypes)
        //{
        //    if(content.Contains(" "))
        //    {
        //        var regexPattern = new Regex(string.Join("|", modelTypes);
        //        regexPattern.Replace(content, delegate (Match m) {
        //            return m.Value + " memory";
        //        });
        //    }
        //    else
        //    {
        //        var parsed = Enum.TryParse(content, out Types parsedType);
        //        return !parsed && (modelTypes.Contains(content) || content.Contains("[]")) ? $"{content} memory" : parsedType.CheckRequiresMemorySyntax();
        //    }
        //}

        public static string CheckRequiresMemorySyntax(this string content, List<string> modelTypes)
        {
            var variableDecleration = content.Split(" ", 2);
            var variableType = variableDecleration.First();
            var variableName = variableDecleration.Length == 1 ? string.Empty : $" {variableDecleration.Last()}";

            var parsed = Enum.TryParse(variableDecleration.First(), out Types parsedType);
            return !parsed && (modelTypes.Contains(variableType) || variableType.Contains("[]")) ? $"{variableType} memory{variableName}" : (parsedType.CheckRequiresMemorySyntax() + variableName);
        }
    }
}