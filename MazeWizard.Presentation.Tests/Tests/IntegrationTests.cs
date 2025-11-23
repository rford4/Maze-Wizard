using MazeWizard.Presentation.Tests.Fixtures;

namespace MazeWizard.Presentation.Tests.Tests;

public class IntegrationTests : IntegrationTest
{
    private const string _testDataRelativePath = "TestData";

    [Fact]
    public async Task Missing_arguments_should_fail()
    {
        // Arrange
        var process = StartApplication(null);

        // Act
        var error1 = await process.StandardError.ReadLineAsync();
        var error2 = await process.StandardError.ReadLineAsync();
        var error3 = await process.StandardError.ReadLineAsync();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(error1));
        Assert.False(string.IsNullOrWhiteSpace(error2));
        Assert.True(string.IsNullOrWhiteSpace(error3));
    }

    [Theory]
    [InlineData("maze.bmp", "maze-solution.bmp")]
    [InlineData("maze.bmp", "maze-solution.png")]
    [InlineData("maze.bmp", "maze-solution.jpg")]
    [InlineData("maze.jpg", "maze-solution.jpg")]
    [InlineData("maze.jpg", "maze-solution.bmp")]
    [InlineData("maze.jpg", "maze-solution.png")]
    [InlineData("maze.png", "maze-solution.png")]
    [InlineData("maze.png", "maze-solution.bmp")]
    [InlineData("maze.png", "maze-solution.jpg")]
    [InlineData("maze-2.png", "maze-2-solution.jpg")]
    [InlineData("maze-3.png", "maze-3-solution.jpg")]
    [InlineData("maze-4.png", "maze-4-solution.jpg")]
    public async Task Happy_path_should_succeed(string inputFileName, string outputFileName)
    {
        // Arrange
        var testFilePath = Path.Combine(AppContext.BaseDirectory, _testDataRelativePath, inputFileName);
        var outputFilePath = Path.Combine(AppContext.BaseDirectory, _testDataRelativePath, outputFileName);
        var overwriteOption = "--overwrite";
        var process = StartApplication($"{testFilePath} {outputFilePath} {overwriteOption}");

        // Act
        var errorOutput = await process.StandardError.ReadLineAsync();

        // Assert
        Assert.True(string.IsNullOrWhiteSpace(errorOutput));
    }

    [Fact]
    public async Task Invalid_file_type_should_fail()
    {
        // Arrange
        var testFilePath = Path.Combine(AppContext.BaseDirectory, _testDataRelativePath, "maze.tif");
        var outputFilePath = Path.Combine(AppContext.BaseDirectory, _testDataRelativePath, "test_output.png");
        var process = StartApplication($"{testFilePath} {outputFilePath}");

        // Act
        var errorOutput = await process.StandardError.ReadLineAsync();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(errorOutput));
        Assert.True(File.Exists(testFilePath));
    }

    [Fact]
    public async Task Existing_destination_without_overwrite_should_fail()
    {
        // Arrange
        var testFilePath = Path.Combine(AppContext.BaseDirectory, _testDataRelativePath, "maze.png");
        var outputFilePath = Path.Combine(AppContext.BaseDirectory, _testDataRelativePath, "output.png");
        var process = StartApplication($"{testFilePath} {outputFilePath}");

        // Act
        var errorOutput = await process.StandardError.ReadLineAsync();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(errorOutput));
    }

    [Fact]
    public async Task Existing_destination_with_overwrite_should_succeed()
    {
        // Arrange
        var testFilePath = Path.Combine(AppContext.BaseDirectory, _testDataRelativePath, "maze.png");
        var outputFilePath = Path.Combine(AppContext.BaseDirectory, _testDataRelativePath, "output.png");
        var overwriteOption = "--overwrite";
        var process = StartApplication($"{testFilePath} {outputFilePath} {overwriteOption}");

        // Act
        var errorOutput = await process.StandardError.ReadLineAsync();

        // Assert
        Assert.True(string.IsNullOrWhiteSpace(errorOutput));
    }

    [Fact]
    public async Task Non_existant_destination_directory_should_fail()
    {
        // Arrange
        var testFilePath = Path.Combine(AppContext.BaseDirectory, _testDataRelativePath, "maze.png");
        var outputFilePath = Path.Combine(AppContext.BaseDirectory, Guid.NewGuid().ToString(), "output.png");
        var process = StartApplication($"{testFilePath} {outputFilePath}");

        // Act
        var errorOutput = await process.StandardError.ReadLineAsync();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(errorOutput));
    }
}