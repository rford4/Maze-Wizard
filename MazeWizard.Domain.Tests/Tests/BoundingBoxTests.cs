using MazeWizard.Domain.ValueObjects;

namespace MazeWizard.Domain.Tests.Tests;

[Trait("Category", "Unit")]
public class BoundingBoxTests
{
    [Fact]
    public void Bounding_boxes_with_same_dimensions_are_equal()
    {
        // Arrange
        var box1 = new BoundingBox(1, 10, 1, 10);
        var box2 = new BoundingBox(1, 10, 1, 10);

        // Act
        var areEqual = box1 == box2;

        // Assert
        Assert.True(areEqual);
    }

    [Fact]
    public void Overlaping_boxes_should_intersect()
    {
        // Arrange
        var box1 = new BoundingBox(1, 2, 1, 2);
        var box2 = new BoundingBox(2, 3, 2, 3);

        // Act
        var result = box1.Intersects(box2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Intersecting_overlaping_boxes_should_produce_result()
    {
        // Arrange
        var box1 = new BoundingBox(1, 2, 1, 2);
        var box2 = new BoundingBox(2, 3, 2, 3);
        var expectedResult = new BoundingBox(2, 2, 2, 2);

        // Act
        var result = box1.Intersect(box2);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Spliting_box_should_produce_two_boxes()
    {
        // Arrange


        // Act


        // Assert

    }
}