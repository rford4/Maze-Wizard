using MazeWizard.Domain.ValueObjects;
using System.Drawing;

namespace MazeWizard.Domain.Tests.Factories;

internal static class MazeFactory
{
    public static MazePixel[,] CreateBlankMaze()
    {
        var width = 10;
        var height = 10;

        var output = new MazePixel[width, height];

        return output;
    }

    public static MazePixel[,] CreateUnsolvableMaze()
    {
        var output = GetBaseMaze();

        // Entrance
        output[1, 1] = new(Color.Red.ToArgb());
        output[2, 1] = new(Color.Red.ToArgb());
        output[1, 2] = new(Color.Red.ToArgb());
        output[2, 2] = new(Color.Red.ToArgb());

        // Exit
        output[8, 1] = new(Color.Blue.ToArgb());
        output[8, 2] = new(Color.Blue.ToArgb());
        output[7, 1] = new(Color.Blue.ToArgb());
        output[7, 2] = new(Color.Blue.ToArgb());

        // Walls
        output[3, 1] = new(Color.Black.ToArgb());
        output[3, 2] = new(Color.Black.ToArgb());
        output[3, 3] = new(Color.Black.ToArgb());
        output[3, 4] = new(Color.Black.ToArgb());
        output[3, 5] = new(Color.Black.ToArgb());
        output[3, 6] = new(Color.Black.ToArgb());
        output[4, 6] = new(Color.Black.ToArgb());
        output[5, 6] = new(Color.Black.ToArgb());
        output[5, 7] = new(Color.Black.ToArgb());
        output[5, 8] = new(Color.Black.ToArgb());
        output[6, 6] = new(Color.Black.ToArgb());
        output[6, 5] = new(Color.Black.ToArgb());
        output[6, 4] = new(Color.Black.ToArgb());
        output[6, 3] = new(Color.Black.ToArgb());
        output[6, 2] = new(Color.Black.ToArgb());
        output[6, 1] = new(Color.Black.ToArgb());

        return output;
    }
    
    public static MazePixel[,] CreateMazeWithPerimeterPorts()
    {
        var output = GetBaseMaze();

        // Entrance
        output[1, 1] = new(Color.Red.ToArgb());
        output[2, 1] = new(Color.Red.ToArgb());
        output[1, 2] = new(Color.Red.ToArgb());
        output[2, 2] = new(Color.Red.ToArgb());

        // Exit
        output[4, 1] = new(Color.Blue.ToArgb());
        output[4, 2] = new(Color.Blue.ToArgb());
        output[5, 1] = new(Color.Blue.ToArgb());
        output[5, 2] = new(Color.Blue.ToArgb());

        // Walls
        output[3, 1] = new(Color.Black.ToArgb());
        output[3, 2] = new(Color.Black.ToArgb());
        output[3, 3] = new(Color.Black.ToArgb());
        output[3, 4] = new(Color.Black.ToArgb());
        output[3, 5] = new(Color.Black.ToArgb());
        output[3, 6] = new(Color.Black.ToArgb());
        output[4, 3] = new(Color.Black.ToArgb());
        output[4, 4] = new(Color.Black.ToArgb());
        output[5, 3] = new(Color.Black.ToArgb());
        output[5, 4] = new(Color.Black.ToArgb());
        output[6, 3] = new(Color.Black.ToArgb());
        output[6, 4] = new(Color.Black.ToArgb());
        output[6, 5] = new(Color.Black.ToArgb());
        output[6, 6] = new(Color.Black.ToArgb());

        return output;
    }

    public static MazePixel[,] CreateMazeWithOnePixelPaths()
    {
        var output = GetBaseMaze();

        // Entrance
        output[1, 1] = new(Color.Red.ToArgb());

        // Exit
        output[8, 1] = new(Color.Blue.ToArgb());

        // Walls
        output[2, 1] = new(Color.Black.ToArgb());
        output[2, 2] = new(Color.Black.ToArgb());
        output[2, 3] = new(Color.Black.ToArgb());
        output[2, 4] = new(Color.Black.ToArgb());
        output[2, 5] = new(Color.Black.ToArgb());
        output[2, 6] = new(Color.Black.ToArgb());
        output[2, 7] = new(Color.Black.ToArgb());
        output[3, 7] = new(Color.Black.ToArgb());
        output[4, 7] = new(Color.Black.ToArgb());
        output[5, 7] = new(Color.Black.ToArgb());
        output[6, 7] = new(Color.Black.ToArgb());
        output[7, 7] = new(Color.Black.ToArgb());
        output[7, 6] = new(Color.Black.ToArgb());
        output[7, 5] = new(Color.Black.ToArgb());
        output[7, 4] = new(Color.Black.ToArgb());
        output[7, 3] = new(Color.Black.ToArgb());
        output[7, 2] = new(Color.Black.ToArgb());
        output[7, 1] = new(Color.Black.ToArgb());

        return output;
    }

    private static MazePixel[,] GetBaseMaze(int width = 10, int height = 10)
    {
        var output = new MazePixel[width, height];

        // Perimiter
        for (int i = 0; i < width; i++)
        {
            output[i, 0] = new(Color.Black.ToArgb());
            output[i, height - 1] = new(Color.Black.ToArgb());
        }

        for (int i = 0; i < height; i++)
        {
            output[0, i] = new(Color.Black.ToArgb());
            output[width - 1, i] = new(Color.Black.ToArgb());
        }

        return output;
    }
}