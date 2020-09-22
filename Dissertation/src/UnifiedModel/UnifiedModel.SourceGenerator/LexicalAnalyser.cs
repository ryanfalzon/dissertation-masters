using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnifiedModel.SourceGenerator.CommonModels;
using UnifiedModel.SourceGenerator.Helpers;
using UnifiedModel.SourceGenerator.SourceGenerators;

namespace UnifiedModel.SourceGenerator
{
    public class LexicalAnalyser
    {
        private readonly string FileContent;

        private readonly XChainGeneratorFactory Generator;

        private readonly Dictionary<string, List<ModelProperty>> Models;

        public LexicalAnalyser(string filePath)
        {
            FileContent = File.ReadAllText(filePath);
            Generator = new XChainGeneratorFactory();
            Models = new Dictionary<string, List<ModelProperty>>();
        }

        public void Process()
        {
            var root = CSharpSyntaxTree.ParseText(FileContent).GetRoot();
            ProcessChild(root, new NodeDetails());

            var files = Generator.Consume();
            foreach (var (filename, contents) in files)
            {
                File.WriteAllText($"C://temp/{filename}", contents);
            }
        }

        private void ProcessChild(SyntaxNode syntaxNode, NodeDetails previousNodeDetails)
        {
            switch (syntaxNode.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)syntaxNode;

                        if (classDeclarationSyntax.AttributeLists.Count > 0)
                        {
                            var classDetails = classDeclarationSyntax.GetClassDetails();
                            var classHashes = Generator.Get(classDetails.Attribute).Select(generator =>
                            {
                                var key = generator.GetEnumeratedType();
                                var value = generator.AddClass(classDetails, previousNodeDetails.ParentHashes == null ? string.Empty : previousNodeDetails.ParentHashes[key]);
                                return new KeyValuePair<XChains, string>(key, value);
                            }).ToDictionary(x => x.Key, x => x.Value);

                            if (classDetails.IsModel)
                            {
                                Models.Add(classDetails.Name, new List<ModelProperty>());
                            }

                            classDetails.ParentHashes = classHashes;
                            foreach (var member in classDeclarationSyntax.Members)
                            {
                                ProcessChild(member, classDetails);
                            }
                        }
                    }

                    break;

                case SyntaxKind.FieldDeclaration:
                    {
                        FieldDeclarationSyntax fieldDeclarationSyntax = (FieldDeclarationSyntax)syntaxNode;

                        if (previousNodeDetails != null && previousNodeDetails.GetType() == typeof(ClassDetails) && fieldDeclarationSyntax.AttributeLists.Count > 0)
                        {
                            var fieldDetails = fieldDeclarationSyntax.GetFieldDetails();
                            var fieldHashes = Generator.Get(fieldDetails.Attribute, fieldDetails.AttributeArgument).Select(generator =>
                            {
                                var key = generator.GetEnumeratedType();
                                var value = generator.AddField(fieldDetails, previousNodeDetails.ParentHashes == null ? string.Empty : previousNodeDetails.ParentHashes[key]);
                                return new KeyValuePair<XChains, string>(key, value);
                            }).ToDictionary(x => x.Key, x => x.Value);

                            var parsedPreviousNodeDetails = (ClassDetails)previousNodeDetails;
                            if (parsedPreviousNodeDetails.IsModel)
                            {
                                foreach (var fieldHash in fieldHashes)
                                {
                                    Models[parsedPreviousNodeDetails.Name].Add(new ModelProperty
                                    {
                                        Location = fieldHash.Key,
                                        Hash = fieldHash.Value
                                    });
                                }
                            }
                        }
                        else
                        {
                            throw new InvalidExpressionException("Field declared in incorrect place...");
                        }
                    }

                    break;

                case SyntaxKind.MethodDeclaration:
                    {
                        MethodDeclarationSyntax methodDeclarationSyntax = (MethodDeclarationSyntax)syntaxNode;

                        if (previousNodeDetails != null && previousNodeDetails.GetType() == typeof(ClassDetails))
                        {
                            var methodDetails = methodDeclarationSyntax.GetMethodDetails();
                            var statementQueue = new Queue<SyntaxNode>(methodDeclarationSyntax.Body.Statements);
                            string lastKnownBlockHash = string.Empty;

                            while (statementQueue.Count > 0)
                            {
                                var member = statementQueue.Dequeue();
                                if (statementQueue.Peek().Kind() == SyntaxKind.Block)
                                {
                                    if (Regex.IsMatch(member.ToString(), Constants.XOffChainRegex))
                                    {
                                        var methodHashes = Generator.Get(Constants.XOffChain).Select(generator =>
                                        {
                                            var key = generator.GetEnumeratedType();
                                            var value = generator.AddMethod(methodDetails, previousNodeDetails.ParentHashes == null ? string.Empty : previousNodeDetails.ParentHashes[key]); ;
                                            return new KeyValuePair<XChains, string>(key, value);
                                        }).ToDictionary(x => x.Key, x => x.Value);

                                        methodDetails.ParentHashes = methodHashes;
                                        methodDetails.Attribute = Constants.XOffChain;
                                        ProcessChild(statementQueue.Dequeue(), methodDetails);

                                        lastKnownBlockHash = methodHashes.First().Value;
                                    }
                                    else if (Regex.IsMatch(member.ToString(), Constants.XOnChainRegex) && !string.IsNullOrEmpty(lastKnownBlockHash))
                                    {
                                        var onChainArguments = member.ToString().Split('(', ')')[1].Split(',').ToList();
                                        var blockAttribute = onChainArguments.First();
                                        var argumentList = onChainArguments.Skip(1).ToList().Select(argument => argument.Trim());
                                        var mainMethodParameters = methodDetails.Parameters.Split(',').Select(parameter => parameter.Trim());

                                        List<string> parameterList = new List<string>();
                                        blockAttribute = blockAttribute.Contains("\"") ? blockAttribute.Replace("\"", "") : blockAttribute;

                                        foreach (var argument in argumentList)
                                        {
                                            var mainMethodParameter = mainMethodParameters.Where(parameter => parameter.Contains(argument)).First().Split(' ');
                                            parameterList.AddRange(GetOnChainModelParameters(blockAttribute, mainMethodParameter.First(), mainMethodParameter.Last()));
                                        }
                                        var xCallParameters = string.Join(", ", parameterList.Select(parameter => parameter.ToString()));
                                        var xCallArguments = string.Join(", ", parameterList.Select(parameter => parameter.Split(' ').Last()));

                                        var methodHashes = Generator.Get(Constants.XOnChain, blockAttribute).Select(generator =>
                                        {
                                            var key = generator.GetEnumeratedType();
                                            var value = generator.AddMethod(methodDetails.GenerateOnChainMethodDetails(xCallParameters), previousNodeDetails.ParentHashes == null ? string.Empty : previousNodeDetails.ParentHashes[key]); ;
                                            return new KeyValuePair<XChains, string>(key, value);
                                        }).ToDictionary(x => x.Key, x => x.Value);

                                        methodDetails.ParentHashes = methodHashes;
                                        methodDetails.Attribute = Constants.XOnChain;
                                        methodDetails.AttributeArgument = blockAttribute;
                                        ProcessChild(statementQueue.Dequeue(), methodDetails);

                                        Generator.Get(Constants.XOffChain).ForEach(generator =>
                                        {
                                            generator.AddExpression(new ExpressionDetails()
                                            {
                                                Statement = string.Format(Constants.XCallExpression, blockAttribute, methodDetails.Identifier, xCallArguments)
                                            }, lastKnownBlockHash);
                                        });
                                    }
                                }
                                else
                                {
                                    ProcessChild(member, methodDetails);
                                }
                            }
                        }
                        else
                        {
                            throw new InvalidExpressionException("Method declared in incorrect place...");
                        }
                    }

                    break;

                case SyntaxKind.ExpressionStatement:
                    {
                        ExpressionStatementSyntax expressionStatementSyntax = (ExpressionStatementSyntax)syntaxNode;

                        if (previousNodeDetails != null)
                        {
                            var expressionDetails = expressionStatementSyntax.GetExpressionDetails();
                            var statementHashes = Generator.Get(previousNodeDetails.Attribute, previousNodeDetails.AttributeArgument).Select(generator =>
                            {
                                var key = generator.GetEnumeratedType();
                                var value = generator.AddExpression(expressionDetails, previousNodeDetails.ParentHashes == null ? string.Empty : previousNodeDetails.ParentHashes[key]);
                                return new KeyValuePair<XChains, string>(key, value);
                            }).ToDictionary(x => x.Key, x => x.Value);
                        }
                        else
                        {
                            throw new InvalidExpressionException("Expression declared in incorrect place...");
                        }
                    }

                    break;

                case SyntaxKind.Block:
                    {
                        BlockSyntax blockSyntax = (BlockSyntax)syntaxNode;

                        if (previousNodeDetails != null)
                        {
                            foreach (var child in blockSyntax.ChildNodes())
                            {
                                ProcessChild(child, previousNodeDetails);
                            }
                        }
                        else
                        {
                            throw new InvalidExpressionException("Block declared in incorrect place...");
                        }

                        break;
                    }

                default:
                    {
                        foreach (var member in syntaxNode.ChildNodes())
                        {
                            ProcessChild(member, new NodeDetails());
                        }

                        break;
                    }
            }
        }

        private List<string> GetOnChainModelParameters(string targetChain, string modelType, string modelName)
        {
            var xChain = targetChain switch
            {
                Constants.XOnEthereumChain => XChains.EthereumChain,
                _ => throw new InvalidEnumArgumentException("Invalid XChain token!"),
            };

            if (modelType.IsPrimitiveType())
            {
                return new List<string>()
                {
                    $"{modelType} {modelName}"
                };
            }

            return Models[modelType]
                .Where(model => model.Location == xChain)
                .Select(model => Generator.Get(Constants.XOnChain, targetChain).First().CreatePropertyArgument(model.Hash))
                .ToList();
        }
    }
}