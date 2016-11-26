// WiFiModuleRXHandler.h

/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created:
	Class:
	Class Description:

*/

#ifndef _WIFIMODULERXHANDLER_h
#define _WIFIMODULERXHANDLER_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#define PROTRINKET5V 0
#define PINSETUPARG "PINSETUP"
#define CHANGEPIXELCOLORARG "CHANGEPIXELCOLOR"
#define INITLEDSEQS "INITLEDSEQS"
#define USERINFORECV "USERINFO"
#define LEDSWITCH "LED"
#define START 0
#define STOP 1
#define RESTART 2

class WiFiModuleRXHandlerClass
{
	protected:

	public:
		String inputString;         // a string to hold incoming data
		boolean stringComplete = false;  // whether the string is complete
		boolean beginSeqs = false;
		boolean sw = false;
		enum UserMethods { LED, USERINFO, ILEDSEQS };
		enum Command { RESET, RESUME };
		Command cmd;
		WiFiModuleRXHandlerClass();
		void RXHandler();
		void LoopHandle();
		int ESP8266CH_POn(int PinTurnOn);
		int ESP8266CH_POff(int PinTurnOn);
		int getCurrentCommand();
	private:
		//Setup struct for userinformation
		struct userInfo {
			char username[20];
			int num;
		};
};

extern WiFiModuleRXHandlerClass WiFiModuleRXHandler;

#endif

