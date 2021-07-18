using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFPixelCanvas.Canvas.Models.Boids3D;

namespace WPFPixelCanvas.Canvas.Models.Woims
{
    /// <summary>
    ///  Defines a 'woim'. It can move around in 3D. It has length.
    ///  Each segment in woim is visualized with a sprite.
    /// </summary>
    public class Woim3D
    {

        //## Private fields
        private int _growthRate { get; set; }
        private Random _randomSource { get; set; }
        private int _size { get; set; }              //Defines number of segments ( or how large the woim is )

        private long _refreshCount;


        //## Constructor(s)
        public Woim3D(Random randomSource, int size, int growthRate)
        {
            //Store parameters
            _refreshCount = 0;                          // How many times has the update call been made
            _randomSource = randomSource;               // Source for random data

            _growthRate = growthRate;                   // How long should woim wait between adding each new body segment

            //Create the head
            Head = new WoimHeadSegment(randomSource);
            _size = size;                                // Number of segments

            //Add head to list of segments
            Segments = new List<WoimSegment> { Head };
        }


        //## Public  interface
        public void update()
        {
            //Determine if woim at max-segments
            if(Segments.Count < _size && _refreshCount % _growthRate == 0)
            {
                //Determine parameters for new tail segment
                WoimSegment positionSource = Tail ?? (WoimSegment)Head;            // Use the last available segment as source for position of new element 
                Vector3D segmentPosition = new Vector3D(positionSource.Position);
                Vector3D segmentAcceleration = positionSource.Acceleration;
                Vector3D segmentVelocity = positionSource.Velocity;

                // Create the new segment and set it as tail
                Tail = new WoimBodySegment(_randomSource, segmentPosition, segmentVelocity, segmentAcceleration);

                //Add new segment to the woim segment list
                Segments.Add(Tail);

            }

            //Update all segments
            foreach(var segment in Segments)
            

            _refreshCount++;

        }

        //## Public properties
        public List<WoimSegment> Segments { get; set; }  // Holds an object for each segment in the woim ( including the head and tail )
        public WoimHeadSegment Head { get; set; }
        public WoimBodySegment Tail{ get; set; }

    }
}
