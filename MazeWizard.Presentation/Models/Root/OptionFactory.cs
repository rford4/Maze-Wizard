using System.CommandLine;

namespace MazeWizard.Presentation.Models.Root;

internal static class OptionFactory
{
    public static List<Option> BuildOptions()
    {
        var output = new List<Option>
        {
            BuildOverwriteOption()
        };

        return output;
    }

    private static Option<bool> BuildOverwriteOption()
    {
        var output = new Option<bool>("--overwrite")
        {
            Description = "Allow overwriting the destination file if it already exists."
        };

        output.Aliases.Add("--o");

        return output;
    }
}