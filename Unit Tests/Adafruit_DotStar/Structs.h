// Structs.h

#ifndef _STRUCTS_h
#define _STRUCTS_h

/*#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#endif*/

/*
*/
struct StripInfo {
	uint16_t NumPixels;
	uint8_t Datapin;
	uint8_t ClockPin;
	uint8_t RGB;
};

/*
*/
struct CommaDelimitReturn {
	int value;
	int nextBegPosition;
};

/*
Lighting Sequence Struct
DelayTime is milliseconds

72 bits + arbitrary char* variables
*/
struct LightingSequence {
	uint8_t lightsequence;
	uint8_t totalPixels;
	//const char* effectedPixels; //Comma delimited
	const char* colors; //Comma delimited and are single digit numbers that have to be compared to get actual color
	uint32_t delayTime; //In milliseconds - will do as low as 200 milliseconds per second
	uint16_t duration; //In milliseconds
	uint8_t bounces;
	uint8_t iterations;
	int16_t brightness;
	int16_t incrBrightness;
	uint32_t brightnessDelayTime;
};

//struct StripUpdateReturn {
//	int8_t effectNum;
//	int32_t currentSequence;
//	long currentDuration;
//	long performanceElapsedTime;
//	long prevSeqTimesAccumulated;
//	int8_t effectSuccess;
//};
//
struct EffectsManagerUpdateReturn {
	unsigned long performanceElapsedTime;
};

#endif

