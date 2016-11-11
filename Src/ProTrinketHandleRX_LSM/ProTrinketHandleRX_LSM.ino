


#include <Structs.h>
#include <Strip.h>
#include <EffectsManager.h>
#include <Effects.h>
#include <WiFiModuleRXHandler.h>

//WiFi Module RX handling variables
WiFiModuleRXHandlerClass wmRXHandler;

//Lighting Sequence Manager variables
int8_t numStrips = 1, numEffects = 2;
uint16_t numPixels = 30;
Strip *strips;
EffectsManager effectsManager;
unsigned long localElapsedTime, temp;
EffectsManagerUpdateReturn *uRet = (EffectsManagerUpdateReturn*)calloc(numStrips, sizeof(EffectsManagerUpdateReturn));
StripUpdateReturn* sRets = (StripUpdateReturn*)calloc(numStrips, sizeof(StripUpdateReturn));
bool endSeqs = false;

void setup()
{
	/*Initialize serial interface*/
	Serial.begin(115200);
	Serial.println("Capturing serial output on ProTrinket");

	//Turn on A1 pin for ESP8266 to function
	wmRXHandler.ESP8266CH_POn(PROTRINKET5V);

	//Allocate memory for array of strips
	strips = (Strip*)calloc(numStrips, sizeof(Strip));

	//Setup lighting sequences
	LightingSequence* seqs1 = (LightingSequence*)calloc(numEffects, sizeof(LightingSequence));
	seqs1[0] = { CLEAR, numPixels, "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29 ", " ", 1000, 1000, 1, 2 };//Clear
	//seqs1[0] = { LOADCOLOR, numPixels, "0,1,2,3,4 ", "0,0,0,0,0 ", 1000, 1000, 0, 0 };//load color effect
	//seqs1[1] = { FLOWTHROUGH, numPixels, "1,2,3,4,5 ", "1,2,3,4,5 ", 200, 4000, 0, 2 };//flowthrough effect
	//seqs1[2] = { CLEAR, numPixels, "0,1,2,3,4 ", " ", 4, 4, 0, 0 };//clear effect
	//seqs1[1] = { BOUNCEBACK, numPixels, "0,1 ", "1,2 ", 200, 4000, 2, 2 };//bounceback effect
	//seqs1[1] = { RAINBOW, numPixels, "0,1,2,3,4 ", "1,2,3,4,5 ", 1000, 7000, 1, 2 };//rainbow effect
	//seqs1[1] = { RAINBOW, numPixels, "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99 ", "1,2,3,4,5 ", 1000, 7000, 1, 2 };//rainbow effect
	seqs1[1] = { RAINBOW, numPixels, "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29 ", "1,2,3,4,5 ", 200, 7000, 1, 2 };//rainbow effect
	//seqs1[2] = { LOADCOLOR, numPixels, "0,1,2,3,4 ", "1,1,1,1,1 ", 4000, 4000, 0, 0 };//load color effect
	//seqs1[3] = { RAINBOW, numPixels, "0,1,2,3,4 ", "2,2,0,2,2 ", 200, 4000, 1, 2 };//rainbow effect
	//seqs1[4] = { LOADCOLOR, numPixels, "0,1,2,3,4 ", "0,3,3,3,0 ", 4000, 4000, 0, 0 };//load color effect
	//seqs1[5] = { LOADCOLOR, numPixels, "0,1,2,3,4 ", "4,4,0,4,4 ", 4000, 4000, 0, 0 };//load color effect
	//seqs1[6] = { LOADCOLOR, numPixels, "0,1,2,3,4 ", "5,5,5,5,5 ", 4000, 4000, 0, 0 };//load color effect
	//seqs1[1] = { CLEAR, numPixels, "0,1,2,3,4 ", " ", 4000, 4000, 0, 0 };//clear effect
	//seqs1[3] = { CLEAR, numPixels, "0,1,2,3,4 ", " ", 4000, 4000, 0, 0 };//clear effect

	//Initialize strip
	strips[0] = Strip(numPixels, 8, 6, DOTSTAR_RGB, seqs1, numEffects);
	strips[0].getStrip()->begin();
	strips[0].getStrip()->show();
	strips[0].stripUpdateRet = &sRets[0];

	//LightingSequence* seqs2 = (LightingSequence*)calloc(numPixels, sizeof(LightingSequence));
	//seqs2[0] = { LOADCOLOR, numPixels, "0,1,2,3,4 ", "5,4,3,2,1 ", 5000, 5000, 0, 0 };//load color effect
	//seqs2[1] = { FLOWTHROUGH, numPixels, "0,1,2,3,4 ", "5,4,3,2,1 ", 200, 5000, 0, 2 };//flowthrough effect
	//seqs2[2] = { CLEAR, numPixels, "0,1,2,3,4 ", " ", 0, 0, 0, 0 };//clear effect
	//seqs2[3] = { BOUNCEBACK, numPixels, "0,1,2,3,4 ", "5,4,3,2,1 ", 200, 2000, 0, 2 };//bounceback effect
	//seqs2[4] = { RAINBOW, numPixels, "0,1,2,3,4 ", "5,4,3,2,1 ", 200, 6000, 1, 2 };//rainbow effect

	//strips[1] = Strip(numPixels, 8, 6, DOTSTAR_RGB, seqs2, numEffects);
	//strips[1].getStrip()->begin();
	//strips[1].getStrip()->show();
	//strips[1].stripUpdateRet = &sRets[1];

	//Initiliaze effects manager
	effectsManager = EffectsManager(strips, numStrips);
	effectsManager.effectsManagerUpdateRet = uRet;
	effectsManager.effectsManagerUpdateRet->sURet = sRets;

	//strip.begin(); // Initialize pins for output
	//strip.show();  // Turn all LEDs off ASAP
}

void loop()
{
	//Handle loop
	wmRXHandler.LoopHandle();

	//Check if beginSeqs = true
	if (wmRXHandler.beginSeqs) {
		//Signal received to start processing

		//Turn pin to high once then stop
		if (wmRXHandler.sw) {
			digitalWrite(8, HIGH);
			Serial.println("The value of beginSeq is true in loop...");
			wmRXHandler.sw = false;
		}

		//Update elapsed time in EffectsManager object
		localElapsedTime = effectsManager.getElapsedTime(millis());

		//Update effectsManager total performance time
		effectsManager.Update(localElapsedTime);

		Serial.print("Elapsed time is : ");
		Serial.println(localElapsedTime);
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
