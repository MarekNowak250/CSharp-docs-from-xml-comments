﻿using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DescriptionGenerator.Core
{
    public class AssemblyReader
    {
        private readonly string assemblyPath;

        public AssemblyReader(string assemblyPath)
        {
            this.assemblyPath = assemblyPath;
        }

        public Assembly Read()
        {
            return Assembly.LoadFrom(assemblyPath);
        }
    }

    public class AssemblyProcessor
    {
        public Assembly ass;
        private readonly Config config;

        public AssemblyProcessor(Assembly ass, Config config)
        {
            this.ass = ass;
            this.config = config;
        }

        public NodeContainer[] ProcessClass(string name)
        {
            var type = ass.GetType(name);
            return ProcessContainer(type);
        }

        public NodeContainer[] ProcessContainer(Type type, List<string> processedElementNames = null)
        {
            if (processedElementNames == null)
                processedElementNames = new List<string>();
            else if (processedElementNames.Contains(type.FullName)
                || (!config.IncludeNested && processedElementNames.Count > 0))
                return new NodeContainer[] { };

            if (type.IsEnum)
            {
                return ProcessEnum(type, processedElementNames);
            }

            return ProcessClass(type, processedElementNames);

        }

        private NodeContainer[] ProcessClass(Type type, List<string> processedElementNames = null)
        {
            if (type == null
                || (!type.IsClass && !type.IsInterface && !type.IsEnum)
                || type.Name.StartsWith('<')
                || type.Namespace.StartsWith("System"))
                return new NodeContainer[0];
            //Console.WriteLine(type.FullName);

            var typeSummary = config.IncludeContainersSummary ? type.GetSummary() : string.Empty;
            var classContainer = new NodeContainer(type.Name, "class", type.GetSummary(), type.Namespace);

            processedElementNames.Add(type.FullName);

            //var methodSummary = type.GetMethod(name.Split('.').Last()).GetSummary();

            (List<NodeContainer> containers, List<StructElement> properties) result = ProcessProperties(type, processedElementNames);
            var containers = result.containers;
            containers.Add(classContainer);
            classContainer.Properties.AddRange(result.properties);


            return containers.ToArray();
        }

        private (List<NodeContainer> containers, List<StructElement> properties) ProcessProperties(Type type, List<string> processedElementNames)
        {
            var classProperties = new List<StructElement>();
            var containers = new List<NodeContainer>();

            foreach (var prop in type.GetProperties())
            {
                // Console.WriteLine(prop.Name);

                var propType = prop.PropertyType;
                var structElement = GeneratePropertyElement(prop);
                classProperties.Add(structElement);

                if (processedElementNames.Contains(propType.FullName))
                    continue;
                if (propType.FullName == typeof(object).FullName)
                    continue;

                if (propType.IsEnum)
                {
                    if (processedElementNames.Contains(propType.FullName))
                        continue;

                    containers.AddRange(ProcessContainer(propType, processedElementNames));
                    continue;
                }


                if (propType.IsValueType || propType == typeof(string))
                {
                    processedElementNames.Add(propType.FullName);
                    continue;
                }

                if (propType.IsArray)
                {
                    containers.AddRange(ProcessArrays(propType, structElement, processedElementNames));
                    continue;
                }

                try
                {
                    // is it a generic collection?
                    var argType = propType.GetGenericArguments()[0];
                    structElement.Type = $"{propType.Name.Substring(0, propType.Name.Length - 2)}<{argType.Name}>";

                    if (argType.IsValueType || argType == typeof(string))
                    {
                        if (!processedElementNames.Contains(argType.FullName))
                            processedElementNames.Add(argType.FullName);
                        continue;
                    }

                    else
                    {
                        containers.AddRange(ProcessContainer(argType, processedElementNames));
                        continue;
                    }

                }
                catch
                {
                    // it is not a collection
                }

                if (propType.IsInterface)
                {
                    continue;
                }


                containers.AddRange(ProcessContainer(propType, processedElementNames));
            }

            return (containers, classProperties);
        }

        private NodeContainer[] ProcessArrays(Type propType, StructElement currentElement, List<string> processedElementNames)
        {
            var argType = propType.GetElementType();
            int count = 0;

            while (argType.IsArray)
            {
                count++;
                argType = argType.GetElementType();
            }

            currentElement.Type = $"{argType.Name}";
            for (int i = 0; i < count; i++)
            {
                currentElement.Type += "[]";
            }

            if (argType.IsValueType || argType == typeof(string))
            {
                if (!processedElementNames.Contains(argType.FullName))
                    processedElementNames.Add(argType.FullName);
                return Array.Empty<NodeContainer>();
            }
            else
            {
                return ProcessContainer(argType, processedElementNames);
            }
        }

        private NodeContainer[] ProcessMethods(Type baseType, StructElement currentElement, List<string> processedElementNames)
        {
            var methods = baseType.GetMethods(BindingFlags.Public);
            int count = 0;


            foreach(var method in methods)
            {
                var summary = method.GetSummary();
                var name = method.Name;

                var returnType = method.ReturnType;
                var parameters = method.GetParameters();

                foreach(var param in parameters)
                {
                    var structElement = GenerateParameterElement(param);
                }

                var methodStruct = new MethodContainer(name, summary, arguments: null, returnType, null, null);
            }

            return null;
        }

        private NodeContainer[] ProcessEnum(Type type, List<string> processedElementNames = null)
        {
            var enumSummary = config.IncludeContainersSummary ? type.GetSummary() : string.Empty;
            var enumContainer = new NodeContainer(type.Name, "enum", enumSummary, type.Namespace);
            var containers = new List<NodeContainer>
            {
                enumContainer
            };
            processedElementNames.Add(type.FullName);

            foreach (FieldInfo field in type.GetFields())
            {
                if (field.Name == "value__")
                    continue;

                enumContainer.Properties.Add(GenerateEnumElement(field));
            }

            return new NodeContainer[] { enumContainer };
        }


        private StructElement GeneratePropertyElement(PropertyInfo propertyInfo)
        {
            var propertySummary = config.IncludePropertiesSummary ? propertyInfo.GetSummary() : string.Empty;
            return new StructElement(propertyInfo.Name, propertyInfo.PropertyType.Name, propertySummary);
        }

        private StructElement GenerateParameterElement(ParameterInfo propertyInfo)
        {
            var propertySummary = string.Empty;
            return new StructElement(propertyInfo.Name, propertyInfo.ParameterType.Name, propertySummary);
        }


        private StructElement GenerateEnumElement(FieldInfo fieldInfo)
        {
            var fieldSummary = config.IncludePropertiesSummary ? fieldInfo.GetSummary() : string.Empty;
            var fieldRepresentation = "";
            try
            {
                var fieldValue = (int)fieldInfo.GetValue(null);
                fieldRepresentation = fieldValue.ToString();
            }
            catch
            {
                fieldRepresentation = fieldInfo.GetValue(null).ToString();
            }

            return new StructElement(fieldInfo.Name, fieldRepresentation ?? string.Empty, fieldSummary);
        }
    }

    public class MethodContainer : StructElement
    {
        private readonly List<(string name, Type type)> arguments;
        private readonly Type output;
        public readonly List<StructElement> ArgumentsElements;
        public readonly StructElement OutputElement;

        public MethodContainer(string name,
                               string description,
                               List<(string, Type)> arguments,
                               Type output, 
                               List<StructElement> argumentsElements,
                               StructElement outputElement) : base(name, type: "method", description)
        {
            this.arguments = arguments ?? new();
            this.output = output;
            ArgumentsElements = argumentsElements;
            OutputElement = outputElement;
        }

        public string GetOutputJson()
        {
            if (output == typeof(string))
                return "";

            var outputObject = RuntimeHelpers.GetUninitializedObject(output);
            return JsonConvert.SerializeObject(outputObject);
        }

        public string GetInputJson()
        {
            string[] output = new string[arguments.Count];
            for (int i = 0; i < arguments.Count; i++)
            {
                var type = arguments[i].type;
                var defaultValue = type.IsValueType
                   ? RuntimeHelpers.GetUninitializedObject(type)
                   : "null";
                if (type == typeof(string))
                    defaultValue = "";

                output[i] = $"\"{arguments[i].name}\": {defaultValue}";
            }

            return "{" + string.Join($",{Environment.NewLine}", output) + "}";
        }

        public override IPrinter GetPrinter(PrinterType printerType)
        {
            switch (printerType)
            {
                case PrinterType.Markdown:
                    return new MDMethodPrinter(this);
                default:
                    throw new NotImplementedException($"Type {printerType} is not supported for method element");
            }
        }
    }
}
