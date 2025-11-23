using MazeWizard.Domain.Entities;
using MazeWizard.Domain.Enums;
using MazeWizard.Domain.Tests.Factories;
using MazeWizard.Domain.ValueObjects;

namespace MazeWizard.Domain.Tests.Tests;

[Trait("Category", "Unit")]
public class MazeTests
{
    [Fact]
    public void Constructor_sets_entrance_and_exit()
    {
        // Arrange
        var mazePixels = MazeFactory.CreateMazeWithPerimeterPorts();

        // Act
        var sut = new Maze(mazePixels);

        // Assert
        Assert.NotEqual(default, sut.Entrance);
        Assert.NotEqual(default, sut.Exit);
    }

    [Fact]
    public void Valid_maze_is_valid()
    {
        // Arrange
        var mazePixels = MazeFactory.CreateMazeWithPerimeterPorts();

        // Act
        var sut = new Maze(mazePixels);

        // Assert
        Assert.True(sut.IsValid);
        Assert.Empty(sut.ValidationErrors);
    }

    [Fact]
    public void Invalid_maze_is_invalid()
    {
        // Arrange
        var mazePixels = MazeFactory.CreateBlankMaze();

        // Act
        var sut = new Maze(mazePixels);

        // Assert
        Assert.False(sut.IsValid);
        Assert.Equal(3, sut.ValidationErrors.Count());
    }

    [Fact]
    public void Empty_maze_has_no_solution()
    {
        // Arrange
        var mazePixels = MazeFactory.CreateBlankMaze();

        // Act
        var sut = new Maze(mazePixels);
        var solution = sut.GetSolution();

        // Assert
        Assert.Empty(solution);
    }

    [Fact]
    public void Unsolvable_has_no_solution()
    {
        // Arrange
        var mazePixels = MazeFactory.CreateUnsolvableMaze();

        // Act
        var sut = new Maze(mazePixels);
        var solution = sut.GetSolution();

        // Assert
        Assert.Empty(solution);
    }

    [Fact]
    public void Maze_with_one_pixel_paths_is_solvable()
    {
        // Arrange
        var mazePixels = MazeFactory.CreateMazeWithOnePixelPaths();

        // Act
        var sut = new Maze(mazePixels);
        var solution = sut.GetSolution();

        // Assert
        Assert.NotEmpty(solution);
    }

    [Theory]
    [InlineData(MazeFeature.Entrance)]
    [InlineData(MazeFeature.Exit)]
    [InlineData(MazeFeature.Wall)]
    public void Can_identify_range_with_feature(MazeFeature feature)
    {
        // Arrange
        var mazePixels = MazeFactory.CreateMazeWithPerimeterPorts();

        // Act
        var sut = new Maze(mazePixels);
        var result = sut.RangeHasFeature(new(0, mazePixels.GetLength(0) - 1, 0, mazePixels.GetLength(1) - 1), feature);

        // Assert
        Assert.True(result);
    }
}