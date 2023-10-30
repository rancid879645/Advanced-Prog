﻿// See https://aka.ms/new-console-template for more information

using FileFinder;
Console.WriteLine("Root path:");
var rootPath = Console.ReadLine();
if (rootPath is not (null or ""))
{
    Console.WriteLine("write the name of the file/directory to search:");
    var directoryName = Console.ReadLine();
    
    Func<string, bool> filter = (path) => path.ToUpper().Contains(directoryName.ToUpper());
    var fileVisitor = new FileSystemVisitor(rootPath, filter);

    fileVisitor.SearchStarted += (sender, e) => Console.WriteLine("The search Begins (⌐■_■)");
    fileVisitor.SearchFinished += (sender, e) => Console.WriteLine("The search finished └(^o^ )Ｘ( ^o^)┘");
    fileVisitor.FileFound += (sender, e) =>
    {
        if (e.Name.Contains("stop.txt"))
            e.IsAbortSearch=true;
    };
    
    Console.WriteLine("Result:");
    Console.WriteLine("-------------------------------------");
    fileVisitor.OnSearchStarted();
    fileVisitor.PrintTree(fileVisitor.Traverse());
    fileVisitor.OnSearchFinished();
    Console.WriteLine("-------------------------------------");

}