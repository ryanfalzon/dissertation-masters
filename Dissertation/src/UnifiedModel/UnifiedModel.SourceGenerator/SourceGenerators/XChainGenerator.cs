using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnifiedModel.SourceGenerator.CommonModels;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public abstract class XChainGenerator : IXChainGenerator
    {
        public List<ChainModel> Models { get; set; }

        public List<ChainModel> Memory { get; set; }

        public string FileExtension { get; set; }

        public XChainGenerator()
        {
            Models = new List<ChainModel>();
            Memory = new List<ChainModel>();
        }

        public abstract string AddClass(ClassDetails classDetails, string parentHash);

        public abstract string AddField(FieldDetails fieldDetails, string parentHash);

        public abstract string AddMethod(MethodDetails methodDetails, string parentHash);

        public abstract string AddExpression(ExpressionDetails expressionDetails, string parentHash);

        public abstract string CreatePropertyArgument(string hash);

        public override string ToString()
        {
            return $"{string.Join("\n", Models.Select(@class => @class.ToString()))}";
        }

        public virtual void Consume()
        {
            var models = Memory.Where(item => item.ParentHash.Equals(string.Empty));

            foreach(var item in models)
            {
                Models.Add(Consume(item));
            }
        }

        protected virtual ChainModel Consume(ChainModel current)
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
    }
}