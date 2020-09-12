using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
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

        public LexicalAnalyser(string filePath)
        {
            FileContent = File.ReadAllText(filePath);
            Generator = new XChainGeneratorFactory();
        }

        public void Process()
        {
            var root = CSharpSyntaxTree.ParseText(FileContent).GetRoot();
            ProcessChild(root);

            var files = Generator.Consume();
            foreach(var (filename, contents) in files)
            {
                File.WriteAllText($"C://temp/{filename}.txt", contents);
            }
        }

        private void ProcessChild(SyntaxNode syntaxNode, Dictionary<XChains, string> parentHashes = null, string previousAttribute = null, string previousAttributeArgument = null)
        {
            switch (syntaxNode.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)syntaxNode;

                        if (classDeclarationSyntax.AttributeLists.Count > 0)
                        {
                            Enum.TryParse(classDeclarationSyntax.Modifiers.First().ValueText, out Modifiers modifier);
                            var name = classDeclarationSyntax.Identifier.ValueText;
                            var baseType = classDeclarationSyntax.BaseList?.Types.ToString();
                            var attribute = classDeclarationSyntax.AttributeLists[0].Attributes[0].Name.ToString();
                            var attributeArgument = classDeclarationSyntax.AttributeLists.FirstOrDefault()?.Attributes.FirstOrDefault()?.ArgumentList.Arguments.FirstOrDefault()?.ToString();
                            var classHashes = Generator.Get(attribute).Select(generator =>
                            {
                                var key = generator.GetEnumeratedType();
                                var value = generator.AddClass(modifier, name, !string.IsNullOrEmpty(baseType) && baseType.Equals("XModel"), parentHashes == null ? string.Empty : parentHashes[key]);
                                return new KeyValuePair<XChains, string>(key, value);
                            }).ToDictionary(x => x.Key, x => x.Value);

                            foreach (var member in classDeclarationSyntax.Members)
                            {
                                ProcessChild(member, classHashes, attribute, attributeArgument);
                            }
                        }

                        break;
                    }

                case SyntaxKind.FieldDeclaration:
                    {
                        FieldDeclarationSyntax fieldDeclarationSyntax = (FieldDeclarationSyntax)syntaxNode;

                        if (fieldDeclarationSyntax.AttributeLists.Count > 0)
                        {
                            Enum.TryParse(fieldDeclarationSyntax.Modifiers.First().ValueText, out Modifiers modifier);
                            Enum.TryParse(fieldDeclarationSyntax.Declaration.Type.ToString(), out Types type);
                            var name = fieldDeclarationSyntax.Declaration.Variables.ToString();
                            var attribute = fieldDeclarationSyntax.AttributeLists[0].Attributes[0].Name.ToString();
                            var attributeArgument = fieldDeclarationSyntax.AttributeLists.FirstOrDefault()?.Attributes.FirstOrDefault()?.ArgumentList.Arguments.FirstOrDefault()?.ToString();
                            var fieldHashes = Generator.Get(attribute, attributeArgument).Select(generator =>
                            {
                                var key = generator.GetEnumeratedType();
                                var value = generator.AddField(modifier, type, name, parentHashes == null ? string.Empty : parentHashes[key]);
                                return new KeyValuePair<XChains, string>(key, value);
                            }).ToDictionary(x => x.Key, x => x.Value);
                        }

                        break;
                    }

                case SyntaxKind.MethodDeclaration:
                    {
                        MethodDeclarationSyntax methodDeclarationSyntax = (MethodDeclarationSyntax)syntaxNode;

                        if (methodDeclarationSyntax.AttributeLists.Count > 0)
                        {
                            Enum.TryParse(methodDeclarationSyntax.Modifiers.First().ValueText, out Modifiers modifier);
                            var returnType = methodDeclarationSyntax.ReturnType.ToString();
                            var identifier = methodDeclarationSyntax.Identifier.ValueText;
                            var parameters = methodDeclarationSyntax.ParameterList.Parameters.ToString();
                            var attribute = methodDeclarationSyntax.AttributeLists[0].Attributes[0].Name.ToString();
                            var attributeArgument = methodDeclarationSyntax.AttributeLists.FirstOrDefault()?.Attributes.FirstOrDefault()?.ArgumentList.Arguments.FirstOrDefault()?.ToString();
                            var methodHashes = Generator.Get(attribute).Select(generator =>
                            {
                                var key = generator.GetEnumeratedType();
                                var value = generator.AddMethod(modifier, returnType, identifier, parentHashes == null ? string.Empty : parentHashes[key]); ;
                                return new KeyValuePair<XChains, string>(key, value);
                            }).ToDictionary(x => x.Key, x => x.Value);

                            var statementQueue = new Queue<SyntaxNode>(methodDeclarationSyntax.Body.Statements);
                            while(statementQueue.Count > 0)
                            {
                                var member = statementQueue.Dequeue();
                                if(Regex.IsMatch(member.ToString(), Constants.XOnChainRegex))
                                {
                                    if(statementQueue.Peek().Kind() == SyntaxKind.Block)
                                    {
                                        var blockAttribute = member.ToString().Split('"')[1];

                                        var memberHashes = Generator.Get(Constants.XOnChain, blockAttribute).Select(generator =>
                                        {
                                            var key = generator.GetEnumeratedType();
                                            var value = generator.AddMethod(Modifiers.@public, returnType, identifier, parentHashes == null ? string.Empty : parentHashes[key]);
                                            return new KeyValuePair<XChains, string>(key, value);
                                        }).ToDictionary(x => x.Key, x => x.Value);

                                        ProcessChild(statementQueue.Dequeue(), memberHashes, Constants.XOnChain, blockAttribute);
                                    }
                                }
                                else
                                {
                                    ProcessChild(member, methodHashes, attribute, attributeArgument);
                                }
                            }
                        }

                        break;
                    }

                case SyntaxKind.ExpressionStatement:
                    {
                        ExpressionStatementSyntax expressionStatementSyntax = (ExpressionStatementSyntax)syntaxNode;

                        if (previousAttribute != null)
                        {
                            var statement = expressionStatementSyntax.Expression.ToString();
                            var statementHashes = Generator.Get(previousAttribute, previousAttributeArgument).Select(generator =>
                            {
                                var key = generator.GetEnumeratedType();
                                var value = generator.AddExpression(statement, parentHashes == null ? string.Empty : parentHashes[key]);
                                return new KeyValuePair<XChains, string>(key, value);
                            }).ToDictionary(x => x.Key, x => x.Value);
                        }

                        break;
                    }

                case SyntaxKind.Block:
                    {
                        BlockSyntax blockSyntax = (BlockSyntax)syntaxNode;

                        if (previousAttribute != null)
                        {
                            foreach (var child in blockSyntax.ChildNodes())
                            {
                                ProcessChild(child, parentHashes, previousAttribute, previousAttributeArgument);
                            }
                        }

                        break;
                    }

                default:
                    {
                        foreach (var member in syntaxNode.ChildNodes())
                        {
                            ProcessChild(member);
                        }

                        break;
                    }
            }
        }
    }
}