using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.Boids3D;

namespace WPFPixelCanvas.Canvas.Models.Blobs
{
    // A blob is defined by position, base color and strength.
    public class Blob
    {
        //## Private fields

        //## Constructor(s)
        public Blob()
        {
            //Default values 
            Position = new Vector3D(0, 0, 0);               // Upper left corner
            BaseColor = new Vector3D(128.0, 255.0, 128.0);  // Light green
            Strength = 1.0;                                 // 
        }

        //## Public interface

        //## Public properties
        public Vector3D Position { get; set; }      // Where is blob-point located
        public Vector3D BaseColor { get; set; }    // What color does it contribute with
        public double Strength { get; set; }         // How strongly does it contribute

        //## Private helpers


    }
}
