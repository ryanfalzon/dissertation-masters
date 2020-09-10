using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnifiedModel.SourceGenerator.CommonModels;
using UnifiedModel.SourceGenerator.OffChainModels;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public abstract class XOnChainGenerator : IXChainGenerator
    {
        public List<Class> Classes { get; set; }

        public List<ChainModel> Memory { get; set; }

        public XOnChainGenerator()
        {
            Classes = new List<Class>();
            Memory = new List<ChainModel>();
        }

        public abstract string AddClass(Modifiers modifier, string name, string parentHash);
        public abstract string AddField(Modifiers modifier, Types type, string name, string parentHash);
        public abstract string AddMethod(Modifiers modifier, string returnType, string identifier, string parentHash);
        public abstract string AddExpression(string statement, string parentHash);

        public void Consume()
        {
            var classes = Memory.Where(@object => @object.GetType() == typeof(Class)).ToList();
            foreach (var @class in classes)
            {
                Classes.Add((Class)Consume(@class));
            }
        }

        private ChainModel Consume(ChainModel current)
        {
            var children = Memory.Where(potentialChild => potentialChild.ParentHash.Equals(current.Hash)).ToList();
            for (int i = 0; i < children.Count; i++)
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