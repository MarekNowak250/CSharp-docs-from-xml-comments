using DescriptionGenerator.Core.Interfaces;
using DescriptionGenerator.Core.Printing;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace DescriptionGenerator.Core.Types
{
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
            if (output == typeof(string) || output == typeof(void) || output.IsAbstract)
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
