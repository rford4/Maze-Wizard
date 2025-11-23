using MazeWizard.Domain.Enums;

namespace MazeWizard.Domain.Extensions;

[Obsolete("CardnialDirection enum is obsolete.")]
public static class CardinalDirectionExtensions
{
    public static CardinalDirection Opposite(this CardinalDirection value)
    {
        return value switch
        {
            CardinalDirection.North => CardinalDirection.South,
            CardinalDirection.East => CardinalDirection.West,
            CardinalDirection.South => CardinalDirection.North,
            CardinalDirection.West => CardinalDirection.East,
            _ => throw new ArgumentOutOfRangeException(nameof(value)),
        };
    }
}