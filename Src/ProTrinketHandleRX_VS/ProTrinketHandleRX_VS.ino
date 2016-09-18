#ifndef _WIFIMODULERXHANDLER_h
	#include "WiFiModuleRXHandler.h"
#endif

#ifndef _STRIP_h
	#include "Strip.h"
#endif

#ifndef _SEQUENCESCHEDULER_h
	#include "SequenceScheduler.h"
#endif

WiFiModuleRXHandlerClass wmRXHandler;
SequenceSchedulerClass stripScheduler;
StripInfo **strips;
LightingEffect **leffects;

void setup()
{
	/*Setup variables*/

	/*Initialize serial interface*/
	Serial.begin(115200);
	Serial.println("Capturing serial output on ProTrinket");

	//Turn on A0 pin for ESP8266 to function
	wmRXHandler.ESP8266CH_POn(PROTRINKET5V);

	//Initialize strip arrays then pass to Sequence Scheduler
	strips = (StripInfo**)calloc(2, sizeof(StripInfo*));

	strips[0] = (StripInfo*)calloc(1, sizeof(StripInfo));
	strips[0]->NumPixels = 30;
	strips[0]->Datapin = 4;
	strips[0]->ClockPin = 5;

	strips[1] = (StripInfo*)calloc(1, sizeof(StripInfo));
	strips[1]->NumPixels = 30;
	strips[1]->Datapin = 8;
	strips[1]->ClockPin = 6;

	leffects = (LightingEffect**)calloc(2, sizeof(LightingEffect*));

	leffects[0] = (LightingEffect*)calloc(5, sizeof(LightingEffect));
	leffects[0][0] = {2, 5, 2, WHITE, 50, 4, 0, 0};//load color effect
	leffects[0][1] = {4, 5, 2, BLUE, 50, 4, 0, 2};//flowthrough effect
	leffects[0][2] = {0, 5, 2, 0, 0, 4, 0, 0};//clear effect
	leffects[0][3] = {3, 5, 2, GREEN, 50, 4, 1, 0};//bounceback effect
	leffects[0][4] = {1, 5, 0, 0, 50, 4, 0, 1};//rainbow effect

	leffects[1] = (LightingEffect*)calloc(5, sizeof(LightingEffect));
	leffects[1][0] = { 2, 5, 2, WHITE, 50, 4, 0, 0 };//load color effect
	leffects[1][1] = { 4, 5, 2, BLUE, 50, 4, 0, 2 };//flowthrough effect
	leffects[1][2] = { 0, 5, 2, 0, 0, 4, 0, 0 };//clear effect
	leffects[1][3] = { 3, 5, 2, GREEN, 50, 4, 1, 0 };//bounceback effect
	leffects[1][4] = { 1, 5, 0, 0, 50, 4, 0, 1 };//rainbow effect
	
	//Initialize strip scheduler's strips with pins and effects
	stripScheduler.initializeStrips(strips, leffects);

	//Turn pin 8 to output and to high
	pinMode(8, OUTPUT);
	digitalWrite(8, HIGH);
}

void loop()
{
	/*Handle loop*/
	wmRXHandler.LoopHandle();

	//Update TimerHandle
	stripScheduler.UpdateElapsedTime();

	//Check if beginSeqs = true
	if (wmRXHandler.beginSeqs) {
		//Signal received to start processing

		//Turn pin to high once then stop
		if (wmRXHandler.sw) {
			digitalWrite(8, HIGH);
			Serial.println("The value of beginSeq is true in loop...");
			wmRXHandler.sw = false;
		}

		//Process strip lighting effects
		stripScheduler.UpdateElapsedTime();
	}
	else {
		if (wmRXHandler.sw) {
			digitalWrite(8, LOW);
			Serial.println("The value of beginSeq is false in loop...");
			wmRXHandler.sw = false;
		}
	}

}

void serialEvent() {
	wmRXHandler.RXHandler();
}
