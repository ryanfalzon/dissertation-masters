using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using UnifiedModel.SourceGenerator.CommonModels;

namespace UnifiedModel.SourceGenerator.Helpers
{
    public static class Extractor
    {
        public static ClassDetails GetClassDetails(this ClassDeclarationSyntax classDeclarationSyntax)
        {
            Enum.TryParse(classDeclarationSyntax.Modifiers.First().ValueText, out Modifiers modifier);
            var name = classDeclarationSyntax.Identifier.ValueText;
            var baseType = classDeclarationSyntax.BaseList?.Types.First().ToString();
            var isModel = !string.IsNullOrEmpty(baseType) && Regex.IsMatch(baseType, Constants.XModelRegex);
            var modelLocation = isModel ? baseType.ToString().Replace("\"", "").Split('(', ')')[1].Split(',').First() : string.Empty;
            var attribute = classDeclarationSyntax.AttributeLists[0].Attributes[0].Name.ToString();
            var attributeArgument = classDeclarationSyntax.AttributeLists.FirstOrDefault()?.Attributes.FirstOrDefault()?.ArgumentList.Arguments.FirstOrDefault()?.ToString();

            return new ClassDetails()
            {
                Modifier = modifier,
                Name = name,
                IsModel = isModel,
                ModelLocation = modelLocation,
                Attribute = attribute,
                AttributeArgument = attributeArgument
            };
        }

        public static FieldDetails GetFieldDetails(this FieldDeclarationSyntax fieldDeclarationSyntax)
        {
            Enum.TryParse(fieldDeclarationSyntax.Modifiers.First().ValueText, out Modifiers modifier);
            Enum.TryParse(fieldDeclarationSyntax.Declaration.Type.ToString(), out Types type);
            var name = fieldDeclarationSyntax.Declaration.Variables.ToString();
            var attribute = fieldDeclarationSyntax.AttributeLists.Count == 0 ? string.Empty : fieldDeclarationSyntax.AttributeLists[0].Attributes[0].Name.ToString();
            var attributeArgument = fieldDeclarationSyntax.AttributeLists.Count == 0 ? string.Empty : fieldDeclarationSyntax.AttributeLists.FirstOrDefault()?.Attributes.FirstOrDefault()?.ArgumentList.Arguments.FirstOrDefault()?.ToString();

            return new FieldDetails()
            {
                Modifier = modifier,
                Type = type,
                Name = name,
                Attribute = attribute,
                AttributeArgument = attributeArgument
            };
        }

        public static ConstructorDetails GetConstructorDetails(this ConstructorDeclarationSyntax constructorDeclarationSyntax)
        {
            Enum.TryParse(constructorDeclarationSyntax.Modifiers.First().ValueText, out Modifiers modifier);
            var identifier = constructorDeclarationSyntax.Identifier.ValueText;
            var parameters = constructorDeclarationSyntax.ParameterList.Parameters.ToString();
            var attribute = constructorDeclarationSyntax.AttributeLists.Count == 0 ? string.Empty : constructorDeclarationSyntax.AttributeLists[0].Attributes[0].Name.ToString();
            var attributeArgument = constructorDeclarationSyntax.AttributeLists.Count == 0 ? string.Empty : constructorDeclarationSyntax.AttributeLists.FirstOrDefault()?.Attributes.FirstOrDefault()?.ArgumentList.Arguments.FirstOrDefault()?.ToString();

            return new ConstructorDetails()
            {
                Modifier = modifier,
                Identifier = identifier,
                Parameters = parameters,
                Attribute = attribute,
                AttributeArgument = attributeArgument
            };
        }

        public static MethodDetails GetMethodDetails(this MethodDeclarationSyntax methodDeclarationSyntax)
        {
            Enum.TryParse(methodDeclarationSyntax.Modifiers.First().ValueText, out Modifiers modifier);
            var returnType = methodDeclarationSyntax.ReturnType.ToString();
            var identifier = methodDeclarationSyntax.Identifier.ValueText;
            var parameters = methodDeclarationSyntax.ParameterList.Parameters.ToString();

            return new MethodDetails()
            {
                Modifier = modifier,
                ReturnType = returnType,
                Identifier = identifier,
                Parameters = parameters
            };
        }

        public static MethodDetails GenerateOnChainMethodDetails(this MethodDetails methodDetails, string newParameters)
        {
            return new MethodDetails()
            {
                Modifier = Modifiers.@public,
                ReturnType = methodDetails.ReturnType,
                Identifier = methodDetails.Identifier,
                Parameters = newParameters,
                ParameterAnchor = methodDetails.Parameters
            };
        }

        public static ExpressionDetails GetSyntaxNodeDetails(this SyntaxNode expressionStatementSyntax)
        {
            return new ExpressionDetails()
            {
                Statement = expressionStatementSyntax.ToString()
            };
        }
    }
}