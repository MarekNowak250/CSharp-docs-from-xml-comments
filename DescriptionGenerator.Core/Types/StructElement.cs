﻿using DescriptionGenerator.Core.Interfaces;
using DescriptionGenerator.Core.Printing;

namespace DescriptionGenerator.Core.Types
{
    public class StructElement
    {
        public StructElement(string name, string type, string description)
        {
            Name = name;
            Type = type;
            Description = description;
        }

        public string Name { get; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }

        public virtual IPrinter GetPrinter(PrinterType printerType)
        {
            switch (printerType)
            {
                case PrinterType.Markdown:
                    return new MDStructElementPrinter(this);
                default:
                    throw new NotImplementedException($"Type {printerType} is not supported for struct element");
            }
        }
    }
}