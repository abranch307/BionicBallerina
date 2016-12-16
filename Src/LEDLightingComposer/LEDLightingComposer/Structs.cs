/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created: 
	Last Modified:    
	Class: Structs.cs
	Class Description:
		This class holds different struct elements needed by other classes 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEDLightingComposer
{
    public static class Structs
    {
        /*
            Lighting Sequence Struct
            DelayTime is milliseconds

            72 bits + arbitrary char* variables
        */
        public struct LightingSequence
        {
            public int lightsequence { get; set; }
            public ushort totalPixels { get; set; }
            public String[] effectedPixels { get; set; } //Comma delimited
            public String[] colors { get; set; } //Comma delimited and are single digit numbers that have to be compared to get actual color
            public UInt32 delayTime { get; set; } //In milliseconds - will do as low as 200 milliseconds per second
            public UInt16 duration { get; set; } //In milliseconds
            public ushort bounces { get; set; }
            public ushort iterations { get; set; }
            public Int16 brightness { get; set; }
            public Int16 incrBrightness { get; set; }
            public Int32 brightnessDelayTime { get; set; }
        };

        struct StripInfo
        {
            public UInt16 NumPixels { get; set; }
            public byte Datapin { get; set; }
            public byte ClockPin { get; set; }
            public byte RGB { get; set; }
        };


    }
}
