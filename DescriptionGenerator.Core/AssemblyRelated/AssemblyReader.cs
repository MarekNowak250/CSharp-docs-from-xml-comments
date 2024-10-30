using System.Reflection;

namespace DescriptionGenerator.Core.AssemblyRelated
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
}
