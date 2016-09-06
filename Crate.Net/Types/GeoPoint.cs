using System;

namespace Crate.Net.Client.Types
{
    /// <summary>
    /// Crate geo_point type
    /// </summary>
    public class GeoPoint
    {

        public double Longitude { get; set; }
        public double Latitude { get; set; }
        
        public GeoPoint(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public GeoPoint(double[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (values.Length < 2)
                throw new ArgumentException("Values array must contain at least two values", nameof(values));

            Longitude = values[0];
            Latitude = values[1];
        }

        public override string ToString()
        {
            return $"[{Longitude}, {Latitude}]";
        }
    }
}
