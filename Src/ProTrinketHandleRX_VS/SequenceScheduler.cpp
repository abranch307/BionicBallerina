// 
// 
// 

#include "SequenceScheduler.h"
#include "Servo\src\Servo.h"

#ifndef _STRIP_h
	#include "Strip.h"
#endif

//Declare global variables
float prevTime;
float elapsedTime;
StripClass *stripArray;

/*
*/
SequenceSchedulerClass::SequenceSchedulerClass() {
	//Initialize elapsedTime to zero
	elapsedTime = 0;
	prevTime = millis();
}

/*
*/
void initializeStrips(StripInfo **Strips, LightingEffect **LEffects) {
	//Declare variables
	int i = 0;
	stripArray = new StripClass[(sizeof(Strips) / sizeof(StripInfo))];
	
	//Loop through and initialize all strips with a StripClass class
	for (i = 0; i < (sizeof(Strips) / sizeof(StripInfo)); i++) {
		stripArray[i].UpdateStripData(Strips[i], LEffects[i]);
	}
}

/*
*/
void SequenceSchedulerClass::UpdateElapsedTime() {
	//Track current millis
	unsigned long currentMillis = millis();
	int i = 0;

	//Add to elapsed time by subtracting current millis from prevTime
	elapsedTime += currentMillis - prevTime;

	//If elapsed time is greater than 500 millis, then set global flag timeToUpdate to true
	if (elapsedTime >= 500) {
		//Update all strip's counters
		for (i = 0; i < (sizeof(stripArray) / sizeof(StripClass)); i++) {
			stripArray[i].updateCounter();
		}

		//Reset elapsedTIme to zero to wait for next 500 millis to pass
		elapsedTime = 0;
	}

	//Set prevTime to currentMillis
	prevTime = currentMillis;
}

