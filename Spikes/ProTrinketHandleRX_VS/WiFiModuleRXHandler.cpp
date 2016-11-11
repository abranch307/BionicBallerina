// 
// 
// 

#ifndef _WIFIMODULERXHANDLER_h
	#include "WiFiModuleRXHandler.h"
#endif

/*
*/
WiFiModuleRXHandlerClass::WiFiModuleRXHandlerClass() {}

/*
	Function ESP8266CH_POn:
	This function will turn on necessary pin to HIGH so that ESP8266 module will
	turn on

	Parameters: int PinTurnOn - this will match a defined microcontroller and turn on specific pin

	Returns: int - the value of the defined microcontroller
*/
int WiFiModuleRXHandlerClass::ESP8266CH_POn(int PinTurnOn) {
	int iret = -1;

	switch (PinTurnOn) {
		case PROTRINKET5V:
			//Turn A1 on for ESP8266 CH_P
			pinMode(A1, OUTPUT);
			digitalWrite(A1, HIGH);
			iret = PROTRINKET5V;
			break;
	}

	return iret;
}

/*
	
*/
void WiFiModuleRXHandlerClass::LoopHandle() {
	if (stringComplete) {
		Serial.println(inputString);
		// clear the string:
		inputString = "";
		stringComplete = false;
	}
}

/*
	Function RXHandler:
	This function will handle data incoming on the RX interface

	Parameters: none

	Returns: nothing
*/
void WiFiModuleRXHandlerClass::RXHandler() {
	String str;
	boolean methFound = false;
	int state, i = 0;
	UserMethods meth = LED;

	Serial.println("I made it into RXHandler interrupt...");
	stringComplete = true;

	//Loop through received data
	while (Serial.available()) {
		//If a method has been found, then skip to use method, otherwise
		//keep looking for method in RX info
		if (!methFound) {
			//Read string from RX
			str = Serial.readStringUntil('\n');
			str.trim();
			inputString += "The read string was " + str + "\n";

			//Verify if valid method found
			if (str.equals(LEDSWITCH)) {
				//Serial.println("LED string found!");
				inputString += "LED string found!\n";
				meth = LED;
				methFound = true;
			}
			else if (str.equals(USERINFORECV)) {
				//Serial.println("USERINFO string found!");
				inputString += "USERINFO string found!\n";
				meth = USERINFO;
				methFound = true;
			}
			else if (str.equals(INITLEDSEQS)) {
				//Serial.println("INITLEDSEQS string found!");
				inputString += "INITLEDSEQS string found!\n";
				meth = ILEDSEQS;
				methFound = true;
			}
		}
		else {
			//Initialize variables
			String tstr = "";
			
			//Add comment to inputString for later notification to user
			inputString += "I'm in the meth found portion!\n";

			switch (meth) {
				case LED:
					//Read int into state and change pin
					state = Serial.parseInt();
					inputString += "The state will be changed to " + String(state) + "\n";
					if (state == 0) {
						pinMode(8, OUTPUT);
						digitalWrite(8, LOW);
					}
					else {
						pinMode(8, OUTPUT);
						digitalWrite(8, HIGH);
					}
					break;
				case USERINFO:
					//Create userInfo struct
					userInfo ui;

					//Read username and convert to character array
					tstr = Serial.readStringUntil('\n');
					tstr.toCharArray(ui.username, (tstr.length() + 1));

					//Read number and convert to integer
					tstr = Serial.readStringUntil('\n');
					ui.num = tstr.toInt();

					inputString += "The username is " + String(ui.username) + ", ";
					inputString += "The number is " + String(ui.num);

					break;
				case ILEDSEQS:
					//Read int into state and change beginSeqs boolean
					state = Serial.parseInt();
					sw = true;

					switch (state) {
						case START:
							//Set a global variable to yes
							beginSeqs = true;

							inputString += "The beginSeqs was set to true and the state will be changed to " + String(state) + "\n";
							break;
						case STOP:
							//Set a global variable to no
							beginSeqs = false;

							inputString += "The beginSeqs was set to false and the state will be changed to " + String(state) + "\n";

							break;
						case RESTART:
							//Set a global variable to no
							beginSeqs = false;

							inputString += "The beginSeqs was set to false and the state will be changed to " + String(state) + "\n";

							break;
					}
					break;
			}

			//Read the rest of string from the serial interface
			Serial.readString();
		}
	}
}