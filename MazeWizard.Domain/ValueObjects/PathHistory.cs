using MazeWizard.Domain.Enums;

namespace MazeWizard.Domain.ValueObjects;

[Obsolete("Only used by obsolete pathfinding logic.")]
public record PathHistory(BoundingBox BoundingBox, CardinalDirection Heading);