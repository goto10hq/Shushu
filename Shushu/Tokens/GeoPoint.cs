namespace Shushu.Tokens
{
    /// <summary>
    /// Geo point.
    /// </summary>
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
        
        public GeoPoint()
        {            
        }

        public GeoPoint(double @long, double lat)
        {
            Coordinates = new[] { @long, lat };            
        }        
    }
}
