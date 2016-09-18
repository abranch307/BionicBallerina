ESP8266HandleStructsSpike directory

This program tests the ESP8266's ability to receive structs via WiFI
and forward to microcontroller.

The request URI's are:

/add_struct
/remove_struct
/replace_struct
/list_structs

They accept these args
?PinSetup
	struct{
		uint16_t numpixels;
		uint8_t datapin;
		uint8_t clockpin;
		uint8_t dotstar_brg
	}PinSetup;
?ChangePixelColor
	struct{
		uint16 pixelIndex;
		uint32_t pixelColor (0xFF0000 = red, 0x00FF00 = green?, 0x0000FF = blue?)
	}PinSetup;

The values for the args should be a struct with the struct datatypes
in the same order as above

A test C++ program will be written to send the structs
to the ESP8266 and the ESP8266 will reverberate what
it received and send the structs down to the Pro Trinket
which will add to an array that it holds