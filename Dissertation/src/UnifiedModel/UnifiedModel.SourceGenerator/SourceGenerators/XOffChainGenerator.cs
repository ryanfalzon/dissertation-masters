﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnifiedModel.SourceGenerator.CommonModels;
using UnifiedModel.SourceGenerator.Helpers;
using UnifiedModel.SourceGenerator.OffChainModels;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public class XOffChainGenerator : IXChainGenerator
    {
        public List<Class> Classes { get; set; }

        public List<ChainModel> Memory { get; set; }

        public XOffChainGenerator()
        {
            Classes = new List<Class>();
            Memory = new List<ChainModel>();
        }

        public string AddClass(Modifiers modifier, string name, string parentHash)
        {
            Class @class = new Class(modifier, name, parentHash);
            @class.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@class))));
            Memory.Add(@class);

            return @class.Hash;
        }

        public string AddField(Modifiers modifier, Types type, string name, string parentHash)
        {
            Field field = new Field(modifier, type, name, parentHash);
            field.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(field))));
            Memory.Add(field);

            return field.Hash;
        }

        public string AddMethod(Modifiers modifier, string returnType, string identifier, string parentHash)
        {
            Method method = new Method(modifier, returnType, identifier, parentHash);
            method.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(method))));
            Memory.Add(method);

            return method.Hash;
        }

        public string AddExpression(string statement, string parentHash)
        {
            Expression expression = new Expression(statement, parentHash);
            expression.Hash = Tools.ByteToHex(Tools.GetSha256Hash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(expression))));
            Memory.Add(expression);

            return expression.Hash;
        }

        public void Consume()
        {
            var classes = Memory.Where(@object => @object.GetType() == typeof(Class)).ToList();
            foreach(var @class in classes)
            {
                Classes.Add((Class)Consume(@class));
            }
        }

        private ChainModel Consume(ChainModel current)
        {
            var children = Memory.Where(potentialChild => potentialChild.ParentHash.Equals(current.Hash)).ToList();
            for(int i = 0; i < children.Count; i++)
            {
                children[i] = Consume(children[i]);
            }

            if (children.Count > 0)
            {
                var properties = current.GetType().GetProperties().Where(property => property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                foreach (var property in properties)
                {
                    var propertyType = property.PropertyType.GetGenericArguments()[0];
                    var propertyItems = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(propertyType).Invoke(null, new object[]
                    {
                        children.Where(child => child.GetType() == propertyType).ToList()
                    });

                    if (propertyItems != null)
                    {
                        object instanceOfProperty = property.GetValue(current);
                        Type typeofMainProperty = instanceOfProperty.GetType();
                        MethodInfo methodOfMainProperty = typeofMainProperty.GetMethod("AddRange");
                        methodOfMainProperty.Invoke(instanceOfProperty, new[]
                        {
                            propertyItems
                        });
                    }
                }
            }

            return current;
        }

        public override string ToString()
        {
            return $"{string.Join("\n", Classes.Select(@class => @class.ToString()))}";
        }
    }
}