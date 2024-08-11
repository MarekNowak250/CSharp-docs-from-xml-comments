﻿using Microsoft.VisualBasic;
using System.Reflection;
using System.Xml;

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

        public AssemblyProcessor(Assembly ass)
        {
            this.ass = ass;
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
            else if (processedElementNames.Contains(type.FullName))
                return new NodeContainer[] { };

            if (type.IsEnum)
            {
                return ProcessEnum(type, processedElementNames);
            }

            return ProcessClass(type, processedElementNames);

        }

        private NodeContainer[] ProcessClass(Type type, List<string> processedElementNames = null)
        {
            if((!type.IsClass && !type.IsInterface && !type.IsEnum) || type.Name.StartsWith('<') || type.Namespace.StartsWith("System"))
                return new NodeContainer[0];
            //Console.WriteLine(type.FullName);

            var typeSummary = type.GetSummary();
            var classContainer = new NodeContainer(type.Name, "class", type.GetSummary());
            var containers = new List<NodeContainer>
            {
                classContainer
            };

            processedElementNames.Add(type.FullName);

            //var methodSummary = type.GetMethod(name.Split('.').Last()).GetSummary();

            foreach (var prop in type.GetProperties())
            {
                // Console.WriteLine(prop.Name);

                var propType = prop.PropertyType;
                var structElement = new StructElement(prop.Name, propType.Name, prop.GetSummary());
                classContainer.Properties.Add(structElement);

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
                    var argType = propType.GetElementType();
                    int count = 0;

                    while (argType.IsArray)
                    {
                        count++;
                        argType = argType.GetElementType();
                    }

                    structElement.Type = $"{argType.Name}";
                    for (int i = 0; i < count; i++)
                    {
                        structElement.Type += "[]";
                    }

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

                try
                {
                    // is it a  generic collection?
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

            return containers.ToArray();
        }

        private NodeContainer[] ProcessEnum(Type type, List<string> processedElementNames = null)
        {
            var enumContainer = new NodeContainer(type.Name, "enum", type.GetSummary());
            var containers = new List<NodeContainer>
            {
                enumContainer
            };
            processedElementNames.Add(type.FullName);

            foreach (var field in type.GetFields())
            {
                if (field.Name == "value__")
                    continue;
                var structElement = new StructElement(field.Name, ((int)field.GetValue(null)).ToString(), field.GetSummary());
                enumContainer.Properties.Add(structElement);
            }

            return new NodeContainer[] { enumContainer };
        }

        //private ClassContainer[] HandleArray()
        //{

        //}
    }

    public class NodeContainer : StructElement, IDataContainer
    {
        public List<StructElement> Properties { get; }

        public NodeContainer(string name, string type, string description, List<StructElement> properties = null) : base(name, type, description)
        {
            Properties = properties ?? new();
        }

        public string GetType()
        {
            return Type;
        }
    }

    // https://stackoverflow.com/questions/15602606/programmatically-get-summary-comments-at-runtime
    /// <summary>
    /// Utility class to provide documentation for various types where available with the assembly
    /// </summary>
    public static class DocumentationExtensions
    {
        /// <summary>
        /// Provides the documentation comments for a specific method
        /// </summary>
        /// <param name="methodInfo">The MethodInfo (reflection data ) of the member to find documentation for</param>
        /// <returns>The XML fragment describing the method</returns>
        public static XmlElement GetDocumentation(this MethodInfo methodInfo)
        {
            // Calculate the parameter string as this is in the member name in the XML
            var parametersString = "";
            foreach (var parameterInfo in methodInfo.GetParameters())
            {
                if (parametersString.Length > 0)
                {
                    parametersString += ",";
                }

                parametersString += parameterInfo.ParameterType.FullName;
            }

            //AL: 15.04.2008 ==> BUG-FIX remove “()” if parametersString is empty
            if (parametersString.Length > 0)
                return methodInfo.DeclaringType.XmlFromName('M', methodInfo.Name + "(" + parametersString + ")");
            else
                return methodInfo.DeclaringType.XmlFromName('M', methodInfo.Name);
        }

        /// <summary>
        /// Provides the documentation comments for a specific member
        /// </summary>
        /// <param name="memberInfo">The MemberInfo (reflection data) or the member to find documentation for</param>
        /// <returns>The XML fragment describing the member</returns>
        public static XmlElement GetDocumentation(this MemberInfo memberInfo)
        {
            // First character [0] of member type is prefix character in the name in the XML
            return memberInfo.DeclaringType.XmlFromName(memberInfo.MemberType.ToString()[0], memberInfo.Name);
        }
        /// <summary>
        /// Returns the Xml documenation summary comment for this member
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static string GetSummary(this MemberInfo memberInfo)
        {
            var element = memberInfo.GetDocumentation();
            var summaryElm = element?.SelectSingleNode("summary");
            if (summaryElm == null) return "";
            return summaryElm.InnerText.Trim();
        }

        /// <summary>
        /// Provides the documentation comments for a specific type
        /// </summary>
        /// <param name="type">Type to find the documentation for</param>
        /// <returns>The XML fragment that describes the type</returns>
        public static XmlElement GetDocumentation(this Type type)
        {
            // Prefix in type names is T
            return type.XmlFromName('T', "");
        }

        /// <summary>
        /// Gets the summary portion of a type's documenation or returns an empty string if not available
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetSummary(this Type type)
        {
            var element = type.GetDocumentation();
            if (element == null) return string.Empty;
            var summaryElm = element?.SelectSingleNode("summary");
            if (summaryElm == null) return "";
            return summaryElm.InnerText.Trim();
        }

        /// <summary>
        /// Obtains the XML Element that describes a reflection element by searching the 
        /// members for a member that has a name that describes the element.
        /// </summary>
        /// <param name="type">The type or parent type, used to fetch the assembly</param>
        /// <param name="prefix">The prefix as seen in the name attribute in the documentation XML</param>
        /// <param name="name">Where relevant, the full name qualifier for the element</param>
        /// <returns>The member that has a name that describes the specified reflection element</returns>
        private static XmlElement XmlFromName(this Type type, char prefix, string name)
        {
            string fullName;

            if (string.IsNullOrEmpty(name))
                fullName = prefix + ":" + type.FullName;
            else
                fullName = prefix + ":" + type.FullName + "." + name;

            var xmlDocument = type.Assembly.XmlFromAssembly();
            if (xmlDocument == null)
                return null;
            var matchedElement = xmlDocument["doc"]["members"].SelectSingleNode("member[@name='" + fullName + "']") as XmlElement;

            return matchedElement;
        }

        /// <summary>
        /// A cache used to remember Xml documentation for assemblies
        /// </summary>
        private static readonly Dictionary<Assembly, XmlDocument> Cache = new Dictionary<Assembly, XmlDocument>();

        /// <summary>
        /// A cache used to store failure exceptions for assembly lookups
        /// </summary>
        private static readonly Dictionary<Assembly, Exception> FailCache = new Dictionary<Assembly, Exception>();

        /// <summary>
        /// Obtains the documentation file for the specified assembly
        /// </summary>
        /// <param name="assembly">The assembly to find the XML document for</param>
        /// <returns>The XML document</returns>
        /// <remarks>This version uses a cache to preserve the assemblies, so that 
        /// the XML file is not loaded and parsed on every single lookup</remarks>
        public static XmlDocument XmlFromAssembly(this Assembly assembly)
        {
            if (FailCache.ContainsKey(assembly))
            {
                throw FailCache[assembly];
            }

            try
            {

                if (!Cache.ContainsKey(assembly))
                {
                    // load the docuemnt into the cache
                    Cache[assembly] = XmlFromAssemblyNonCached(assembly);
                }

                return Cache[assembly];
            }
            catch (Exception exception)
            {
                FailCache[assembly] = exception;
                throw;
            }
        }

        /// <summary>
        /// Loads and parses the documentation file for the specified assembly
        /// </summary>
        /// <param name="assembly">The assembly to find the XML document for</param>
        /// <returns>The XML document</returns>
        private static XmlDocument XmlFromAssemblyNonCached(Assembly assembly)
        {
            var assemblyFilename = assembly.Location;

            if (!string.IsNullOrEmpty(assemblyFilename))
            {
                StreamReader streamReader;

                try
                {
                    streamReader = new StreamReader(Path.ChangeExtension(assemblyFilename, ".xml"));
                }
                catch (FileNotFoundException exception)
                {
                    Console.WriteLine("Cannot read {0}", assemblyFilename);
                    return null;
                    //throw new Exception("XML documentation not present (make sure it is turned on in project properties when building)", exception);
                }

                var xmlDocument = new XmlDocument();
                xmlDocument.Load(streamReader);
                return xmlDocument;
            }
            else
            {
                throw new Exception("Could not ascertain assembly filename", null);
            }
        }
    }
}