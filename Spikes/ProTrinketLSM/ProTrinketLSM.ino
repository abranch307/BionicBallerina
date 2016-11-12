/*
	Name:
	Created:
	Author:
*/

#include "Structs.h"
#include <SPI.h>
#include "Adafruit_DotStar.h"
#include "Strip.h"
#include "EffectsManager.h"
#include "Effects.h"

int8_t numStrips = 1, numEffects = 2;
uint16_t numPixels = 5;
Strip *strips;
EffectsManager effectsManager;
unsigned long localElapsedTime, temp;
EffectsManagerUpdateReturn *uRet = (EffectsManagerUpdateReturn*)calloc(numStrips, sizeof(EffectsManagerUpdateReturn));
bool endSeqs = false;

void setup()
{
	/*Initialize serial interface*/
	Serial.begin(115200);
	Serial.println("Capturing serial output on ProTrinket");

	//Allocate memory for array of strips
	strips = (Strip*)calloc(numStrips, sizeof(Strip));

	//Setup lighting sequences
	LightingSequence* seqs1 = (LightingSequence*)calloc(numEffects, sizeof(LightingSequence));
	//seqs1[0] = { CLEAR, numPixels, , " ", 1000, 1000, 1, 2 };//Clear
	//seqs1[0] = { CLEAR, numPixels, " ", 1000, 1000, 0, 0 };//Clear
	//seqs1[1] = { LOADCOLOR, numPixels, "2,2,2,2,2 ", 3000, 3000, 0, 0 };//load color effect
	//seqs1[2] = { CLEAR, numPixels, "0,0,0,0,0 ", 1000, 1000, 0, 0 };//Clear
	//seqs1[3] = { LOADCOLOR, numPixels, "8,8,8,8,8 ", 3000, 3000, 0, 0 };//load color effect
	//seqs1[4] = { CLEAR, numPixels, "0,0,0,0,0 ", 1000, 1000, 0, 0 };//Clear
	//seqs1[1] = { FLOWTHROUGH, numPixels, "1,2,3,4,5 ", 200, 4000, 0, 2 };//flowthrough effect
	//seqs1[2] = { CLEAR, numPixels, " ", 4, 4, 0, 0 };//clear effect
	//seqs1[1] = { BOUNCEBACK, numPixels, "0,0,1,2,0 ", 200, 5000, 2, 2 };//bounceback effect
	seqs1[0] = { FLOWTHROUGH, numPixels, "1,2,0,2,1 ", 200, 11000, 0, 2 };//flowthrough effect
	seqs1[1] = { RAINBOW, numPixels, "3,3,3,3,3 ", 200, 7000, 1, 2 };//rainbow effect
	//seqs1[1] = { RAINBOW, numPixels, "1,2,3,4,5 ", 1000, 7000, 1, 2 };//rainbow effect
	//seqs1[1] = { RAINBOW, numPixels, "1,2,3,4,5 ", 200, 7000, 1, 2 };//rainbow effect
	//seqs1[6] = { LOADCOLOR, numPixels, "4,4,4,4,4 ", 4000, 4000, 0, 0 };//load color effect
	//seqs1[6] = { RAINBOW, numPixels, "2,2,0,2,2 ", 200, 6000, 1, 2 };//rainbow effect
	//seqs1[6] = { CLEAR, numPixels"0,0,0,0,0 ", 1000, 1000, 1, 2 };//Clear
	//seqs1[4] = { LOADCOLOR, numPixels, "0,4,4,4,0 ", 4000, 4000, 0, 0 };//load color effect
	//seqs1[1] = { BOUNCEBACK, numPixels, "0,0,1,2,0 ", 200, 4000, 4, 2 };//bounceback effect
	//seqs1[5] = { LOADCOLOR, numPixels, "4,4,0,4,4 ", 4000, 4000, 0, 0 };//load color effect
	//seqs1[6] = { LOADCOLOR, numPixels, "5,5,5,5,5 ", 4000, 4000, 0, 0 };//load color effect
	//seqs1[1] = { CLEAR, numPixels, " ", 4000, 4000, 0, 0 };//clear effect
	//seqs1[3] = { CLEAR, numPixels, " ", 4000, 4000, 0, 0 };//clear effect

	//Initialize strip
	strips[0] = Strip(numPixels, 8, 6, DOTSTAR_RGB, seqs1, numEffects);
	strips[0].getStrip()->begin();
	strips[0].getStrip()->show();
	//strips[0].stripUpdateRet = &sRets[0];

	//LightingSequence* seqs2 = (LightingSequence*)calloc(numPixels, sizeof(LightingSequence));
	//seqs2[0] = { LOADCOLOR, numPixels, "5,4,3,2,1 ", 5000, 5000, 0, 0 };//load color effect
	//seqs2[1] = { FLOWTHROUGH, numPixels, "5,4,3,2,1 ", 200, 5000, 0, 2 };//flowthrough effect
	//seqs2[2] = { CLEAR, numPixels, " ", 0, 0, 0, 0 };//clear effect
	//seqs2[3] = { BOUNCEBACK, numPixels, "5,4,3,2,1 ", 200, 2000, 0, 2 };//bounceback effect
	//seqs2[4] = { RAINBOW, numPixels, "5,4,3,2,1 ", 200, 6000, 1, 2 };//rainbow effect

	//strips[1] = Strip(numPixels, 8, 6, DOTSTAR_RGB, seqs2, numEffects);
	//strips[1].getStrip()->begin();
	//strips[1].getStrip()->show();
	//strips[1].stripUpdateRet = &sRets[1];
	
	//Initiliaze effects manager
	effectsManager = EffectsManager(strips, numStrips);
	effectsManager.effectsManagerUpdateRet = uRet;

	//strip.begin(); // Initialize pins for output
	//strip.show();  // Turn all LEDs off ASAP
}

void loop()
{
	//Update elapsed time in EffectsManager object
	localElapsedTime = effectsManager.getElapsedTime(millis());

	effectsManager.Update(localElapsedTime);
	
	Serial.print("Elapsed time is : ");
	Serial.println(localElapsedTime);

	Serial.println(effectsManager.effectsManagerUpdateRet->performanceElapsedTime);
	Serial.println();
}

