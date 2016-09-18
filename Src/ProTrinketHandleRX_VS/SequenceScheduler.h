// SequenceScheduler.h

#ifndef _SEQUENCESCHEDULER_h
#define _SEQUENCESCHEDULER_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#ifndef _STRIP_h
	#include "Strip.h"
#endif

/**/
struct StripInfo {
	uint16_t NumPixels;
	uint8_t Datapin;
	uint8_t ClockPin;
	uint8_t BRG;
};

class SequenceSchedulerClass
{
	protected:

	public:
		SequenceSchedulerClass();
		void initializeStrips(StripInfo** Strips, LightingEffect** LEffects);
		void UpdateElapsedTime();

	private:
		
};

//extern SequenceSchedulerClass SequenceScheduler ;

#endif

