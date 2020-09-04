using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using UnifiedModel.SourceGenerator.OffChainModels;
using UnifiedModel.SourceGenerator.SourceGenerators;

namespace UnifiedModel.SourceGenerator
{
    public class LexicalAnalyser
    {
        public string FileContent { get; set; }

        private XChainGeneratorFactory Generator;

        public LexicalAnalyser(string filePath)
        {
            FileContent = File.ReadAllText(filePath);
            Generator = new XChainGeneratorFactory();
        }

        public void Process()
        {
            var root = CSharpSyntaxTree.ParseText(FileContent).GetRoot();
            ProcessChild(root, string.Empty);
            var files = Generator.Consume();
        }

        private void ProcessChild(SyntaxNode syntaxNode, string parentHash)
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
                            var attribute = classDeclarationSyntax.AttributeLists[0].Attributes[0].Name.ToString();
                            var classHash = Generator.Get(attribute).AddClass(modifier, name, parentHash);

                            foreach (var member in classDeclarationSyntax.Members)
                            {
                                ProcessChild(member, classHash);
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
                            var fieldHash = Generator.Get(attribute).AddField(modifier, type, name, parentHash);
                        }

                        break;
                    }

                default:
                    {
                        foreach (var member in syntaxNode.ChildNodes())
                        {
                            ProcessChild(member, string.Empty);
                        }

                        break;
                    }
            }
        }
    }
}