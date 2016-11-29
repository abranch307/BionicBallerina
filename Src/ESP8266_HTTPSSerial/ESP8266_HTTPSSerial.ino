/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Class: ESP8266_HTTPSSerial.ino
	Class Description:
		This is the main file for this program.  It handles the setup of variables and configurations for the ESP8266
		and the events that happen during the infite loop.  Setup includes; initializing helper classes, SSID
		and passphrase for connecting to Access Point, starting serial communication, and use helper classes to
		handle http requests which in turn will send received commands through the serial interface.
*/

#include <DNSServer.h>
#include "PassStructViaWiFi.h"

/*Setup webserver handling class*/
PassStructViaWiFiClass psvwc;

void setup()
{
	/*Initialize serial interface*/
	Serial.begin(115200);

	/*Setup web request URIs and responses*/
	psvwc.begin();

	/*Notify developer HTTP server has started successfully*/
	//Serial.println("HTTP server started");
}

void loop()
{
	/*Handle client requests*/
	psvwc.handleClient();
}