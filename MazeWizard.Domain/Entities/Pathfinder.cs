using MazeWizard.Domain.Enums;
using MazeWizard.Domain.ValueObjects;

namespace MazeWizard.Domain.Entities;

[Obsolete("Used in obsolete traversal strategy")]
public class Pathfinder(BoundingBox initialPosition)
{
    private readonly Stack<Breadcrumb> _breadcrumbs = [];
    private readonly Stack<PathHistory> _pathHistory = [];
    private readonly Stack<PathHistory> _allPathHistory = [];

    private BoundingBox _currentPosition = initialPosition;
    private CardinalDirection? _heading;

    public PathHistory? PreviousPosition => _pathHistory.LastOrDefault();
    public BoundingBox CurrentPosition => _currentPosition;
    public CardinalDirection? Heading => _heading;
    public Stack<PathHistory> TraversedPath => new(_pathHistory.Reverse());


    public void AddBreadcrumb(IEnumerable<CardinalDirection> traversalOptions)
    {
        _breadcrumbs.Push(new(_currentPosition, traversalOptions));
    }

    public bool CanMove(CardinalDirection direction)
    {
        if (_pathHistory.Count == 0)
            return true;

        var previousPosition = _pathHistory.Peek();

        // RF - Logic to prevent double-back
        return direction switch
        {
            CardinalDirection.North => previousPosition.BoundingBox.MinY != _currentPosition.MinY - 1,
            CardinalDirection.East => previousPosition.BoundingBox.MaxX != _currentPosition.MaxX + 1,
            CardinalDirection.South => previousPosition.BoundingBox.MaxY != _currentPosition.MaxY + 1,
            CardinalDirection.West => previousPosition.BoundingBox.MinX != _currentPosition.MinX - 1,
            _ => false,
        };
    }

    public void Move(CardinalDirection direction)
    {
        var xShift = 0;
        var yShift = 0;

        switch(direction)
        {
            case CardinalDirection.North:
                yShift = -1;
                break;
            case CardinalDirection.East:
                xShift = 1;
                break;
            case CardinalDirection.South:
                yShift = 1;
                break;
            case CardinalDirection.West:
                xShift = -1;
                break;
            default:
                break;
        }

        if (xShift == 0 && yShift == 0)
            return;

        _heading = direction;

        _pathHistory.Push(new(CurrentPosition, direction));
        _allPathHistory.Push(new(CurrentPosition, direction));

        _currentPosition = new(
            _currentPosition.MinX + xShift,
            _currentPosition.MaxX + xShift,
            _currentPosition.MinY + yShift,
            _currentPosition.MaxY + yShift);
    }

    public Breadcrumb? ReturnToPreviousBreadcrumb()
    {
        if (_breadcrumbs.Count == 0)
            return null;

        var breadcrumb = _breadcrumbs.Pop();

        while(_pathHistory.Count > 0)
        {
            var poppedHistory =_pathHistory.Pop();
            if (poppedHistory.BoundingBox == breadcrumb.Position)
                break;
        }

        _currentPosition = breadcrumb.Position;
        _heading = breadcrumb.Heading;

        return breadcrumb;
    }

    public bool IsBreadcrumbPosition(BoundingBox position)
    {
        return _allPathHistory.Any(x => x.BoundingBox == position);
    }
}