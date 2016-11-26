/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created:
	Class:
	Class Description:

*/

#ifndef _WIFIMODULERXHANDLER_h
	#include "WiFiModuleRXHandler.h"
#endif

WiFiModuleRXHandlerClass wmRXHandler;

void setup()
{
	/*Setup variables*/

	/*Initialize serial interface*/
	Serial.begin(115200);
	Serial.println("Capturing serial output on ProTrinket");

	//Turn on A1 pin for ESP8266 to function
	wmRXHandler.ESP8266CH_POn(PROTRINKET5V);

	//Turn pin 8 to output and to high
	pinMode(8, OUTPUT);
	digitalWrite(8, HIGH);
}

void loop()
{
	/*Handle loop*/
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
