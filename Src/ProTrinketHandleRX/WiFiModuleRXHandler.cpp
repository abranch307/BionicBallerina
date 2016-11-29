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

			//Set default command
			cmd = RESET;

			break;
	}

	return iret;
}

/*
*/
int WiFiModuleRXHandlerClass::ESP8266CH_POff(int PinTurnOn) {
	int iret = -1;

	switch (PinTurnOn) {
	case PROTRINKET5V:
		//Turn A1 on for ESP8266 CH_P
		pinMode(A1, OUTPUT);
		digitalWrite(A1, LOW);
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
	int state, i = 0, looping = 0, looping2 = 0;
	UserMethods meth = LED;
	//inputString = "";

	//Reset serialString
	emptySerialString();

	Serial.println("I made it into RXHandler interrupt...");

	//Loop through received data
	while (Serial.available() > 0) {
		//Set string complete to true to string will be read to TX via serial later
		stringComplete = true;

		looping++;
		Serial.print("Serial is available and loop count is ");
		Serial.println(looping);

		//Exit serial if looping is greater than 2
		if (looping > 2) {
			break;
		}

		//If a method has been found, then skip to use method, otherwise
		//keep looking for method in RX info
		if (!methFound) {
			Serial.println("No method found yet so reading until new line");

			looping2 = -1;

			//Read string from RX
			Serial.setTimeout(1000);
			while (Serial.available()) {
				looping2++;
				delay(1);  //small delay to allow input buffer to fill
				char c = Serial.read();  //gets one byte from serial buffer
				if (c == '\n') {
					break; //breaks out of capture loop to print readstring
				}
				serialString[looping2] = c; //makes the string readString
				Serial.println(serialString);

				if (looping2 > 100) {
					break;
				}
			}
			//str = Serial.readStringUntil('\n');
			str = serialString;

			Serial.print("Found a new line now and the value is: ");
			Serial.println(str);

			str.trim();
			//inputString += "The read string was " + str + "\n";

			//Verify if valid method found
			if (str.equals(LEDSWITCH)) {
				Serial.println("LED string found!");
				//inputString += "LED string found!\n";
				meth = LED;
				methFound = true;
			}
			else if (str.equals(USERINFORECV)) {
				Serial.println("USERINFO string found!");
				//inputString += "USERINFO string found!\n";
				meth = USERINFO;
				methFound = true;
			}
			else if (str.equals(INITLEDSEQS)) {
				Serial.println("INITLEDSEQS string found!");
				//inputString += "INITLEDSEQS string found!\n";
				meth = ILEDSEQS;
				methFound = true;
			}
			else if (str.equals(UPDATEPERFORMANCETIME)) 
			{
				Serial.println("UPT string found!");
				//inputString += "UPT string found!\n";
				meth = UPT;
				methFound = true;
			}
			else {
				//Empty serial buffer
				while (Serial.read() != -1);
				Serial.println("Emptied serial buffer.  Should exit now...");
			}
		}
		else {
			//Initialize variables
			String tstr = "";
			
			//Add comment to inputString for later notification to user
			//inputString += "I'm in the meth found portion!\n";

			switch (meth) {
				case LED:
					//Read int into state and change pin
					state = Serial.parseInt();
					//inputString += "The state will be changed to " + String(state) + "\n";
					if (state == 0) {
						pinMode(A3, OUTPUT);
						digitalWrite(A3, LOW);
					}
					else {
						pinMode(A3, OUTPUT);
						digitalWrite(A3, HIGH);
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

					//inputString += "The username is " + String(ui.username) + ", ";
					//inputString += "The number is " + String(ui.num);

					break;
				case ILEDSEQS:
					Serial.println("Now handling ILEDSEQS...");
					//Read int into state and change beginSeqs boolean
					state = Serial.parseInt();

					Serial.println("Just parsed int...");

					sw = true;

					switch (state) {
						case START:
							//Set a global variable to yes
							beginSeqs = true;
							sw = true;

							//inputString += "The beginSeqs was set to true and the state will be changed to " + String(state) + "\n";
							break;
						case STOP:
							//Set a global variable to no
							beginSeqs = false;
							sw = true;
							cmd = WiFiModuleRXHandlerClass::RESUME;

							//inputString += "The beginSeqs was set to false and the state will be changed to " + String(state) + "\n";

							break;
						case RESTART:
							//Set a global variable to no
							beginSeqs = false;
							sw = true;
							cmd = WiFiModuleRXHandlerClass::RESET;

							//inputString += "The beginSeqs was set to false and the state will be changed to " + String(state) + "\n";

							break;
					}
					break;
				case UPT:
					Serial.println("Now handling UPT...");
					
					//Read int into updateTime variable for loop to call Effects Manager method for updating performance time of all strips change sw boolean
					updateTime = Serial.parseInt();

					Serial.print("UPT update performance time equals ");
					Serial.println(updateTime);

					sw = true;

					//Set command
					cmd = UPDATETIME;
					break;
				default:
					break;
			}

			//Set methFound to false
			methFound = false;

			//Empty serial buffer
			while (Serial.read() != -1);
		}
	}
}

int WiFiModuleRXHandlerClass::getCurrentCommand() {
	int iret = -1;

	switch (cmd)
	{
		case WiFiModuleRXHandlerClass::RESET:
			iret = RESTART;
			break;
		case WiFiModuleRXHandlerClass::RESUME:
			iret = START;
			break;
		default:
			iret = STOP;
			break;
	}

	return iret;
}

/*
*/
bool WiFiModuleRXHandlerClass::emptySerialString() {
	bool bret = false;

	for (int i = 0; i < strlen(serialString); i++) {
		serialString[i] = '\0';
	}

	return bret;
}