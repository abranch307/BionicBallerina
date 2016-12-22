// PassStructViaWiFi.h

/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created: 
	Last Modified: 
	Class: PassStructViaWiFi.h
	Class Description:
		This class defines URIs, http parameters & arguments used/expected when handling HTTP requests.  Also defined
		are structs and methods used to setup a Webserver on ESP8266 and handle passing information/commands from WiFi 
		to the serial interface
*/

#ifndef _PASSSTRUCTVIAWIFI_h
#define _PASSSTRUCTVIAWIFI_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#ifndef wifiserver_h
	#include <WiFiServer.h>
#endif

#ifndef wificlient_h
	#include <WiFiClient.h>
#endif

#ifndef ESP8266WEBSERVER_H
	#include <ESP8266WebServer.h>
#endif

#ifndef WiFi_h
	#include <ESP8266WiFi.h>
#endif

#ifndef ESP8266MDNS_H
	#include <ESP8266mDNS.h>
#endif

//Define http parameters and args
#define INITLEDSEQS "INITLEDSEQS"
#define UPDATEPERFORMANCETIME "UPT"
#define READY "READY"
#define ACTION "ACTION"
#define START 0
#define STOP 1
#define RESTART 2


class PassStructViaWiFiClass
{
 protected:


 public:
	PassStructViaWiFiClass();
	bool begin();
	bool handleClient();

 private:
	bool setupWebURIs();
	bool handleRoot(void);
	bool handleReady();
	bool handleInitLEDSeqs();
	bool handleUpdatePerformanceTime();
	bool handleNotFound();

	/*Setup ssid & password*/
	//const char *ssid = "ATT9i5Q6AE";
	//const char *password = "6ve+g+vew3bf";

	const char *ssid = "DOTSTARCOMPOSER";
	const char *password = "dotstar1234";

	//const char *ssid = "linksys";
	//const char *password = "user1234";

	char hostString[16] = { 0 };//Define DNS Hostname
};

extern PassStructViaWiFiClass PassStructViaWiFi;

#endif

