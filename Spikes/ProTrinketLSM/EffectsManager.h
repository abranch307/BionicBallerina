// EffectsManager.h

#ifndef _EFFECTSMANAGER_h
#define _EFFECTSMANAGER_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#ifndef _STRUCTS_h
	#include "Structs.h"
#endif

#ifndef _STRIP_h
	#include "Strip.h""
#endif

class EffectsManager {

	public:
		EffectsManager(void);
		~EffectsManager();
		EffectsManager(Strip *strips, int8_t numStrips);
		void Update(unsigned long elapsedTime);
		uint16_t getElapsedTime(unsigned long currentTime);
		bool resetPerformance();
		bool findCurrentSeqFromPerformanceTime(unsigned long performanceTime); //performance time is in milliseconds
		Strip* getStrips();

		EffectsManagerUpdateReturn* effectsManagerUpdateRet = NULL; //Holds information to return from Update method
		int8_t countStrips = 0;
	private:
		Strip *strips2Manage = NULL; //Class array of strips connected to microcontroller along with lighting sequences to run
		unsigned long performanceElapsedTime; //Time gone by since sequence started - milliseconds
		long prevTime = 0; //Time when microcontroller global loop ended
	protected:

};

#endif

