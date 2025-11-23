namespace MazeWizard.DomainServices;

using MazeWizard.Domain.Entities;
using MazeWizard.Domain.Enums;

[Obsolete("Abandoned the traversal methodology.")]
public sealed class MazeTraversalService
{
    public void Solve(Maze maze, Pathfinder pathfinder)
    {
        while (true)
        {
            MakeNextMove(maze, pathfinder);

            if (maze.RangeHasFeature(pathfinder.CurrentPosition, MazeFeature.Exit))
                break;
        }
    }

    private void MakeNextMove(Maze maze, Pathfinder pathfinder)
    {
        CardinalDirection? traversalOption = null;
       
        var traversalOptions = CalculateTraversalOptions(maze, pathfinder);

        var currentPosition = pathfinder.CurrentPosition;

        if(traversalOptions.Count == 1)
        {
            traversalOption = traversalOptions.First();
        }

        // RF - If more than one traversal option, pathfinder is at a fork; store breadcrumb and continue down first available travel direction
        else if (traversalOptions.Count > 1)
        {
            traversalOption = traversalOptions.First();

            var unexploredPaths = traversalOptions.Where(x => x != traversalOption);

            pathfinder.AddBreadcrumb(unexploredPaths);
        }

        // RF - If no traversal options remain, return to last breadcrumb with available traversal option
        else if(traversalOptions.Count == 0)
        {
            var breadCrumb = pathfinder.ReturnToPreviousBreadcrumb();

            while(breadCrumb?.UnexploredDirections?.Count == 0)
            {
                breadCrumb = pathfinder.ReturnToPreviousBreadcrumb();
            }

            if(breadCrumb != null)
            {
                traversalOption = breadCrumb.UnexploredDirections.Pop();
                pathfinder.AddBreadcrumb(breadCrumb.UnexploredDirections);
            }

            var currentPosition3 = pathfinder.CurrentPosition;
        }

        if (traversalOption == null)
        {
            var currentPosition2 = pathfinder.CurrentPosition;
            throw new Exception("Maze is not solvable");
        }
            

        pathfinder.Move(traversalOption.Value);
    }

    private HashSet<CardinalDirection> CalculateTraversalOptions(Maze maze, Pathfinder pathfinder)
    {
        var output = new HashSet<CardinalDirection>();
        var currentPositon = pathfinder.CurrentPosition;

        // RF - Check current heading to prevent going backwards as an option
        if (pathfinder.Heading != CardinalDirection.South && currentPositon.MinY > 0)
        {
            var northPosition = currentPositon.Shift(yShift: -1);

            // RF - Check if doubling back by comparing position to breadcrumb position
            if (!maze.RangeHasFeature(northPosition, MazeFeature.Wall) && !pathfinder.IsBreadcrumbPosition(northPosition)
                && pathfinder.CanMove(CardinalDirection.North))
            {
                output.Add(CardinalDirection.North);
            }
        }
            
        if(pathfinder.Heading != CardinalDirection.West && currentPositon.MaxX < maze.Width - 1)
        {
            var eastPosition = currentPositon.Shift(xShift: 1);

            if (!maze.RangeHasFeature(eastPosition, MazeFeature.Wall) && !pathfinder.IsBreadcrumbPosition(eastPosition)
                && pathfinder.CanMove(CardinalDirection.East))
            {
                output.Add(CardinalDirection.East);
            }
        }

        if (pathfinder.Heading != CardinalDirection.North && currentPositon.MaxY < maze.Height - 1)
        {
            var southPosition = currentPositon.Shift(yShift: 1);

            if (!maze.RangeHasFeature(southPosition, MazeFeature.Wall) && !pathfinder.IsBreadcrumbPosition(southPosition)
                && pathfinder.CanMove(CardinalDirection.South))
            {
                output.Add(CardinalDirection.South);
            }
        }

        if (pathfinder.Heading != CardinalDirection.East && currentPositon.MinX > 0)
        {
            var westPosition = currentPositon.Shift(xShift: -1);

            if (!maze.RangeHasFeature(westPosition, MazeFeature.Wall) && !pathfinder.IsBreadcrumbPosition(westPosition)
                && pathfinder.CanMove(CardinalDirection.West))
            {
                output.Add(CardinalDirection.West);
            }      
        }
           
        return output;
    }
}