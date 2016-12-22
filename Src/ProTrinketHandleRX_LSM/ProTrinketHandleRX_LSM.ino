/*
	Author:
	Project:
	Created:
*/

#include "Structs.h"
#include <SPI.h>
#include <Adafruit_DotStar.h>
#include "Strip.h"
#include "EffectsManager.h"
#include "Effects.h"
#include "WiFiModuleRXHandler.h"

//WiFi Setup***********************************************************************************************************

//WiFi Module RX handling variables
WiFiModuleRXHandlerClass wmRXHandler;

//WiFi handling specific variables
bool endSeqs = false, firstRX = true;

//End of WiFi Setup***********************************************************************************************************

//Allocate Lighting Effect Memory for MCU: MCU1**************************************************************************************

//Effects specific variables
unsigned long localElapsedTime, temp;
int8_t  numStrips = 1, numEffects1 = 17;
uint16_t numPixels1 = 20;

//Allocate memory for effects manager and steup
EffectsManager effectsManager;
EffectsManagerUpdateReturn *uRet = (EffectsManagerUpdateReturn*)calloc(1, sizeof(EffectsManagerUpdateReturn));

//Allocate memory for strips
Strip *strips = (Strip*)calloc(numStrips, sizeof(Strip));

//Allocation memory for Lighting Sequences
LightingSequence* seqs1 = (LightingSequence*)calloc(numEffects1, sizeof(LightingSequence));

//End of Allocate Lighting Effect Memory for MCU: MCU1**************************************************************************************

//Tenp variables
uint16_t elapsedTime, prevTime;

void setup()
{
	/*Initialize serial interface*/
	Serial.begin(115200);
	Serial.println("Capturing serial output on ProTrinket");

	//Turn on A1 pin on for ESP8266 to function
	wmRXHandler.ESP8266CH_POn(PROTRINKET5V);

	//Turn A2 on for testing
	pinMode(A2, OUTPUT);
	digitalWrite(A2, LOW);

  //Initialize Lighting Effects for MCU: BATON*****************************************************************************************
  
  seqs1[0] = {FLOWTHROUGH, numPixels1, "4,0,4,0,4,0,4,0,4,0,4,0,4,0,4,0,4,0,4,0 ", 800, 9000, 100, 100, 100, 20, 800};
  seqs1[1] = {BOUNCEBACK, numPixels1, "4,0,4,0,0,0,0,0,0,0,4,0,4,0,0,0,0,0,0,0 ", 400, 4000, 100, 100, 100, 50, 600};
  seqs1[2] = {FLOWTHROUGH, numPixels1, "4,0,4,4,0,4,0,4,0,4,4,0,4,4,0,4,0,4,0,4 ", 200, 4000, 100, 100, 100, 80, 200};
  seqs1[3] = {BOUNCEBACK, numPixels1, "4,1,4,0,0,0,0,0,0,0,4,1,4,0,0,0,0,0,0,0 ", 100, 4000, 100, 100, 100, 120, 100};
  seqs1[4] = {ALLCLEAR, numPixels1, "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 ", 0, 3000, 0, 0, 255, 0, 0};
  seqs1[5] = {LOADCOLOR, numPixels1, "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 ", 500, 5000, 0, 0, 100, 150, 500};
  seqs1[6] = {LOADCOLOR, numPixels1, "4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4 ", 300, 3000, 0, 0, 255, 150, 300};
  seqs1[7] = {LOADCOLOR, numPixels1, "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 ", 3000, 3000, 0, 0, 150, 100, 300};
  seqs1[8] = {LOADCOLOR, numPixels1, "4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4 ", 7000, 7000, 0, 0, 100, 100, 200};
  seqs1[9] = {LOADCOLOR, numPixels1, "1,4,1,4,1,4,1,4,1,4,1,4,1,4,1,4,1,4,1,4 ", 6000, 6000, 0, 0, 150, 150, 400};
  seqs1[10] = {LOADCOLOR, numPixels1, "4,1,4,1,4,1,4,1,4,1,4,1,4,1,4,1,4,1,4,1 ", 2000, 2000, 0, 0, 150, 150, 400};
  seqs1[11] = {FLOWTHROUGH, numPixels1, "1,1,4,1,1,4,1,1,4,1,1,1,4,1,1,4,1,1,4,1 ", 100, 8000, 100, 100, 150, 150, 100};
  seqs1[12] = {FLOWTHROUGH, numPixels1, "4,4,1,4,4,1,4,4,1,4,4,4,1,4,4,1,4,4,1,4 ", 100, 2000, 100, 100, 150, 150, 100};
  seqs1[13] = {FLOWTHROUGH, numPixels1, "1,1,1,4,1,1,1,4,1,1,1,1,1,4,1,1,1,4,1,1 ", 100, 3000, 100, 100, 150, 200, 100};
  seqs1[14] = {FLOWTHROUGH, numPixels1, "4,1,4,1,4,1,4,1,4,1,4,1,4,1,4,1,4,1,4,1 ", 100, 2000, 100, 100, 150, 200, 100};
  seqs1[15] = {BOUNCEBACK, numPixels1, "0,0,0,4,1,1,4,0,0,0,0,0,0,4,1,1,4,0,0,0 ", 100, 6000, 100, 100, 150, 200, 100};
  seqs1[16] = {BOUNCEBACK, numPixels1, "0,4,4,1,1,1,1,4,4,0,0,4,4,1,1,1,1,4,4,0 ", 100, 7000, 100, 100, 255, 255, 100};
  
  //End of Initialize Lighting Effects for MCU: BATON*****************************************************************************************

  //Initialize Strips for MCU: BATON***************************************************************************************************
  
  strips[0] = Strip(numPixels1, 4, 5, DOTSTAR_RGB, seqs1, numEffects1);
  strips[0].getStrip()->begin();
  strips[0].getStrip()->show();
  
  
  //End of Initialize Strips for MCU: BATON***************************************************************************************************
  
  //Initialize Effects Manager for MCU: BATON***************************************************************************************
  
  //Initialize effects manager
  effectsManager = EffectsManager(strips, numStrips);
  effectsManager.effectsManagerUpdateRet = uRet;
  
  //End of Initialize Effects Manager for MCU: BATON***************************************************************************************

	//Set prev time
	prevTime = millis();

	//Turn on A1 pin off then on for ESP8266 to function
	wmRXHandler.ESP8266CH_POff(PROTRINKET5V);
	delay(1000);
	wmRXHandler.ESP8266CH_POn(PROTRINKET5V);
	delay(4000);

	Serial.println("Finished with Setup section...");
}

void loop()
{
	//Keep track of time needed to execute loop
	elapsedTime = millis() - prevTime;
	prevTime = millis();

	//Check if beginSeqs = true
	if (wmRXHandler.beginSeqs) {
		//Signal received to start processing

		//Set performance once at beginning of start
		//Turn pin to high once then stop
		if (wmRXHandler.sw) {
			digitalWrite(A2, HIGH);
			Serial.println("The value of beginSeq is true in loop...");
			wmRXHandler.sw = false;

			switch (wmRXHandler.cmd)
			{
				case WiFiModuleRXHandlerClass::RESET:
					effectsManager.resetPerformance();
					break;
				case WiFiModuleRXHandlerClass::RESUME:
					effectsManager.resumePerformance(effectsManager.getPerformanceElapsedTime());
					break;
				default:
					effectsManager.resumePerformance(effectsManager.getPerformanceElapsedTime());
					break;
			}
		}

		//Update elapsed time in EffectsManager object
		localElapsedTime = effectsManager.getElapsedTime(millis());

		//Update effectsManager total performance time
		effectsManager.Update(localElapsedTime);

		//Serial.print("Elapsed time is : ");
		Serial.println(localElapsedTime);

		Serial.print("The performance time is: ");
		Serial.println(effectsManager.getPerformanceElapsedTime());
		Serial.println();
	}
	else {
		if (wmRXHandler.sw) {
			digitalWrite(A2, LOW);
			
			Serial.println("The value of beginSeq is false in loop...");
			wmRXHandler.sw = false;

			switch (wmRXHandler.cmd)
			{
				case WiFiModuleRXHandlerClass::RESET:
					effectsManager.clearStrips();
					Serial.println("Just cleared all strips...");
					break;
				case WiFiModuleRXHandlerClass::UPDATETIME:
					effectsManager.findCurrentSeqFromPerformanceTime(wmRXHandler.updateTime);
					Serial.println("Just updated performance time...");
				default:
					break;
			}
		}

		/*Serial.print("Elapsed time is ");
		Serial.println(elapsedTime);
		Serial.println();*/
	}
}

void serialEvent() {
	if (firstRX) {
		//Read all garbage from ESP8266 and dispose, then set init to false for later reader from ESP8266
		while (Serial.read() != -1);
		firstRX = false;
	}
	else {
		wmRXHandler.RXHandler();
	}
}

