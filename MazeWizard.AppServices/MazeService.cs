using MazeWizard.Domain.Entities;
using MazeWizard.Domain.Enums;
using MazeWizard.Domain.ValueObjects;
using MazeWizard.DomainServices;
using MazeWizard.Utilities;
using System.Drawing;

namespace MazeWizard.AppServices;

/// <summary>
/// Provides functionality to solve maze images and generate a visual solution.
/// </summary>
/// <remarks>
/// This service currently depends on the Windows GDI+ <see cref="Bitmap"/> implementation. 
/// </remarks>
public class MazeService()
{
    /// <summary>
    /// Solves a rectangular maze with orthogonal cells from an input image file and writes the solution to an output image file.
    /// </summary>
    /// <param name="sourceFilePath">The file path of the source maze image to solve.</param>
    /// <param name="destinationFilePath">The file path where the solved maze image will be saved.</param>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    /// <item><description><c>success</c>: <see langword="true"/> if the maze was solved successfully; otherwise <see langword="false"/>.</description></item>
    /// <item><description><c>errors</c>: A collection of error messages describing why the maze could not be solved, if any.</description></item>
    /// </list>
    /// </returns>
    public static (bool success, IEnumerable<string> errors) SolveRectangularMaze(string sourceFilePath, string destinationFilePath)
    {
        // TODO: This creates a dependency on the Windows runtime environment
        // Consider replacing with cross plaform library like ImageSharp: https://sixlabors.com/
        using var bitmap = new Bitmap(sourceFilePath);

        var mazePixels = ParseMazePixels(bitmap);

        var maze = new Maze(mazePixels);

        if (!maze.IsValid)
        {
            var errors = maze.ValidationErrors;
            return (false, errors);
        }

        var exitSequence = maze.GetSolution();
        if(exitSequence.Count == 0)
        {
            return (false, ["Unable to find a solution for the input file"]);
        }

        var pixelsToPaint = ConvertExitSequenceToPaintableArea(exitSequence);

        MazeRenderer.PaintSolution(bitmap, pixelsToPaint, Color.Green);

        bitmap.Save(destinationFilePath);

        return (true, []);
    }

    [Obsolete($"Use {nameof(SolveRectangularMaze)} instead.")]
    private void SolveWithPathfinder(string sourceFilePath, string destinationFilePath)
    {
        using var bitmap = new Bitmap(sourceFilePath);

        var mazePixels = ParseMazePixels(bitmap);

        var maze = new Maze(mazePixels);

        var pathfinder = new Pathfinder(
            initialPosition: new(
                maze.Entrance.MinX,
                maze.Entrance.MinX + Math.Min(maze.Entrance.XRange, maze.Entrance.YRange),
                maze.Entrance.MinY,
                maze.Entrance.MinY + Math.Min(maze.Entrance.XRange, maze.Entrance.YRange)));

        var mazeTraversalService = new MazeTraversalService();

        mazeTraversalService.Solve(maze, pathfinder);

        var traversedPath = pathfinder.TraversedPath;
        var pixelsToPaint = ConvertTraversedPathToPaintableArea(traversedPath);

        MazeRenderer.PaintSolution(bitmap, pixelsToPaint, Color.Green);

        bitmap.Save(destinationFilePath);
    }

    private static List<Point> ConvertExitSequenceToPaintableArea(Stack<BoundingBox> exitSequence)
    {
        var output = new List<Point>();

        foreach (var boundingBox in exitSequence)
        {
            for (var x = boundingBox.MinX; x <= boundingBox.MaxX; x++)
            {
                for (var y = boundingBox.MinY; y <= boundingBox.MaxY; y++)
                {
                    output.Add(new Point(x, y));
                }
            }
        }

        return output;
    }

    [Obsolete]
    private static List<Point> ConvertTraversedPathToPaintableArea(Stack<PathHistory> traversedPath)
    {
        var output = new List<Point>();

        while(traversedPath.Count > 0)
        {
            var pathStep = traversedPath.Pop();

            switch(pathStep.Heading)
            {
                case CardinalDirection.North:
                    for(var i = pathStep.BoundingBox.MinX; i <= pathStep.BoundingBox.MaxX; i++)
                        output.Add(new(i, pathStep.BoundingBox.MinY));
                    break;
                case CardinalDirection.East:
                    for (var i = pathStep.BoundingBox.MinY; i <= pathStep.BoundingBox.MaxY; i++)
                        output.Add(new(pathStep.BoundingBox.MaxX, i));
                    break;
                case CardinalDirection.South:
                    for (var i = pathStep.BoundingBox.MinX; i <= pathStep.BoundingBox.MaxX; i++)
                        output.Add(new(i, pathStep.BoundingBox.MaxY));
                    break;
                case CardinalDirection.West:
                    for (var i = pathStep.BoundingBox.MinY; i <= pathStep.BoundingBox.MaxY; i++)
                        output.Add(new(pathStep.BoundingBox.MinX, i));
                    break;
            }
        }

        return output;
    }

    private static MazePixel[,] ParseMazePixels(Bitmap bitmap)
    {
        var output = new MazePixel[bitmap.Width, bitmap.Height];

        for(var x = 0; x < bitmap.Width; x++)
            for(var y = 0; y < bitmap.Height; y++)
                output[x, y] = new(bitmap.GetPixel(x, y).ToArgb());

        return output;
    }
}