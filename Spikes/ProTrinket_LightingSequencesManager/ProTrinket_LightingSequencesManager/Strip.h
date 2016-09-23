// Strip.h

#ifndef _STRIP_h
#define _STRIP_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#include <Adafruit_DotStar.h>

/*
*/
struct StripInfo {
	uint16_t NumPixels;
	uint8_t Datapin;
	uint8_t ClockPin;
	uint8_t RGB;
};

/*
	Lighting Sequence Struct
	DelayTime is milliseconds

	72 bits + arbitrary char* variables
*/
struct LightingSequence {
	uint8_t totalPixels;
	char* effectedPixels;
	char* colors;
	uint32_t delayTime;
	uint16_t duration;
	uint8_t bounces;
	uint8_t iterations;
};

class Strip {

	public:
		Strip(void);
		Strip(uint16_t NumPixels, uint8_t Datapin, uint8_t ClockPin, uint8_t RGB, LightingSequence* initSeq);
	private:
		Adafruit_DotStar strip;
		LightingSequence* lseqs;
	protected:

};

#endif

