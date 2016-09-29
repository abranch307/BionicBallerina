// Strip.h

#ifndef _STRIP_h
#define _STRIP_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#ifndef _STRUCTS_h
	#include "Structs.h"
#endif

#ifndef _EFFECTS_h
	#include <Effects.h>
#endif

#ifndef _ADAFRUIT_DOT_STAR_H_
	#include <Adafruit_DotStar\Adafruit_DotStar.h>
#endif

class Strip {

	public:
		Strip(void);
		~Strip();
		Strip(uint16_t NumPixels, uint8_t Datapin, uint8_t ClockPin, uint8_t RGB, LightingSequence* initSeq, uint16_t numSeqs);
		void Update(unsigned long currentPerformanceTime);
		uint16_t getCurrentSeq();
		uint16_t getCountSeqs();
		unsigned long getCurrentDuration();
		unsigned long getPrevDuration();
		unsigned long getPrevSeqTimesAccumulated();
		uint8_t getEffectNum();
		bool resetGlobalVars();
		bool resetPerformance();
		bool findCurrentSeqFromPerformanceTime(unsigned long performanceTime);
		LightingSequence* getCurrentLightingSequence();

		StripUpdateReturn* stripUpdateRet; //Holds information to return from Update method
	private:
		Adafruit_DotStar strip;
		LightingSequence* lseqs = NULL;
		uint16_t currentSequence = 0, countSeqs = 0;
		bool proceed = false;
		unsigned long currentDuration = 0, // so elapsedtime - this = time within this sequence
			prevDuration = 0, //Makes sure duration isn't processed more than once for sequence
			prevSeqTimesAccumulated = 0; //As sequences change, the duration times are accumulated to here (used to find current effect's duration)
		uint16_t counter1 = 0, counter2 = 0, i = -1, j = -1, p0 = -1, p1 = 0, p2 = 1, p3 = 2, p4 = 3, p5 = 4;
	protected:

};

#endif

