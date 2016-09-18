// PassStructViaWiFi.h

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

#define PINSETUPARG "PINSETUP"
#define CHANGEPIXELCOLORARG "CHANGEPIXELCOLOR"

typedef struct {
	uint16_t numPixels; //Number of dotstar pixels linked
	uint8_t dataPin; //Data pin for one dotstar setup
	uint8_t clockPin; //Clock pin for one dotstar setup
	uint8_t brg; //Coloring arrangment DOTSTAR_BRG, ...
}PinSetup;

typedef struct {
	uint16_t pixelIndex; //Which pixel to setup
	uint32_t color; //What color to change to; 0xFF0000 = red, 0x00FF00 = ?, 0x0000FF = ?
}ChangePixelColor;

class PassStructViaWiFiClass
{
 protected:


 public:
	PassStructViaWiFiClass();
	void begin();
	void handleClient();
	void finishedProcessingData(String clientReply);

 private:
	void setupWebURIs();
	void handleRoot(void);
	void handleAddStruct();
	void handleRemoveStruct();
	void handleListStructs();
	void handleNotFound();
	void Add_Struct(PinSetup **ps);
	void Add_Struct(ChangePixelColor **cpc);
	void Remove_Struct(PinSetup *ps);
	void Remove_Struct(ChangePixelColor *cpc);
	void Replace_Struct(PinSetup *ps, PinSetup *nps);
	void Replace_Struct(ChangePixelColor *cpc, ChangePixelColor *ncpc);
	void List_Structs(void);
};

extern PassStructViaWiFiClass PassStructViaWiFi;

#endif

