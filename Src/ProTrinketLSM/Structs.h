// Structs.h

#ifndef _STRUCTS_h
#define _STRUCTS_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

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
	uint8_t lightsequence;
	uint8_t totalPixels;
	const char* effectedPixels; //Comma delimited
	const char* colors; //Comma delimited and are single digit numbers that have to be compared to get actual color
	uint32_t delayTime; //In milliseconds - will do as low as 200 milliseconds per second
	uint16_t duration; //In milliseconds
	uint8_t bounces;
	uint8_t iterations;
};

struct StripUpdateReturn {
	uint8_t effectNum;
	uint32_t currentSequence;
	unsigned long currentDuration;
	unsigned long performanceElapsedTime;
	unsigned long prevSeqTimesAccumulated;
	int8_t effectSuccess;
};

struct EffectsManagerUpdateReturn {
	unsigned long performanceElapsedTime;
	StripUpdateReturn* sURet;
};

#endif

