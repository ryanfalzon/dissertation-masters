﻿using Microsoft.CodeAnalysis;
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
using Generator = UnifiedModel.SourceGenerator.SourceGenerators.XChainGeneratorFactory;

namespace UnifiedModel.SourceGenerator
{
    public class LexicalAnalyser
    {
        private readonly string FileContent;

        private readonly Dictionary<string, List<ModelProperty>> Models;

        public LexicalAnalyser(string filePath)
        {
            FileContent = File.ReadAllText(filePath);
            Models = new Dictionary<string, List<ModelProperty>>();
        }

        public void Process(string outputPath)
        {
            var root = CSharpSyntaxTree.ParseText(FileContent).GetRoot();
            ProcessChild(root, new NodeDetails());

            outputPath = outputPath.EndsWith('/') ? outputPath : $"{outputPath}/";
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var files = Generator.Consume();
            foreach (var (filename, contents) in files)
            {
                File.WriteAllText($"{outputPath}{filename}", contents);
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
                            var classHashes = Generator.Get(classDetails.Attribute, classDetails.AttributeArgument).Select(generator =>
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

                            var singleLineComments = classDeclarationSyntax.DescendantTrivia().Where(descendent => descendent.IsKind(SyntaxKind.SingleLineCommentTrivia));
                            foreach (var singleLineComment in singleLineComments)
                            {
                                var comment = singleLineComment.ToFullString().Split("//").Last().Trim();
                                if (Regex.IsMatch(syntaxNode.ToString(), Constants.XOnRegex) && classHashes.ContainsKey(XChains.Ethereum))
                                {
                                    var expressionDetails = singleLineComment.GetExpressionDetails();
                                    Generator.Get(Constants.XOn, Constants.XOnEthereumChain).ForEach(generator =>
                                    {
                                        generator.AddExpression(expressionDetails, classHashes.Where(classHash => classHash.Key == XChains.Ethereum).FirstOrDefault().Value);
                                    });
                                }
                            }

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

                        if (previousNodeDetails != null && previousNodeDetails.GetType() == typeof(ClassDetails))
                        {
                            var fieldDetails = fieldDeclarationSyntax.GetFieldDetails();
                            if (fieldDetails.Attribute.Equals(string.Empty))
                            {
                                fieldDetails.Attribute = previousNodeDetails.Attribute;
                                fieldDetails.AttributeArgument = previousNodeDetails.AttributeArgument;
                            }

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
                                        Hash = fieldHash.Value,
                                        IsParameter = fieldDetails.IsParameter
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

                case SyntaxKind.ConstructorDeclaration:
                    {
                        ConstructorDeclarationSyntax constructorDeclarationSyntax = (ConstructorDeclarationSyntax)syntaxNode;

                        if (previousNodeDetails != null && previousNodeDetails.GetType() == typeof(ClassDetails))
                        {
                            var constructorDetails = constructorDeclarationSyntax.GetConstructorDetails();
                            if (constructorDetails.Attribute.Equals(string.Empty))
                            {
                                constructorDetails.Attribute = previousNodeDetails.Attribute;
                                constructorDetails.AttributeArgument = previousNodeDetails.AttributeArgument;
                            }

                            var constructorHashes = Generator.Get(constructorDetails.Attribute, constructorDetails.AttributeArgument).Select(generator =>
                            {
                                var key = generator.GetEnumeratedType();
                                var value = generator.AddConstructor(constructorDetails, previousNodeDetails.ParentHashes == null ? string.Empty : previousNodeDetails.ParentHashes[key]);
                                return new KeyValuePair<XChains, string>(key, value);
                            }).ToDictionary(x => x.Key, x => x.Value);

                            var statementQueue = new Queue<SyntaxNode>(constructorDeclarationSyntax.Body.Statements);
                            string lastKnownBlockHash = string.Empty;

                            while (statementQueue.Count > 0)
                            {
                                var member = statementQueue.Dequeue();
                                if (Regex.IsMatch(member.ToString(), Constants.XOnRegex) && statementQueue.Peek().Kind() == SyntaxKind.Block)
                                {
                                    var onChainArguments = member.ToString().Split('(', ')')[1].Split(',').ToList();
                                    var blockAttribute = onChainArguments.First().Contains("\"") ? onChainArguments.First().Replace("\"", "") : onChainArguments.First();
                                    var argumentList = onChainArguments.Skip(1).ToList().Select(argument => argument.Trim()).ToList();

                                    constructorDetails.Arguments = argumentList;

                                    constructorDetails.ParentHashes = constructorHashes.Where(methodHash => methodHash.Key.ToString() == blockAttribute).ToDictionary(methodHash => methodHash.Key, methodHash => methodHash.Value);
                                    constructorDetails.Attribute = Constants.XOn;
                                    constructorDetails.AttributeArgument = blockAttribute;

                                    Generator.Get(Constants.XOn, blockAttribute).ForEach(generator => generator.AddMethodParameters(constructorDetails, constructorHashes.Where(methodHash => methodHash.Key.ToString() == blockAttribute).First().Value, GetModelParameters, Models.Select(model => model.Key).ToList()));

                                    ProcessChild(statementQueue.Dequeue(), constructorDetails);

                                    lastKnownBlockHash = blockAttribute.Equals(Constants.XOnDesktop) ? constructorHashes.First().Value : string.Empty;
                                }
                                else
                                {
                                    constructorDetails.ParentHashes = constructorHashes;
                                    constructorDetails.Attribute = constructorDetails.Attribute;
                                    constructorDetails.AttributeArgument = constructorDetails.AttributeArgument;
                                    ProcessChild(member, constructorDetails);
                                }
                            }
                        }
                        else
                        {
                            throw new InvalidExpressionException("Constructor declared in incorrect place...");
                        }
                    }

                    break;

                case SyntaxKind.MethodDeclaration:
                    {
                        MethodDeclarationSyntax methodDeclarationSyntax = (MethodDeclarationSyntax)syntaxNode;

                        if (previousNodeDetails != null && previousNodeDetails.GetType() == typeof(ClassDetails))
                        {
                            var methodDetails = methodDeclarationSyntax.GetMethodDetails();
                            if (methodDetails.Attribute.Equals(string.Empty))
                            {
                                methodDetails.Attribute = previousNodeDetails.Attribute;
                                methodDetails.AttributeArgument = previousNodeDetails.AttributeArgument;
                            }

                            var methodHashes = Generator.Get(string.IsNullOrEmpty(methodDetails.Attribute) ? previousNodeDetails.Attribute : methodDetails.Attribute, string.IsNullOrEmpty(methodDetails.Attribute) ? previousNodeDetails.AttributeArgument : methodDetails.AttributeArgument).Select(generator =>
                            {
                                var key = generator.GetEnumeratedType();
                                var value = generator.AddMethod(methodDetails, previousNodeDetails.ParentHashes == null ? string.Empty : previousNodeDetails.ParentHashes[key]);
                                return new KeyValuePair<XChains, string>(key, value);
                            }).ToDictionary(x => x.Key, x => x.Value);

                            var statementQueue = new Queue<SyntaxNode>(methodDeclarationSyntax.Body.Statements);
                            string lastKnownBlockHash = string.Empty;

                            while (statementQueue.Count > 0)
                            {
                                var member = statementQueue.Dequeue();
                                if (Regex.IsMatch(member.ToString(), Constants.XOnRegex) && statementQueue.Peek().Kind() == SyntaxKind.Block)
                                {
                                    var onChainArguments = member.ToString().Split('(', ')')[1].Split(',').ToList();
                                    var blockAttribute = onChainArguments.First().Contains("\"") ? onChainArguments.First().Replace("\"", "") : onChainArguments.First();
                                    var argumentList = onChainArguments.Skip(1).ToList().Select(argument => argument.Trim()).ToList();
                                    var isSynchronous = member.ToString().StartsWith(Constants.SynchronousEscapeCharacter);

                                    methodDetails.Arguments = argumentList;

                                    methodDetails.IsSynchronous = isSynchronous;
                                    methodDetails.ParentHashes = methodHashes.Where(methodHash => methodHash.Key.ToString() == blockAttribute).ToDictionary(methodHash => methodHash.Key, methodHash => methodHash.Value);
                                    methodDetails.Attribute = Constants.XOn;
                                    methodDetails.AttributeArgument = blockAttribute;

                                    Generator.Get(Constants.XOn, blockAttribute).ForEach(generator =>
                                    {
                                        var xCallArguments = generator.AddMethodParameters(methodDetails, methodHashes.Where(methodHash => methodHash.Key.ToString() == blockAttribute).First().Value, GetModelParameters, Models.Select(model => model.Key).ToList());
                                        generator.AddMethodReturnTypes(methodDetails, methodHashes.Where(methodHash => methodHash.Key.ToString() == blockAttribute).First().Value, GetModelReturnsTypes, Models.Select(model => model.Key).ToList());

                                        if (!blockAttribute.Equals(Constants.XOnDesktop))
                                        {
                                            Generator.Get(Constants.XOn, Constants.XOnDesktop).ForEach(generator =>
                                            {
                                                var isReturnStatement = member.ToString().StartsWith("return");
                                                generator.AddExpression(new ExpressionDetails()
                                                {
                                                    Statement = (isReturnStatement ? "return " : string.Empty) + (methodDetails.IsSynchronous ? "await " : string.Empty) + string.Format(Constants.XCallExpression, Constants.XOnEthereumChain, methodDetails.Identifier, xCallArguments)
                                                }, lastKnownBlockHash);
                                            });
                                        }
                                    });


                                    ProcessChild(statementQueue.Dequeue(), methodDetails);

                                    lastKnownBlockHash = blockAttribute.Equals(Constants.XOnDesktop) ? methodHashes.First().Value : string.Empty;
                                }
                                else
                                {
                                    methodDetails.ParentHashes = methodHashes;
                                    methodDetails.Attribute = methodDetails.Attribute;
                                    methodDetails.AttributeArgument = methodDetails.AttributeArgument;
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

                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.IfStatement:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.ExpressionStatement:
                case SyntaxKind.LocalDeclarationStatement:
                    {
                        if (previousNodeDetails != null)
                        {
                            var expressionDetails = syntaxNode.GetExpressionDetails();
                            var statementHashes = Generator.Get(previousNodeDetails.Attribute, previousNodeDetails.AttributeArgument).Select(generator =>
                            {
                                var key = generator.GetEnumeratedType();
                                var value = generator.AddExpression(expressionDetails, previousNodeDetails.ParentHashes == null ? string.Empty : previousNodeDetails.ParentHashes[key], Models.Select(model => model.Key).ToList());
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

        private List<string> GetModelParameters(string targetChain, string modelType, string modelName)
        {
            var xChain = targetChain switch
            {
                Constants.XOnEthereumChain => XChains.Ethereum,
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
                .Where(model => model.Location == xChain && model.IsParameter)
                .Select(model => Generator.Get(Constants.XOn, targetChain).First().CreatePropertyArgument(model.Hash))
                .ToList();
        }

        private List<string> GetModelReturnsTypes(string targetChain, string modelType)
        {
            var xChain = targetChain switch
            {
                Constants.XOnEthereumChain => XChains.Ethereum,
                _ => throw new InvalidEnumArgumentException("Invalid XChain token!"),
            };

            if (modelType.IsPrimitiveType())
            {
                return new List<string>()
                {
                    $"{modelType}"
                };
            }
            else if (modelType.IsEnumerableType())
            {
                var type = modelType.Split('<', '>')[1];
                return new List<string>()
                {
                    $"{type}[] memory"
                };
            }

            return Models[modelType]
                .Where(model => model.Location == xChain)
                .Select(model => Generator.Get(Constants.XOn, targetChain).First().CreateReturnType(model.Hash))
                .ToList();
        }
    }
}