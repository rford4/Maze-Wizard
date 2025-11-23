namespace MazeWizard.Domain.ValueObjects;

/// <summary>
/// Represents a rectangular area defined by minimum and maximum X and Y coordinates.
/// </summary>
/// <remarks>
/// Provides methods to shift, intersect, split, and compare bounding boxes.
/// Useful for working with spatial regions, pixel areas, or grids.
/// </remarks>
public readonly struct BoundingBox : IEquatable<BoundingBox>
{
    #region Properties

    /// <summary>
    /// Gets the minimum X-coordinate of the bounding box.
    /// </summary>
    public int MinX { get; }

    /// <summary>
    /// Gets the maximum X-coordinate of the bounding box.
    /// </summary>
    public int MaxX { get; }

    /// <summary>
    /// Gets the minimum Y-coordinate of the bounding box.
    /// </summary>
    public int MinY { get; }

    /// <summary>
    /// Gets the maximum Y-coordinate of the bounding box.
    /// </summary>
    public int MaxY { get; }

    /// <summary>
    /// Gets the width of the bounding box (difference between <see cref="MaxX"/> and <see cref="MinX"/>).
    /// </summary>
    public int XRange => MaxX - MinX;

    /// <summary>
    /// Gets the height of the bounding box (difference between <see cref="MaxY"/> and <see cref="MinY"/>).
    /// </summary>
    public int YRange => MaxY - MinY;

    #endregion

    #region ctor

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundingBox"/> struct with specified coordinates.
    /// </summary>
    /// <param name="minX">The minimum X-coordinate (must be &gt;= 0 and &lt;= <paramref name="maxX"/>).</param>
    /// <param name="maxX">The maximum X-coordinate (must be &gt;= <paramref name="minX"/>).</param>
    /// <param name="minY">The minimum Y-coordinate (must be &gt;= 0 and &lt;= <paramref name="maxY"/>).</param>
    /// <param name="maxY">The maximum Y-coordinate (must be &gt;= <paramref name="minY"/>).</param>
    /// <exception cref="ArgumentException">Thrown if any argument violates the above constraints.</exception>
    public BoundingBox(int minX, int maxX, int minY, int maxY)
    {
        if (minX < 0)
            throw new ArgumentException($"Value must be >= 0", nameof(minX));
        if (minY < 0)
            throw new ArgumentException($"Value must be >= 0", nameof(minY));
        if (minX > maxX)
            throw new ArgumentException($"Value must be >= {nameof(minX)}", nameof(maxX));
        if (minY > maxY)
            throw new ArgumentException($"Value must be >= {nameof(minY)}", nameof(maxY));

        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Returns a new <see cref="BoundingBox"/> shifted by the specified amounts.
    /// </summary>
    /// <param name="xShift">Amount to shift along the X-axis.</param>
    /// <param name="yShift">Amount to shift along the Y-axis.</param>
    /// <returns>A new <see cref="BoundingBox"/> with shifted coordinates.</returns>
    public BoundingBox Shift(int xShift = 0, int yShift = 0)
    {
        return new BoundingBox(MinX + xShift, MaxX + xShift, MinY + yShift, MaxY + yShift);
    }

    /// <summary>
    /// Computes the intersection of this bounding box with another.
    /// </summary>
    /// <param name="other">The other bounding box to intersect with.</param>
    /// <returns>
    /// A <see cref="BoundingBox"/> representing the overlapping area, or <see langword="null"/> if there is no overlap.
    /// </returns>
    public BoundingBox? Intersect(BoundingBox other)
    {
        int minX = Math.Max(MinX, other.MinX);
        int maxX = Math.Min(MaxX, other.MaxX);
        int minY = Math.Max(MinY, other.MinY);
        int maxY = Math.Min(MaxY, other.MaxY);

        if (minX > maxX || minY > maxY)
            return null;

        return new BoundingBox(minX, maxX, minY, maxY);
    }

    /// <summary>
    /// Splits this bounding box using a delimiter bounding box along one axis.
    /// </summary>
    /// <param name="delimter">A bounding box that shares either the same X or Y bounds.</param>
    /// <returns>A list of bounding boxes representing the remaining areas after removing the delimiter.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the delimiter does not share the same X or Y bounds as this bounding box.
    /// </exception>
    public List<BoundingBox> Split(BoundingBox delimter)
    {
        var output = new List<BoundingBox>();

        if(delimter.MinX == MinX && delimter.MaxX == MaxX)
        {
            if (MaxY > delimter.MaxY)
                output.Add(new(MinX, MaxX, delimter.MaxY + 1, MaxY));

            if (MinY < delimter.MinY)
                output.Add(new(MinX, MaxX, MinY, delimter.MinY - 1));

            return output;
        }
        else if(delimter.MinY == MinY && delimter.MaxY == MaxY)
        {
            if (MaxX > delimter.MaxX)
                output.Add(new(delimter.MaxX + 1, MaxX, MinY, MaxY));

            if (MinX < delimter.MinX)
                output.Add(new(MinX, delimter.MinX - 1, MinY, MaxY));

            return output;
        }

        throw new ArgumentException("Must have same x or y bounds to split.", nameof(delimter));
    }

    /// <summary>
    /// Determines whether this bounding box intersects with another.
    /// </summary>
    /// <param name="other">The other bounding box to check for intersection.</param>
    /// <returns><see langword="true"/> if the bounding boxes overlap; otherwise, <see langword="false"/>.</returns>
    public bool Intersects(in BoundingBox other)
    {
        return !(other.MinX > MaxX ||
                 other.MaxX < MinX ||
                 other.MinY > MaxY ||
                 other.MaxY < MinY);
    }

    #endregion

    #region IEquatable

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is BoundingBox other && this.Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => (MinX, MaxX, MinY, MaxY).GetHashCode();

    public bool Equals(BoundingBox bb)
    {
        return MinX == bb.MinX &&
            MinY == bb.MinY &&
            MaxX == bb.MaxX &&
            MaxY == bb.MaxY;
    }

    public static bool operator ==(BoundingBox lhs, BoundingBox rhs) => lhs.Equals(rhs);

    public static bool operator !=(BoundingBox lhs, BoundingBox rhs) => !(lhs == rhs);

    #endregion
}