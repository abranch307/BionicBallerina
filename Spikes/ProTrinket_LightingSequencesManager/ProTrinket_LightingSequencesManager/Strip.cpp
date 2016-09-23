// 
// 
// 

#include "Strip.h"

Strip::Strip(uint16_t NumPixels, uint8_t Datapin, uint8_t ClockPin, uint8_t RGB = DOTSTAR_RGB) {
	strip = Adafruit_DotStar(NumPixels, Datapin, ClockPin, RGB);
}
