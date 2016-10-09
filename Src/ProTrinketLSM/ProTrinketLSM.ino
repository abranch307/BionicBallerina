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

int8_t numStrips = 1, numEffects = 7;
uint16_t numPixels = 5;
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
	strips = (Strip*)calloc(numStrips, sizeof(Strip));

	//Setup lighting sequences
	LightingSequence* seqs1 = (LightingSequence*)calloc(numEffects, sizeof(LightingSequence));
	seqs1[0] = { LOADCOLOR, numPixels, "0,1,2,3,4 ", "0,0,0,0,0 ", 1000, 1000, 0, 0 };//load color effect
	//seqs1[1] = { FLOWTHROUGH, numPixels, "1,2,3,4,5 ", "1,2,3,4,5 ", 200, 4000, 0, 2 };//flowthrough effect
	//seqs1[2] = { CLEAR, numPixels, "0,1,2,3,4 ", " ", 4, 4, 0, 0 };//clear effect
	seqs1[1] = { BOUNCEBACK, numPixels, "0,1 ", "1,2 ", 200, 4000, 2, 2 };//bounceback effect
	seqs1[2] = { RAINBOW, numPixels, "0,1,2,3,4 ", "1,2,3,4,5 ", 200, 7000, 1, 2 };//rainbow effect
	//seqs1[2] = { LOADCOLOR, numPixels, "0,1,2,3,4 ", "1,1,1,1,1 ", 4000, 4000, 0, 0 };//load color effect
	seqs1[3] = { RAINBOW, numPixels, "0,1,2,3,4 ", "2,2,0,2,2 ", 200, 4000, 1, 2 };//rainbow effect
	seqs1[4] = { LOADCOLOR, numPixels, "0,1,2,3,4 ", "0,3,3,3,0 ", 4000, 4000, 0, 0 };//load color effect
	seqs1[5] = { LOADCOLOR, numPixels, "0,1,2,3,4 ", "4,4,0,4,4 ", 4000, 4000, 0, 0 };//load color effect
	seqs1[6] = { LOADCOLOR, numPixels, "0,1,2,3,4 ", "5,5,5,5,5 ", 4000, 4000, 0, 0 };//load color effect
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
	//Update elapsed time in EffectsManager object
	localElapsedTime = effectsManager.getElapsedTime(millis());

	//temp++;

	effectsManager.Update(localElapsedTime);

	//temp = (unsigned long)((float)temp / float(1000)) * 1000;
	//if ((temp % 2000) == 0) 
	//{
		//effectsManager.getStrips()[0].getStrip()->setPixelColor(3, RED);
		
		//Effects::loadColor(effectsManager.getStrips()[0].getStrip(), effectsManager.getStrips()[0].getLightingSequences(), 2, 0, false, false, NULL);
		//effectsManager.getStrips()[0].getStrip()->show();
		//Serial.print("Effect ");
		//Serial.println(effectsManager.getStrips()[0].getLightingSequences()[2].lightsequence);
		//Serial.print("Pixels ");
		//Serial.println(effectsManager.getStrips()[0].getLightingSequences()[2].effectedPixels);
		//Serial.print("Colors ");
		//Serial.println(effectsManager.getStrips()[0].getLightingSequences()[2].colors);
		//Serial.println("");
		//delay(2000);

		//Effects::allClear(effectsManager.getStrips()[0].getStrip(), effectsManager.getStrips()[0].getLightingSequences(), 0, NULL);
		//effectsManager.getStrips()[0].getStrip()->setPixelColor(4, CLEAR);
		//effectsManager.getStrips()[0].getStrip()->show();
		//delay(1000);
		//effectsManager.Update(localElapsedTime);
		//Serial.println("Just updated effectsManager");
		
	//}
	
	Serial.print("Elapsed time is : ");
	Serial.println(localElapsedTime);

	if (uRet != NULL) {
		for (int i = 0; i < effectsManager.countStrips; i++) {
			if (&effectsManager.effectsManagerUpdateRet->sURet[i] != NULL) {
				//Exit if end of performance is reached
				if (effectsManager.getStrips()[i].getCurrentSeq() >= effectsManager.getStrips()[i].getCountSeqs()) {
					//Skip
					endSeqs = true;
				}
				else {
					Serial.print("Strip ");
					Serial.println(i + 1);
					//Serial.print("The current seq is ");
					//Serial.println(effectsManager.getStrips()[i].getCurrentSeq());
					//Serial.print("The seq count is ");
					//Serial.println(effectsManager.getStrips()[i].getCountSeqs());
					//Serial.print(" Performance Time : ");
					//Serial.println(effectsManager.effectsManagerUpdateRet->sURet[i].performanceElapsedTime);
					//Serial.print("EffectNum is: ");
					//Serial.println(effectsManager.getStrips()[i].getEffectNum());
					//Serial.println(effectsManager.effectsManagerUpdateRet->sURet[i].effectNum);
					//Serial.print("Current Sequence is ");
					//Serial.println(effectsManager.getStrips()[i].getCurrentSeq());
					//Serial.print("Effect Pixels & Colors ");
					//Serial.println(effectsManager.effectsManagerUpdateRet->sURet[i].sRet);
					//Serial.print("User enforced CurrentSequence is: ");
					//Serial.println(effectsManager.effectsManagerUpdateRet->sURet[i].currentSequence);
					//Serial.print("Current Duration is ");
					//Serial.println(effectsManager.getStrips()[i].getCurrentDuration());
					//Serial.print("User enforced Current Duration is: ");
					//Serial.println(effectsManager.effectsManagerUpdateRet->sURet[i].currentDuration);
					//Serial.print("PST Accum is ");
					//Serial.println(effectsManager.getStrips()[i].getPrevSeqTimesAccumulated());
					//Serial.print("Effect Success is ");
					//Serial.println(effectsManager.effectsManagerUpdateRet->sURet[i].effectSuccess);
					//Serial.print("Effect Duration is ");
					//Serial.println(effectsManager.getStrips()[i].getCurrentLightingSequence()->duration);
					Serial.print("Init = ");
					Serial.println(effectsManager.effectsManagerUpdateRet->sURet[i].currentSequence);
					Serial.print("Shift Pixels By = ");
					Serial.println(effectsManager.effectsManagerUpdateRet->sURet[i].currentDuration);
					Serial.print("Head = ");
					Serial.println(effectsManager.effectsManagerUpdateRet->sURet[i].prevSeqTimesAccumulated);
					Serial.print("Tail = ");
					Serial.println(effectsManager.effectsManagerUpdateRet->sURet[i].effectNum);
					Serial.print("Effect success = ");
					Serial.println(effectsManager.effectsManagerUpdateRet->sURet[i].effectSuccess);

					Serial.println();
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

