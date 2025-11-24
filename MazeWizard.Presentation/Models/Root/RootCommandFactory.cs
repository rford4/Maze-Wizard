using System.CommandLine;

namespace MazeWizard.Presentation.Models.Root;

internal static class RootCommandFactory
{
    public static RootCommand BuildRootCommand()
    {
        var output = new RootCommand()
        {
            Description = "Solves a maze image and outputs a solved version. The maze must follow "
                + "these pixel-color rules:\r\n\r\n"
                + "• Entrance:\tred pixels\t(RGB 255, 0, 0)\r\n"
                + "• Exit:\tblue pixels\t(RGB 0, 0, 255)\r\n"
                + "• Walls:\tblack pixels\t(RGB 0, 0, 0)\r\n"
                + "\nThe maze must also be fully surrounded by black walls.\r\n"
                + "\nThe solution to the maze, if found, will be painted in green."
        };

        output.Validators.Add(result =>
        {
            // HACK: https://github.com/dotnet/command-line-api/issues/1074
            if (result.Tokens.Count == 0)
            {
                result.AddError("Required argument 'source' missing for command: 'MazeWizard'.");
            }
            if (result.Tokens.Count < 2)
            {
                result.AddError("Required argument 'destination' missing for command: 'MazeWizard'.");
                return;
            }
                
            var overwriteOption = result.GetValue<bool>("--overwrite");
            if (overwriteOption == false)
            {
                // RF - Using result.GetValue to retrieve arg by name bypasses the arg's validation
                var destinationPath = result.Tokens[1].Value;
                if (File.Exists(destinationPath))
                    result.AddError("Destination file already exists. Use --overwrite to allow replacing it.");
            }
        });

        foreach (var argument in ArgumentFactory.BuildArguments())
            output.Arguments.Add(argument);

        foreach (var option in OptionFactory.BuildOptions())
            output.Options.Add(option);

        return output;
    }
}