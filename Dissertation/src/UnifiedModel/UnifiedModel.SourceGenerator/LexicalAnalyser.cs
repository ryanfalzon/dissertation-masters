using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Data;
using System.IO;
using System.Linq;
using UnifiedModel.SourceGenerator.OffChainModels;

namespace UnifiedModel.SourceGenerator
{
    public class LexicalAnalyser
    {
        public string FileContent { get; set; }

        public LexicalAnalyser(string filePath)
        {
            FileContent = File.ReadAllText(filePath);
        }

        public void Process()
        {
            var root = CSharpSyntaxTree.ParseText(FileContent).GetRoot();
            ProcessChildren(root);
        }

        private void ProcessChildren(SyntaxNode syntaxNode)
        {
            foreach (var child in syntaxNode.ChildNodes())
            {
                switch (child.Kind())
                {
                    case SyntaxKind.ClassDeclaration:
                        {
                            ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)child;
                            
                            // Check if class has an attribute
                            if (classDeclarationSyntax.AttributeLists.Count > 0)
                            {
                                Enum.TryParse(classDeclarationSyntax.Modifiers.First().ValueText, out Modifiers modifier);
                                var name = classDeclarationSyntax.Identifier.ValueText;
                                var attribute = classDeclarationSyntax.AttributeLists[0].Attributes.ToString();
                                var @class = new Class(modifier, name);

                                foreach (var member in classDeclarationSyntax.Members)
                                {
                                    ProcessChildren(member);
                                }
                            }

                            break;
                        }

                    case SyntaxKind.MethodDeclaration:
                        {
                            break;
                        }

                    case SyntaxKind.FieldDeclaration:
                        {
                            Console.WriteLine("Field");
                            break;
                        }

                    default:
                        continue;
                }
            }
        }
    }
}