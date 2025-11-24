using MazeWizard.Domain.Enums;
using MazeWizard.Domain.ValueObjects;
using System.Drawing;

namespace MazeWizard.Domain.Entities;

/// <summary>
/// Represents a maze composed of a 2D grid of <see cref="MazePixel"/> values,
/// including its entrance, exit, navigable path segments, and logic for
/// validating and solving the maze.
/// </summary>
public sealed class Maze
{
    #region Member Variables

    private static readonly Dictionary<CornerType, (int dx, int dy)[]> CornerOffsets = new()
    {
        { CornerType.BottomLeft,  new[] { ( 0, +1), (-1, 0), (-1, +1) } },
        { CornerType.BottomRight, new[] { ( 0, +1), (+1, 0), (+1, +1) } },
        { CornerType.TopRight,    new[] { ( 0, -1), (+1, 0), (+1, -1) } },
        { CornerType.TopLeft,     new[] { ( 0, -1), (-1, 0), (-1, -1) } }
    };

    private readonly MazePixel[,] _pixels;

    private bool? _hasSolution;
    private bool? _isValid;
    private readonly List<string> _errors = [];

    private BoundingBox? _entrance;
    private BoundingBox? _entrancePath;
    private BoundingBox? _exitPath;
    private BoundingBox? _exit;

    private readonly HashSet<BoundingBox> _paths = [];
    private Stack<BoundingBox> _solution = [];

    #endregion

    #region Public

    /// <summary>
    /// Gets the bounding box that represents the maze's entrance region.
    /// </summary>
    /// <remarks>
    /// If no entrance was detected during maze initialization, this returns
    /// the default <see cref="BoundingBox"/> value.
    /// </remarks>
    public BoundingBox Entrance => _entrance.GetValueOrDefault();

    /// <summary>
    /// Gets the bounding box that represents the maze's exit region.
    /// </summary>
    /// <remarks>
    /// If no exit was detected during maze initialization, this returns
    /// the default <see cref="BoundingBox"/> value.
    /// </remarks>
    public BoundingBox Exit => _exit.GetValueOrDefault();

    /// <summary>
    /// Gets the height of the maze image, in pixels.
    /// </summary>
    public int Height => _pixels.GetLength(1);

    /// <summary>
    /// Gets the width of the maze image, in pixels.
    /// </summary>
    public int Width => _pixels.GetLength(0);

    /// <summary>
    /// Gets a value indicating whether the maze is structurally valid.
    /// </summary>
    /// <remarks>
    /// A maze is considered valid if it contains a detectable entrance,
    /// a detectable exit, and has a perimeter composed entirely of walls.
    /// Whether or not the maze has a solution is not considered.
    /// </remarks>
    public bool IsValid => _isValid ?? false;

    /// <summary>
    /// Gets a collection of validation errors discovered while analyzing the maze.
    /// </summary>
    /// <remarks>
    /// If the maze is valid, the collection is empty.
    /// Otherwise, it contains messages describing each validation failure.
    /// </remarks>
    public IEnumerable<string> ValidationErrors => _errors.AsEnumerable();

    /// <summary>
    /// Initializes a new instance of the <see cref="Maze"/> class using
    /// a pre-parsed pixel grid.
    /// </summary>
    /// <param name="mazePixels">
    /// A 2D array of <see cref="MazePixel"/> values representing the maze.
    /// </param>
    /// <remarks>
    /// The constructor identifies the maze's entrance and exit and performs
    /// validation on the maze's structure. A solution path is not computed
    /// until <see cref="GetSolution"/> is called.
    /// </remarks>
    public Maze(MazePixel[,] mazePixels)
    {
        _pixels = mazePixels;

        SetEntranceAndExit();

        Validate();

        if (_isValid == false)
            return;

        BuildPaths();
    }

    /// <summary>
    /// Computes and returns the sequence of path segments that make up
    /// a valid solution from the maze entrance to the exit.
    /// </summary>
    /// <returns>
    /// A stack containing the ordered <see cref="BoundingBox"/> segments
    /// that represent the solution path. If no solution exists, returns an
    /// empty stack.
    /// </returns>
    public Stack<BoundingBox> GetSolution()
    {
        if(_hasSolution == null)
        {
            SolveMaze();
        }

        return _solution;
    }

    /// <summary>
    /// Determines whether the specified bounding region contains one or more
    /// pixels of the given maze feature.
    /// </summary>
    /// <param name="range">The bounding box defining the area of the maze to inspect.</param>
    /// <param name="feature">The maze feature to search for.</param>
    /// <returns>
    /// <c>true</c> if any pixel in the specified region matches the feature;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool RangeHasFeature(BoundingBox range, MazeFeature feature)
    {
        for(var i = range.MinX; i <= range.MaxX; i++)
        {
            for(var j = range.MinY; j <= range.MaxY; j++)
            {
                if (_pixels[i, j].Feature == feature)
                    return true;
            }
        }

        return false;
    }

    #endregion

    #region Implementation Details

    private void Validate()
    {
        // RF - Mazes are immutable, don't revalidate if already performed
        if (_isValid != null)
            return;

        if (_entrance == null)
            _errors.Add("The maze entrance could not be identified.");

        if (_exit == null)
            _errors.Add("The maze exit could not be identified");

        if (IsPerimeterValid() == false)
            _errors.Add("The maze perimieter can only contain wall features.");

        _isValid = _exit != null && _entrance != null;
    }

    private bool IsPerimeterValid()
    {
        for (var x = 0; x < _pixels.GetLength(0); x++)
        {
            if (_pixels[x, 0].Feature != MazeFeature.Wall)
                return false;
            if (_pixels[x, _pixels.GetLength(1) - 1].Feature != MazeFeature.Wall)
                return false;
        }
            
        for (var y = 0; y < _pixels.GetLength(1); y++)
        {
            if (_pixels[_pixels.GetLength(0) - 1, y].Feature != MazeFeature.Wall)
                return false;
            if (_pixels[0, y].Feature != MazeFeature.Wall)
                return false;
        }

        return true;
    }

    private void SolveMaze()
    {
        if(_entrancePath == null || _exitPath == null)
        {
            _hasSolution = false;
            return;
        }

        var pathsToExit = FindExit(_entrancePath.GetValueOrDefault(), []);
        if(pathsToExit == null)
        {
            _hasSolution = false;
            return;
        }

        // RF - Trims out the branches of a forked path that do not lead to the exit
        var pathSegmentsToExit = TrimUntraversedPathSegments(pathsToExit);

        // RF - Trims out the entrance and exit boxes; so they aren't painted over
        pathSegmentsToExit = TrimEntranceAndExit(pathSegmentsToExit);

        _solution = pathSegmentsToExit;
    }

    /// <summary>
    /// Finds exit by recursively checking the entrance path's intersecting paths and in turn their 
    /// intersecting paths. Exit is found if one of the intersecting paths is the exit.
    /// </summary>
    private Stack<BoundingBox>? FindExit(BoundingBox currentPath, HashSet<BoundingBox> traversedPaths)
    {
        if (currentPath == _exitPath)
        {
            var output = new Stack<BoundingBox>();
            output.Push(currentPath);
            return output;
        }

        traversedPaths.Add(currentPath);

        var intersectingPaths = GetIntersectingPaths(currentPath);
        if (intersectingPaths.Count == 0)
            return null;

        foreach (var intersectingPath in intersectingPaths)
        {
            if (traversedPaths.Contains(intersectingPath))
                continue;

            var exitSequence = FindExit(intersectingPath, traversedPaths);

            if (exitSequence is not null)
            {
                exitSequence.Push(currentPath);
                return exitSequence;
            }
        }

        return null;
    }

    private Stack<BoundingBox> TrimUntraversedPathSegments(Stack<BoundingBox> exitPathSequence)
    {
        var output = new Stack<BoundingBox>();

        while (exitPathSequence.Count > 0)
        {
            // No intersections remain, pop and push to output
            if (exitPathSequence.Count == 1)
            {
                output.Push(exitPathSequence.Pop());
                continue;
            }

            var linePreA = output.Count == 0 ? _entrance.GetValueOrDefault() : output.Peek();
            var lineA = exitPathSequence.Pop();
            var lineB = exitPathSequence.Pop();
            var linePostB = exitPathSequence.Count == 0 ? _exit.GetValueOrDefault() : exitPathSequence.Peek();

            var intersection = lineA.Intersect(lineB).GetValueOrDefault();

            var lineASegments = lineA.Split(intersection);
            var lineBSegments = lineB.Split(intersection);

            foreach (var segment in lineASegments)
            {
                if (segment.Intersect(linePreA) != null)
                {
                    output.Push(new(
                        minX: Math.Min(segment.MinX, intersection.MinX),
                        maxX: Math.Max(segment.MaxX, intersection.MaxX),
                        minY: Math.Min(segment.MinY, intersection.MinY),
                        maxY: Math.Max(segment.MaxY, intersection.MaxY)));
                    break;
                }
            }

            foreach (var segment in lineBSegments)
            {
                if (segment.Intersect(linePostB) != null)
                {
                    exitPathSequence.Push(new(
                        minX: Math.Min(segment.MinX, intersection.MinX),
                        maxX: Math.Max(segment.MaxX, intersection.MaxX),
                        minY: Math.Min(segment.MinY, intersection.MinY),
                        maxY: Math.Max(segment.MaxY, intersection.MaxY)));
                    break;
                }
            }
        }

        return output;
    }

    private Stack<BoundingBox> TrimEntranceAndExit(Stack<BoundingBox> exitPathSegments)
    {
        var output = new Stack<BoundingBox>();
        var exitSegment = exitPathSegments.Pop();
        output.Push(exitSegment.Split(_exit.GetValueOrDefault()).First());

        while (exitPathSegments.Count > 0)
        {
            var pathLine = exitPathSegments.Pop();

            if (exitPathSegments.Count == 0)
                output.Push(pathLine.Split(_entrance.GetValueOrDefault()).First());
            else
                output.Push(pathLine);
        }

        return output;
    }

    private List<BoundingBox> GetIntersectingPaths(BoundingBox path)
    {
        var output = new List<BoundingBox>();

        foreach (var pathToComapre in _paths)
        {
            if (pathToComapre == path)
                continue;

            if (path.Intersects(pathToComapre))
                output.Add(pathToComapre);
        }

        return output;
    }

    private void SetEntranceAndExit()
    {
        // RF - Search inner permimter first; most mazes conform to this design and it's a quicker search; else search entire maze
        ScanPermiterForEntranceAndExit();

        if(_entrance == null || _exit == null)
            ScanMazeForEntranceAndExit();
    }

    private void BuildPaths()
    {
        for (var x = 1; x < _pixels.GetLength(0) - 1; x++)
        {
            for (var y = 1; y < _pixels.GetLength(1) - 1; y++)
            {
                var point = new Point(x, y);

                var cornerType = IsCorner(point);

                if (cornerType == null)
                    continue;

                var paths = BuildPathSegmentsFromCorner(point, cornerType.GetValueOrDefault());

                foreach (var path in paths)
                {
                    if (_entrance != null && path.Intersects(_entrance.GetValueOrDefault()))
                        _entrancePath = path;

                    if (_exit != null && path.Intersects(_exit.GetValueOrDefault()))
                        _exitPath = path;

                    _paths.Add(path);
                }       
            }
        }
    }

    private CornerType? IsCorner(Point point)
    {
        if (_pixels[point.X, point.Y].Feature == MazeFeature.Wall)
            return null;

        foreach (var kv in CornerOffsets)
        {
            if (IsCornerType(point, kv.Key))
                return kv.Key;
        }

        return null;
    }

    private bool IsCornerType(Point point, CornerType type)
    {
        foreach (var (dx, dy) in CornerOffsets[type])
        {
            int nx = point.X + dx;
            int ny = point.Y + dy;

            if (nx < 0 || ny < 0 || nx >= _pixels.GetLength(0) || ny >= _pixels.GetLength(1))
                return false;

            if (_pixels[nx, ny].Feature != MazeFeature.Wall)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Determines the boundaries of the two possible paths that can be derived from a corner
    /// </summary>
    private List<BoundingBox> BuildPathSegmentsFromCorner(Point cornerPoint, CornerType cornerType)
    {
        var output = new List<BoundingBox>();

        var xIncrement = cornerType == CornerType.TopLeft || cornerType == CornerType.BottomLeft ? 1 : -1;
        var yIncrement = cornerType == CornerType.TopLeft || cornerType == CornerType.TopRight ? 1 : -1;

        Point basePoint = new(cornerPoint.X, cornerPoint.Y);

        //TODO: Think of more efficient way to see if segment already in hash set before reading 

        #region Width-based path

        var x = basePoint.X;
        var y = basePoint.Y;

        // RF - Traverse x-axis of wall until another wall is reached; this is maximum width of path
        for(; x >= 0 && x <= _pixels.GetLength(0) - 1; x += xIncrement)
        {
            if (_pixels[x, y].Feature == MazeFeature.Wall)
            {
                x -= xIncrement;
                break;
            }
        }

        y += yIncrement;

        bool maxYFound = false;

        // RF - Find height of path by incrementing y and traversing the x-path until a wall is hit; this is the height of the path.
        for(; y >= 0 && y <= _pixels.GetLength(1) - 1; y += yIncrement)
        {
            for (int i = basePoint.X; xIncrement == 1 ? i <= x : i >= x; i += xIncrement)
            {
                if (_pixels[i, y].Feature == MazeFeature.Wall)
                {
                    maxYFound = true;
                    y -= yIncrement;
                    break;
                }
            }

            if (maxYFound)
                break;
        }

        switch (cornerType)
        {
            case CornerType.BottomLeft:
                output.Add(new BoundingBox(minX: basePoint.X, maxX: x, minY: y, maxY: basePoint.Y));
                break;
            case CornerType.BottomRight:
                output.Add(new BoundingBox(minX: x, maxX: basePoint.X, minY: y, maxY: basePoint.Y));
                break;
            case CornerType.TopRight:
                output.Add(new BoundingBox(minX: x, maxX: basePoint.X, minY: basePoint.Y, maxY: y));
                break;
            case CornerType.TopLeft:
                output.Add(new BoundingBox(minX: basePoint.X, maxX: x, minY: basePoint.Y, maxY: y));
                break;
            default:
                throw new NotImplementedException();
        }

        #endregion

        #region Height-based path

        x = basePoint.X;
        y = basePoint.Y;

        for(; y >= 0 && y <= _pixels.GetLength(1) - 1; y += yIncrement)
        {
            if (_pixels[x, y].Feature == MazeFeature.Wall)
            {
                y -= yIncrement;
                break;
            }
        }

        x += xIncrement;

        bool maxXFound = false;
        for(; x <= _pixels.GetLength(0) - 1; x+= xIncrement)
        {
            for (int i = basePoint.Y; yIncrement == 1 ? i <= y : i >= y; i += yIncrement)
            {
                if (_pixels[x, i].Feature == MazeFeature.Wall)
                {
                    maxXFound = true;
                    x -= xIncrement;
                    break;
                }
            }

            if (maxXFound)
                break;
        }

        switch (cornerType)
        {
            case CornerType.BottomLeft:
                output.Add(new BoundingBox(minX: basePoint.X, maxX: x, minY: y, maxY: basePoint.Y));
                break;
            case CornerType.BottomRight:
                output.Add(new BoundingBox(minX: x, maxX: basePoint.X, minY: y, maxY: basePoint.Y));
                break;
            case CornerType.TopRight:
                output.Add(new BoundingBox(minX: x, maxX: basePoint.X, minY: basePoint.Y, maxY: y));
                break;
            case CornerType.TopLeft:
                output.Add(new BoundingBox(minX: basePoint.X, maxX: x, minY: basePoint.Y, maxY: y));
                break;
            default:
                throw new NotImplementedException();
        }

        #endregion

        return output;
    }

    private void ScanPermiterForEntranceAndExit()
    {
        // RF - As per rules, the perimeter of the image must be a wall; skip edge pixels
        var xStart = 1;
        var yStart = 1;

        // RF - Assume the entrance/exit is larger than 1 pixel; increment iterator by more than one pixel to even out search speeds
        var xIncrement = Convert.ToInt32(Math.Floor(_pixels.GetLength(0) * 0.05));
        var yIncrement = Convert.ToInt32(Math.Floor(_pixels.GetLength(1) * 0.05));

        xIncrement = xIncrement == 0 ? 1 : xIncrement;
        yIncrement = yIncrement == 0 ? 1 : yIncrement;

        do
        {
            var x = xStart;

            if (xStart <= xIncrement)
            {
                for (; x < _pixels.GetLength(0); x += xIncrement)
                {
                    ScanForEntranceOrExit(new Point(x, 1));
                    ScanForEntranceOrExit(new Point(x, _pixels.GetLength(1) - 2));

                    if (_entrance != null && _exit != null)
                        break;
                }
            }

            if (_entrance != null && _exit != null)
                break;

            var y = yStart;

            if (yStart <= yIncrement)
            {
                for (; y < _pixels.GetLength(1); y += yIncrement)
                {
                    ScanForEntranceOrExit(new Point(1, y));
                    ScanForEntranceOrExit(new Point(_pixels.GetLength(0) - 2, y));

                    if (_entrance != null && _exit != null)
                        break;
                }
            }

            xStart++;
            yStart++;
        } while ((xStart <= xIncrement || yStart <= yIncrement) && (_entrance == null || _exit == null));
    }

    private void ScanMazeForEntranceAndExit()
    {
        for(var x = 1; x < _pixels.GetLength(0) - 1; x++)
        {
            for(var y = 1; y < _pixels.GetLength(1) - 1; y++)
            {
                ScanForEntranceOrExit(new(x, y));

                if (_entrance != null && _exit != null)
                    return;
            }
        }
    }

    private void ScanForEntranceOrExit(Point samplePoint)
    {
        var pixelFeature = _pixels[samplePoint.X, samplePoint.Y].Feature;

        if (pixelFeature == MazeFeature.Entrance && _entrance == null)
            _entrance = SurveyFeature(samplePoint, MazeFeature.Entrance);

        else if (pixelFeature == MazeFeature.Exit && _exit == null)
            _exit = SurveyFeature(samplePoint, MazeFeature.Exit);
    }

    private BoundingBox SurveyFeature(Point samplePoint, MazeFeature feature)
    {
        int minX;
        int maxX;
        int minY;
        int maxY;
        int x = samplePoint.X;
        int y = samplePoint.Y;

        do
        {
            x--;
        } while (_pixels[x, y].Feature == feature);

        minX = ++x;

        do
        {
            y--;
        } while (_pixels[x, y].Feature == feature);

        minY = ++y;

        do
        {
            x++;
        } while (_pixels[x, y].Feature == feature);

        maxX = --x;

        do
        {
            y++;
        } while (_pixels[x,y].Feature == feature);

        maxY = --y;

        return new BoundingBox(minX, maxX, minY, maxY);
    }

    #endregion
}