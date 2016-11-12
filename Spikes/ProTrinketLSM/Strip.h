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

#ifndef _ADAFRUIT_DOT_STAR_H_
	#include <Adafruit_DotStar.h>
#endif

class Strip {

	public:
		Strip(void);
		~Strip();
		Strip(uint16_t NumPixels, uint8_t Datapin, uint8_t ClockPin, uint8_t RGB, LightingSequence* initSeq, uint16_t numSeqs);
		void Update(unsigned long currentPerformanceTime);
		void setColor(uint16_t pixel, uint32_t color);
		uint16_t getCurrentSeq();
		uint16_t getCountSeqs();
		unsigned long getCurrentDuration();
		unsigned long getPrevDuration();
		unsigned long getPrevSeqTimesAccumulated();
		uint8_t getEffectNum();
		uint16_t getHeadTailofLED(const char* Type);
		Adafruit_DotStar* getStrip();
		bool getProceed();
		bool resetGlobalVars();
		bool resetPerformance();
		bool findCurrentSeqFromPerformanceTime(unsigned long performanceTime);
		LightingSequence* getCurrentLightingSequence();
		LightingSequence* getLightingSequences();

		//StripUpdateReturn* stripUpdateRet; //Holds information to return from Update method
	private:
		Adafruit_DotStar strip;
		LightingSequence* lseqs = NULL;
		bool proceed = false, init = true, forward = true;
		unsigned long currentDuration = 0, // so elapsedtime - this = time within this sequence
			prevDuration = -1, //Makes sure duration isn't processed more than once for sequence
			prevSeqTimesAccumulated = 0; //As sequences change, the duration times are accumulated to here (used to find current effect's duration)
		int16_t i = -1, j = -1, p0 = -1, p1 = 0, p2 = 1, p3 = 2, p4 = 3, p5 = 4, tail = 0, head = 3;
		uint16_t currentSequence = 0, countSeqs = 0, counter1 = 0, counter2 = 0, bounces = 0;
		int16_t* virtualPixelIndexArray = (int16_t*)calloc(100, sizeof(int16_t));
	protected:

};

#endif

