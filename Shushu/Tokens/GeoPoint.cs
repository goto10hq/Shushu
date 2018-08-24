namespace Shushu.Tokens
{
    /// <summary>
    /// Geo point.
    /// </summary>
    /// <remarks>
    /// Since GeographyPoint can not be serialized we use this.
    /// Since this one can.
    /// </remarks>
    public class GeoPoint
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type => "Point";

        /// <summary>
        /// Gets or sets the coordinates.
        /// </summary>
        /// <value>The coordinates.</value>
        public double[] Coordinates { get; set; }

        /// <summary>
        /// Ctor.
        /// </summary>
        public GeoPoint()
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="lat">Lat.</param>
        /// /// <param name="long">Long.</param>
        public GeoPoint(double lat, double @long)
        {
            Coordinates = new[] { lat, @long };
        }
    }
}