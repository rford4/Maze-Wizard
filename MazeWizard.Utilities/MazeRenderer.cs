using System.Drawing;

namespace MazeWizard.Utilities;

/// <summary>
/// Provides utility methods for rendering mazes and their solutions onto images.
/// </summary>
/// /// <remarks>
/// This service currently depends on the Windows GDI+ <see cref="Bitmap"/> implementation. 
/// </remarks>
public static class MazeRenderer
{
    /// <summary>
    /// Paints a given path on the provided bitmap using the specified color.
    /// </summary>
    /// <param name="source">The <see cref="Bitmap"/> on which to paint the solution.</param>
    /// <param name="path">A list of <see cref="Point"/> representing the pixels to paint as the maze solution.</param>
    /// <param name="color">The <see cref="Color"/> to use when painting the path.</param>
    public static void PaintSolution(Bitmap source, List<Point> path, Color color)
    {
        // TODO: This creates a dependency on the Windows runtime environment
        // Consider replacing with cross plaform library like ImageSharp: https://sixlabors.com/
        foreach (var p in path)
            source.SetPixel(p.X, p.Y, color);
    }
}