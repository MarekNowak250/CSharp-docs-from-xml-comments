using DescriptionGenerator.Core;

namespace DescriptionGenerator
{
    public class MDGeneratorByAssembly
    {
        public void Start()
        {
            Console.WriteLine("\nEnter path!");
            var path = Console.ReadLine();

            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("\nPlease provide path!\n");
                return;
            }

            var ass = new AssemblyReader(path).Read();

            Console.WriteLine("\nProvide method full name (namespace.name)");
            var methodName = Console.ReadLine();
            if (string.IsNullOrEmpty(methodName))
            {
                Console.WriteLine("\nPlease provide method full name (namespace.name)!\n");
                return;
            }
            var methodus = new AssemblyProcessor(ass);
            var classes = methodus.ProcessClass(methodName);

            foreach (var clazz in classes)
            {
                Console.WriteLine("\n-------------------------------------------------------------\n");
                Console.WriteLine(new MDPrinter().Print(clazz));
            }


        }
    }
}