using MazeWizard.AppServices;
using MazeWizard.Presentation.Models.Root;

namespace MazeWizard.Presentation;

internal class Application()
{
    public async Task Run(string[] args)
    {
        var parseResult = RootCommandFactory.BuildRootCommand()
            .Parse(args);

        await parseResult.InvokeAsync();

        if (parseResult.Errors.Count > 0)
            return;

        var sourceFile = parseResult.GetValue<FileInfo>("source");
        var destinationFile = parseResult.GetValue<FileInfo>("destination");

        if (sourceFile == null || destinationFile == null)
            return;

        (var success, var errors) = MazeService.SolveRectangularMaze(sourceFile.FullName, destinationFile.FullName);

        if(success == false)
            WriteErrors(errors);
    }

    private static void WriteErrors(IEnumerable<string> errors)
    {
        Console.WriteLine("Unable to solve maze: ");

        foreach (var error in errors)
            Console.WriteLine(error);
    }
}