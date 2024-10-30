using DescriptionGenerator.Core;
using DescriptionGenerator.Core.Interfaces;
using DescriptionGenerator.Core.Types;

namespace DescriptionGenerator
{
    public class MDGeneratorByText
    {
        public void Start()
        {
            var config = Config.LoadConfig();

            while (true)
            {
                Console.WriteLine("\nEnter type!\n1: class\n2: enum\n3: exit");
                var type = Console.ReadLine();
                Handler handler;

                if (!int.TryParse(type, out int elementType))
                {
                    Console.WriteLine("\nPlease provide valid type!\n");
                    continue;
                }

                switch (elementType)
                {
                    case 1:
                        handler = new ClassHandler();
                        break;
                    case 2:
                        handler = new EnumHandler();
                        break;
                    default:
                        return;
                }

                Console.WriteLine("\nEnter path!");
                var path = Console.ReadLine();

                if (string.IsNullOrEmpty(path))
                {
                    Console.WriteLine("\nPlease provide path!\n");
                    continue;
                }

                var fileContent = File.ReadAllLines(path);
                handler.Process(fileContent);

                var content = new MDPrinter().Print(handler);
                Console.WriteLine();
                Console.WriteLine(content);

                var savePath = Path.Join(config.OutputPath, $"{handler.Name}.md");
                File.WriteAllText(savePath, content);
                Console.WriteLine("\nSaved to {0}", savePath);

                Console.WriteLine("\nPress key to continue");
                Console.ReadKey();

                Console.Clear();
            }

        }

        internal abstract class Handler : IDataContainer
        {
            public string Description { get; protected set; }
            public string Name { get; protected set; }
            public List<StructElement> Properties { get; protected set; }

            public string Type => GetType();

            public string Namespace { get; set; }

            public abstract string GetType();

            public void Process(string[] fileContent)
            {
                Name = GetName(ref fileContent, GetType());
                Properties = GetProperties(ref fileContent);
            }

            protected string GetName(ref string[] content, string type)
            {
                string[] types = new[] { "public", "internal", "private" };
                string[] combos = types.Select(x => $"{x} {type}").ToArray();
                for (int i = 0; i < content.Length; i++)
                {
                    var item = content[i].TrimStart();
                    if (!combos.Any(item.StartsWith))
                        continue;

                    content = content.Skip(i + 2).ToArray();

                    var split = item.Split(type, 2);
                    return split[1].TrimStart().Split(':', 2)[0];
                }

                return string.Empty;
            }

            protected List<StructElement> GetProperties(ref string[] content)
            {
                List<StructElement> classProperties = new List<StructElement>();

                string summaryContent = string.Empty;
                for (int i = 0; i < content.Length; i++)
                {
                    var item = content[i].Trim();
                    if (string.IsNullOrEmpty(item))
                        continue;

                    if (item.StartsWith("/// <summary>"))
                    {
                        for (int j = i + 1; j < content.Length; j++)
                        {
                            var val = content[j].Trim();

                            if (val.StartsWith("/// </summary>"))
                            {
                                i = j;
                                break;
                            }

                            summaryContent += val.TrimStart('/');
                        }
                    }
                    else if (item.StartsWith('{'))
                    {
                        for (int j = i + 1; j < content.Length; j++)
                        {
                            if (!content[j].StartsWith("}"))
                                continue;

                            i = j;
                            break;
                        }
                    }
                    else if (item.StartsWith('}'))
                        continue;
                    else
                    {
                        var prop = GetElement(item, summaryContent);
                        classProperties.Add(prop);
                        summaryContent = string.Empty;
                    }
                }

                return classProperties;
            }

            protected abstract StructElement GetElement(string value, string summary);

        }


        internal class ClassHandler : Handler
        {
            protected override StructElement GetElement(string item, string summary)
            {
                var splits = item.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new StructElement(splits[2], splits[1], summary);
            }

            public override string GetType() => "class";
        }

        internal class EnumHandler : Handler
        {
            protected override StructElement GetElement(string item, string summary)
            {
                var splits = item.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new StructElement(splits[0], splits[2].TrimEnd(','), summary);
            }

            public override string GetType() => "enum";
        }
    }
}