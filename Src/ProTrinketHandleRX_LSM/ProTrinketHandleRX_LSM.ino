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
int8_t  numStrips = 1, numEffects1 = 13;
uint16_t numPixels1 = 5;

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
	Serial.setTimeout(1000);
	Serial.println("Capturing serial output on ProTrinket");

	////Turn on A1 pin on for ESP8266 to function
	//wmRXHandler.ESP8266CH_POn(PROTRINKET5V);

	////Turn A2 on for testing
	//pinMode(A2, OUTPUT);
	//digitalWrite(A2, LOW);

	//Initialize Lighting Effects for MCU: MCU1*****************************************************************************************

	  seqs1[0] = {RAINBOW, numPixels1, "0,0,0,0,0 ", 100, 5000, 0, 50};
	  seqs1[1] = {FLOWTHROUGH, numPixels1, "6,4,0,0,3 ", 0, 5000, 0, 100};
	  seqs1[2] = {LOADCOLOR, numPixels1, "6,4,0,2,0 ", 500, 500, 0, 0};
	  seqs1[3] = {LOADCOLOR, numPixels1, "2,1,0,0,5 ", 500, 500, 0, 0};
	  seqs1[4] = {LOADCOLOR, numPixels1, "0,4,0,4,0 ", 500, 500, 0, 0};
	  seqs1[5] = {LOADCOLOR, numPixels1, "3,0,0,0,3 ", 500, 500, 0, 0};
	  seqs1[6] = {LOADCOLOR, numPixels1, "0,2,0,2,0 ", 500, 500, 0, 0};
	  seqs1[7] = {LOADCOLOR, numPixels1, "1,0,3,0,1 ", 500, 500, 0, 0};
	  seqs1[8] = {LOADCOLOR, numPixels1, "3,0,1,0,3 ", 500, 500, 0, 0};
	  seqs1[9] = {LOADCOLOR, numPixels1, "8,0,8,0,2 ", 500, 500, 0, 0};
	  seqs1[10] = {LOADCOLOR, numPixels1, "2,0,2,0,8 ", 500, 500, 0, 0};
	  seqs1[11] = {LOADCOLOR, numPixels1, "0,5,4,5,0 ", 500, 500, 0, 0};
	  seqs1[12] = {BOUNCEBACK, numPixels1, "0,5,4,5,0 ", 500, 10000, 2, 100};
  
	  //End of Initialize Lighting Effects for MCU: MCU1*****************************************************************************************
  
	  //Initialize Strips for MCU: MCU1***************************************************************************************************
  
	  strips[0] = Strip(numPixels1, 4, 5, DOTSTAR_RGB, seqs1, numEffects1);
	  strips[0].getStrip()->begin();
	  strips[0].getStrip()->show();
  
  
	  //End of Initialize Strips for MCU: MCU1***************************************************************************************************
  
	  //Initialize Effects Manager for MCU: MCU1***************************************************************************************
  
	  //Initialize effects manager
	  effectsManager = EffectsManager(strips, numStrips);
	  effectsManager.effectsManagerUpdateRet = uRet;
  
	  //End of Initialize Effects Manager for MCU: MCU1***************************************************************************************

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

