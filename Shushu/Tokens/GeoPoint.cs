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
        /// Initializes a new instance of the <see cref="T:Shushu.Tokens.GeoPoint"/> class.
        /// </summary>
        public GeoPoint()
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Shushu.Tokens.GeoPoint"/> class.
        /// </summary>
        /// <param name="long">Long.</param>
        /// <param name="lat">Lat.</param>
        public GeoPoint(double @long, double lat)
        {
            Coordinates = new[] { @long, lat };            
        }        
    }
}
