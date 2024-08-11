using DescriptionGenerator.Core;

Console.WriteLine("Enter how you want to generate MD!\n1: by text\n2: by assembly\n3: exit");
var type = Console.ReadLine();
if (!int.TryParse(type, out int elementType))
{
    Console.WriteLine("\nPlease provide valid type!\n");
    return;
}

switch (elementType)
{
    case 1:
       var textGenerator = new MDGeneratorByText();
        textGenerator.Start();
        break;
    case 2:
        var assembly = new MDGeneratorByAssembly();
        assembly.Start();
        return;
    default:
        return;
}