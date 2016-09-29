/*
	Name:
	Created:
	Author:
*/

#include "Structs.h"
#include <SPI.h>
#include "Adafruit_DotStar/Adafruit_DotStar.h"
#include "Strip.h"
#include "EffectsManager.h"
#include "Effects.h"

int8_t numStrips = 2;
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

	//Allocate memory for array of strips
	strips = (Strip*)calloc(2, sizeof(Strip));

	////Setup lighting sequences

	LightingSequence* seqs1 = (LightingSequence*)calloc(5, sizeof(LightingSequence));
	seqs1[0] = { LOADCOLOR, 5, "1,2,3,4,5", "1,2,3,4,5", 0, 4000, 0, 0 };//load color effect
	seqs1[1] = { FLOWTHROUGH, 5, "1,2,3,4,5", "1,2,3,4,5", 0, 4000, 0, 2 };//flowthrough effect
	seqs1[2] = { CLEAR, 5, "", "", 0, 4, 0, 0 };//clear effect
	seqs1[3] = { BOUNCEBACK, 5, "1,2,3,4,5", "1,2,3,4,5", 4000, 1000, 0, 2 };//bounceback effect
	seqs1[4] = { RAINBOW, 5, "1,2,3,4,5", "1,2,3,4,5", 4000, 5000, 1, 2 };//rainbow effect

	//Initialize strip
	strips[0] = Strip(30, 4, 5, DOTSTAR_RGB, seqs1, 5);
	strips[0].stripUpdateRet = &sRets[0];

	LightingSequence* seqs2 = (LightingSequence*)calloc(5, sizeof(LightingSequence));
	seqs2[0] = { LOADCOLOR, 5, "1,2,3,4,5", "5,4,3,2,1", 0, 5000, 0, 0 };//load color effect
	seqs2[1] = { FLOWTHROUGH, 5, "1,2,3,4,5", "5,4,3,2,1", 4000, 5000, 0, 2 };//flowthrough effect
	seqs2[2] = { CLEAR, 5, "", "", 0, 0, 0, 0 };//clear effect
	seqs2[3] = { BOUNCEBACK, 5, "1,2,3,4,5", "5,4,3,2,1", 4000, 2000, 0, 2 };//bounceback effect
	seqs2[4] = { RAINBOW, 5, "1,2,3,4,5", "5,4,3,2,1", 4000, 6000, 1, 2 };//rainbow effect

	strips[1] = Strip(30, 8, 6, DOTSTAR_RGB, seqs2, 5);
	strips[1].stripUpdateRet = &sRets[1];
	
	//Initiliaze effects manager
	effectsManager = EffectsManager(strips, numStrips);
	effectsManager.effectsManagerUpdateRet = uRet;
	effectsManager.effectsManagerUpdateRet->sURet = sRets;
}

void loop()
{
	//Update elapsed time in EffectsManager object
	localElapsedTime = effectsManager.getElapsedTime(millis());

	temp++;

	//if ((temp % 4000) == 0) 
	//{
		effectsManager.Update(localElapsedTime);
		//Serial.println("Just updated effectsManager");
		//delay(1000);
	//}
	
	Serial.print("Elapsed time is : ");
	Serial.println(localElapsedTime);
	//Serial.println();

	if (uRet != NULL) {
		for (int i = 0; i < effectsManager.countStrips; i++) {
			if (effectsManager.effectsManagerUpdateRet->sURet != NULL) {
				//Exit if end of performance is reached
				if (effectsManager.getStrips()[i].getCurrentSeq() >= effectsManager.getStrips()[i].getCountSeqs()) {
					//Skip
					endSeqs = true;
				}
				else {
					Serial.print("Strip ");
					Serial.println(i + 1);
					//Serial.print(" Performance Time : ");
					//Serial.println(effectsManager.effectsManagerUpdateRet->sURet->performanceElapsedTime);
					Serial.print("EffectNum is ");
					//Serial.println(effectsManager.getStrips()[i].getEffectNum());
					Serial.println(effectsManager.effectsManagerUpdateRet->sURet->effectNum);
					//Serial.print("Current Sequence is ");
					//Serial.println(effectsManager.getStrips()[i].getCurrentSeq());
					//Serial.print("Current Duration is ");
					//Serial.println(effectsManager.getStrips()[i].getCurrentDuration());
					//Serial.print("PST Accum is ");
					//Serial.println(effectsManager.getStrips()[i].getPrevSeqTimesAccumulated());
					//Serial.print("Effect Duration is ");
					//Serial.println(effectsManager.getStrips()[i].getCurrentLightingSequence()->duration);
					//Serial.println();
				}
			}
			else {
				Serial.println("NULL");
			}
		}
	}

	Serial.println(effectsManager.effectsManagerUpdateRet->performanceElapsedTime);
	Serial.println();
}

