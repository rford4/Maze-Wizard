using MazeWizard.Domain.Enums;
using System.Drawing;

namespace MazeWizard.Domain.ValueObjects;

/// <summary>
/// Represents a single pixel in a maze image, including its ARGB color value and the corresponding maze feature.
/// </summary>
public readonly struct MazePixel
{
    private readonly int EntranceArgbColor = Color.FromArgb(255, 0, 0).ToArgb();
    private readonly int ExitArgbColor = Color.FromArgb(0, 0, 255).ToArgb();
    private readonly int WallArgbColor = Color.FromArgb(0, 0, 0).ToArgb();

    /// <summary>
    /// Gets the ARGB color value of the pixel.
    /// </summary>
    public int ArgbValue { get; }

    /// <summary>
    /// Gets the maze feature represented by this pixel.
    /// </summary>
    public MazeFeature Feature { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MazePixel"/> struct based on an ARGB color value.
    /// </summary>
    /// <param name="aRGB">The ARGB color value of the pixel.</param>
    /// <remarks>
    /// Determines the <see cref="Feature"/> of the pixel based on predefined colors:
    /// <list type="bullet">
    /// <item><description>Red (255,0,0) = Entrance</description></item>
    /// <item><description>Blue (0,0,255) = Exit</description></item>
    /// <item><description>Black (0,0,0) = Wall</description></item>
    /// <item><description>Any other color = Path</description></item>
    /// </list>
    /// </remarks>
    public MazePixel(int aRGB)
    {
        ArgbValue = aRGB;
        Feature = MazeFeatureFromArgb(aRGB);
    }

    private MazeFeature MazeFeatureFromArgb(int aRGB)
    {
        if (aRGB == EntranceArgbColor)
            return MazeFeature.Entrance;
        
        if (aRGB == ExitArgbColor)
            return MazeFeature.Exit;

        if (aRGB == WallArgbColor)
            return MazeFeature.Wall;

        return MazeFeature.Path;
    }
}