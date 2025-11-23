using System.CommandLine;

namespace MazeWizard.Presentation.Models.Root;

internal static class ArgumentFactory
{
    private static readonly HashSet<string> _approvedFileExtensions = ["bmp", "jpg", "png"];

    public static List<Argument> BuildArguments()
    {
        var output = new List<Argument>
        {
            BuildSourceArgument(),
            BuildDestinationArgument()
        };

        return output;
    }

    private static Argument<FileInfo> BuildSourceArgument()
    {
        var output = new Argument<FileInfo>("source")
        {
            // HACK: https://github.com/dotnet/command-line-api/issues/1074
            Arity = ArgumentArity.ZeroOrOne,
            Description = $"The maze image to solve. Supported file types: [{string.Join(", ", _approvedFileExtensions)}]."
        };

        output.Validators.Add(result =>
        {
            var sourceFile = result.GetValueOrDefault<FileInfo>();
            if (sourceFile == null)
            {
                result.AddError("Argument 'source': invalid file path value.");
                return;
            }

            if (!File.Exists(sourceFile.FullName))
            {
                result.AddError("Argument 'source': file does not exist.");
            }

            if (!_approvedFileExtensions.Contains(sourceFile.Extension.TrimStart('.')))
            {
                result.AddError("Argument 'source': invalid file type.");
            }
        });

        return output;
    }

    private static Argument<FileInfo> BuildDestinationArgument()
    {
        var output = new Argument<FileInfo>("destination")
        {
            // HACK: https://github.com/dotnet/command-line-api/issues/1074
            Arity = ArgumentArity.ZeroOrOne,
            Description = $"The ouput image to write. Supported file types: [{string.Join(", ", _approvedFileExtensions)}]."
        };

        output.Validators.Add(result =>
        {
            var sourceFile = result.GetValueOrDefault<FileInfo>();
            if(sourceFile == null)
            {
                result.AddError("Unable to parse file path for argument 'destination'.");
                return;
            }

            if (!_approvedFileExtensions.Contains(sourceFile.Extension.TrimStart('.')))
            {
                result.AddError("Invalid file type for argument 'destination'.");
            }

            if (!Directory.Exists(sourceFile.DirectoryName))
            {
                result.AddError("The specified directory for argument 'destination' does not exist.");
            }
        });

        return output;
    }
}