using MazeWizard.Domain.Enums;
using MazeWizard.Domain.ValueObjects;

namespace MazeWizard.Domain.Entities;

[Obsolete("Used in obsolete pathfinding logic.")]
public class Breadcrumb
{
    public BoundingBox Position { get; }
    public CardinalDirection Heading { get; set; }
    public Stack<CardinalDirection> UnexploredDirections { get; set; } = [];

    public Breadcrumb(BoundingBox position, IEnumerable<CardinalDirection> unexploredDirections)
    {
        Position = position;

        foreach (var ud in unexploredDirections)
            UnexploredDirections.Push(ud);
    }
}