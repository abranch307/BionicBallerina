/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created: 
	Class:
	Class Description:

*/

#include "Structs.h"
#include <SPI.h>
#include "Adafruit_DotStar.h"
#include "Strip.h"
#include "EffectsManager.h"
#include "Effects.h"

//Declare setup values
unsigned long localElapsedTime, temp;
int8_t numStrips = 2, numEffects1 = 5, numEffects2 = 3;
uint16_t numPixels1 = 120, numPixels2 = 5;
bool endSeqs = false;

//Allocate memory for effects manager and setup
EffectsManager effectsManager;
EffectsManagerUpdateReturn *uRet = (EffectsManagerUpdateReturn*)calloc(numStrips, sizeof(EffectsManagerUpdateReturn));

//Allocate memory for strips
Strip *strips = (Strip*)calloc(numStrips, sizeof(Strip));;

//Allocation memory for Lighting Sequences
LightingSequence* seqs1 = (LightingSequence*)calloc(numEffects1, sizeof(LightingSequence));
LightingSequence* seqs2 = (LightingSequence*)calloc(numEffects2, sizeof(LightingSequence));

void setup()
{
	/*Initialize serial interface*/
	Serial.begin(115200);
	Serial.println("Capturing serial output on ProTrinket");

	//Setup lighting sequences 1
	seqs1[0] = { RAINBOW, numPixels1, "0,0,0,0,0 ", 200, 7000, 1, 2, 100, 0, 0 };//rainbow effect
	seqs1[1] = { FLOWTHROUGH, numPixels1, "0,1,2,0,2,1,0 ", 200, 11000, 0, 44, 100, 0, 0 };//flowthrough effect
	seqs1[2] = { LOADCOLOR, numPixels1, "0,4,0,4,0 ", 500, 4000, 0, 0, 10, 0, 0 };//load color effect
	seqs1[3] = { BOUNCEBACK, numPixels1, "0,0,1,2,0 ", 200, 4000, 4, 2, 150, 0, 0 };//bounceback effect
	seqs1[4] = { FLOWTHROUGH, numPixels1, "0,4,0,4,0 ", 200, 11000, 0, 44, 100, 0, 0 };//flowthrough effect

	//Initialize strip 1
	strips[0] = Strip(numPixels1, 8, 6, DOTSTAR_RGB, seqs1, numEffects1);
	strips[0].getStrip()->begin();
	strips[0].getStrip()->show();

	//Setup lighting sequences 2
	seqs2[0] = { LOADCOLOR, numPixels2, "5,4,3,2,1 ", 5000, 5000, 0, 0 };//load color effect
	seqs2[1] = { FLOWTHROUGH, numPixels2, "0,2,3,2,0 ", 200, 5000, 0, 20 };//flowthrough effect
	//seqs2[2] = { CLEAR, numPixels2, " ", 0, 0, 0, 0 };//clear effect
	seqs2[2] = { BOUNCEBACK, numPixels2, "0,3,3,2,0 ", 200, 8000, 8, 8 };//bounceback effect
	//seqs2[4] = { RAINBOW, numPixels2, "5,4,3,2,1 ", 200, 6000, 1, 2 };//rainbow effect

	//Initiliaze strip 2
	strips[1] = Strip(numPixels2, 4, 5, DOTSTAR_RGB, seqs2, numEffects2);
	strips[1].getStrip()->begin();
	strips[1].getStrip()->show();
	
	//Initiliaze effects manager
	effectsManager = EffectsManager(strips, numStrips);
	effectsManager.effectsManagerUpdateRet = uRet;

	Serial.print("Strip 2 details - the number of sequences are ");
	Serial.println(effectsManager.getStrips()[1].getCountSeqs());
	Serial.println();
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

