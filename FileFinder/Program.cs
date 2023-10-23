// See https://aka.ms/new-console-template for more information

using FileFinder;
Console.WriteLine("Root path:");
var rootPath = Console.ReadLine();
if (rootPath is not (null or ""))
{
    var fileVisitor = new FileSystemVisitor(rootPath);

    fileVisitor.SearchStarted += (sender, e) => Console.WriteLine(" ******The search Begins (⌐■_■) \n");
    fileVisitor.SearchFinished += (sender, e) => Console.WriteLine("\n ******The search finished └(^o^ )Ｘ( ^o^)┘");

    Console.WriteLine("-------------------------------------");
    fileVisitor.PrintTree(fileVisitor.GetFullTree());
    Console.WriteLine("-------------------------------------");
    Console.WriteLine("Search:");
    var fileName = Console.ReadLine();
    if (fileName != null)
    {
        Console.WriteLine("-------------------------------------");
        Console.WriteLine("Result: \n"); 
        fileVisitor.PrintTree(fileVisitor.FindFile(fileName));
        Console.WriteLine("-------------------------------------");
    }
}