using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace PhotoLoader.Models
{
    public sealed class Photo
    {
        /// <summary>
        /// The friendly name for the photo
        /// </summary>
        public String DisplayName { get; set; }

        /// <summary>
        /// The name of the file
        /// </summary>
        public String FileName { get; set; }

        /// <summary>
        /// The full path (URL/Local File Path) for the photo
        /// </summary>
        public String FilePath { get; set; }

        /// <summary>
        /// Instant at which the photo file was created
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Instant at which the photo file was updated
        /// </summary>
        public DateTime DateUpdated { get; set; }

        /// <summary>
        /// Contains the location information of the photo if geo tagging was enabled
        /// </summary>
        public Geocoordinate Location { get; set; }
    }
}
