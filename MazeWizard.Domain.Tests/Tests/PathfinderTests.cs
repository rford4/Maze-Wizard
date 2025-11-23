using MazeWizard.Domain.Entities;
using MazeWizard.Domain.Enums;
using MazeWizard.Domain.ValueObjects;

namespace MazeWizard.Domain.Tests.Tests;

[Trait("Category", "Unit")]
[Obsolete]
public class PathfinderTests
{
    [Fact]
    public void Constructor_sets_position()
    {
        // Arrange
        var position = new BoundingBox(0, 10, 0, 10);

        // Act
        var sut = new Pathfinder(position);

        // Assert
        Assert.Equal(position, sut.CurrentPosition);
    }

    [Fact]
    public void New_breadcrumb_is_previous_breadcrumb()
    {
        // Arrange
        var position = new BoundingBox(0, 10, 0, 10);
        var previousBreadcrumbPosition = new BoundingBox(0, 10, 2, 12);

        // Act
        var sut = new Pathfinder(position);

        sut.Move(CardinalDirection.South);
        sut.AddBreadcrumb([CardinalDirection.East]);
        
        sut.Move(CardinalDirection.South);
        sut.AddBreadcrumb([CardinalDirection.West]);
        
        sut.Move(CardinalDirection.South);
        
        var lastBreadcrumb = sut.ReturnToPreviousBreadcrumb();

        // Assert
        Assert.NotNull(lastBreadcrumb);
        Assert.Equal(previousBreadcrumbPosition, lastBreadcrumb.Position);
        Assert.Equal([CardinalDirection.West], lastBreadcrumb.UnexploredDirections);
    }

    [Fact]
    public void Previous_position_stored_on_move()
    {
        // Arrange
        var position = new BoundingBox(0, 10, 0, 10);

        // Act
        var sut = new Pathfinder(position);
        sut.Move(CardinalDirection.South);

        var previousPosition = sut.PreviousPosition;

        // Assert
        Assert.NotNull(previousPosition);
        Assert.Equal(position, previousPosition.BoundingBox);
    }

    [Fact]
    public void Heading_set_on_move()
    {
        // Arrange
        var position = new BoundingBox(0, 10, 0, 10);

        // Act
        var sut = new Pathfinder(position);
        sut.Move(CardinalDirection.East);

        // Assert
        Assert.NotNull(sut.Heading);
        Assert.Equal(CardinalDirection.East, sut.Heading);
    }
}