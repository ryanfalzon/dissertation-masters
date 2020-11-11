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

                                    Generator.Get(Constants.XOn, blockAttribute).ForEach(generator => generator.AddMethodParameters(constructorDetails, constructorHashes.Where(methodHash => methodHash.Key.ToString() == blockAttribute).First().Value, GetModelParameters));

                                    ProcessChild(statementQueue.Dequeue(), constructorDetails);

                                    lastKnownBlockHash = blockAttribute.Equals(Constants.XOnDesktop) ? constructorHashes.First().Value : string.Empty;
                                }
                                else
                                {
                                    constructorDetails.ParentHashes = constructorHashes;
                                    constructorDetails.Attribute = previousNodeDetails.Attribute;
                                    constructorDetails.AttributeArgument = previousNodeDetails.AttributeArgument;
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
                                    var isAsynchronous = member.ToString().StartsWith('~');

                                    methodDetails.Arguments = argumentList;

                                    methodDetails.IsAsynchronous = isAsynchronous;
                                    methodDetails.ParentHashes = methodHashes.Where(methodHash => methodHash.Key.ToString() == blockAttribute).ToDictionary(methodHash => methodHash.Key, methodHash => methodHash.Value);
                                    methodDetails.Attribute = Constants.XOn;
                                    methodDetails.AttributeArgument = blockAttribute;

                                    Generator.Get(Constants.XOn, blockAttribute).ForEach(generator => {
                                        var xCallArguments = generator.AddMethodParameters(methodDetails, methodHashes.Where(methodHash => methodHash.Key.ToString() == blockAttribute).First().Value, GetModelParameters);
                                        Generator.Get(Constants.XOn, Constants.XOnDesktop).ForEach(generator =>
                                        {
                                            generator.AddExpression(new ExpressionDetails()
                                            {
                                                Statement = (methodDetails.IsAsynchronous ? "await " : string.Empty) + string.Format(Constants.XCallExpression, Constants.XOnEthereumChain, methodDetails.Identifier, xCallArguments)
                                            }, lastKnownBlockHash);
                                        });
                                    });
                                    

                                    ProcessChild(statementQueue.Dequeue(), methodDetails);

                                    lastKnownBlockHash = blockAttribute.Equals(Constants.XOnDesktop) ? methodHashes.First().Value : string.Empty;
                                }
                                else
                                {
                                    methodDetails.ParentHashes = methodHashes;
                                    methodDetails.Attribute = previousNodeDetails.Attribute;
                                    methodDetails.AttributeArgument = previousNodeDetails.AttributeArgument;
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

                case SyntaxKind.IfStatement:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.ExpressionStatement:
                case SyntaxKind.LocalDeclarationStatement:
                    {
                        if (previousNodeDetails != null)
                        {
                            var expressionDetails = syntaxNode.GetSyntaxNodeDetails();
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
                .Where(model => model.Location == xChain)
                .Select(model => Generator.Get(Constants.XOn, targetChain).First().CreatePropertyArgument(model.Hash))
                .ToList();
        }
    }
}