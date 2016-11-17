// 
// 
// 

#ifndef _PASSSTRUCTVIAWIFI_h
	#include "PassStructViaWiFi.h"
#endif

#ifndef wifiserver_h
	#include <WiFiServer.h>
#endif

#ifndef wificlient_h
	#include <WiFiClient.h>
#endif

#ifndef WiFi_h
	#include <ESP8266WiFi.h>
#endif

#ifndef ESP8266WEBSERVER_H
	#include <ESP8266WebServer.h>
#endif

#include "Arduino.h"

/*Setup webserver port*/
ESP8266WebServer server(80);

PassStructViaWiFiClass::PassStructViaWiFiClass(){}

void PassStructViaWiFiClass::handleClient() {
	server.handleClient();
}

/*This functions sets up valid web request URIs and reponses*/
void PassStructViaWiFiClass::setupWebURIs() {
	/*Setup Web request naming*/
	server.on("/", std::bind(&PassStructViaWiFiClass::handleRoot,this));
	server.on("/ready", std::bind(&PassStructViaWiFiClass::handleReady, this));
	server.on("/init_led_seqs", std::bind(&PassStructViaWiFiClass::handleInitLEDSeqs, this));
	server.on("/add_struct", std::bind(&PassStructViaWiFiClass::handleAddStruct,this));
	server.on("/remove_struct", std::bind(&PassStructViaWiFiClass::handleRemoveStruct,this));
	server.on("/list_structs", std::bind(&PassStructViaWiFiClass::handleListStructs,this));
	server.onNotFound(std::bind(&PassStructViaWiFiClass::handleNotFound,this));
}

void PassStructViaWiFiClass::begin() {
	/*Setup Web request naming*/
	setupWebURIs();

	/*Start server*/
	server.begin();
}

void PassStructViaWiFiClass::handleRoot(void) {
	String response = "";
	response += "<h1>Welcome to ESP8266 Struct Passing Testing</h1><br><br>";
	response += "Test using these subdirectories: /add_struct, /remove_struct, list_structs, ";
	server.send(200, "text/html", response);
}

/*
	Function handleReady
	This method will send "YES" as an http response to the calling user

	Parameters: none

	Returns: nothing to calling method, response to http user
*/

void PassStructViaWiFiClass::handleReady() {
	/*Declare variables*/
	String response = "";

	/*Verify arg READY is present*/
	if (server.hasArg(READY)) {
		String sret = READY;
		String received = "";

		/*Get passed argument*/
		received = server.arg(READY);;

		Serial.println(sret);
		Serial.println(received);

		/*Notify user ESP8266 is ready by sending YES*/
		response = "YES";

		/*Free malloc'd data*/
		//free(&dBytes);
		//free(ps);
	}
	else {
		return;
	}

	//Send response to user
	server.send(200, "text/plain", response);
}
/*
	Function handleInitLEDSeqs
	This method will send a struct to the attached microcontroller via serial interface which
	gives a signal to start LED Lighting Sequences (0 for start, 2 for restart, 1 for stop

	Parameters: none

	Returns: nothing to calling method, response to http user
*/
void PassStructViaWiFiClass::handleInitLEDSeqs() {
	/*Declare variables*/
	String response = "";

	/*Verify arg INITLEDSEQS is present*/
	if (server.hasArg(INITLEDSEQS)) {
		String sret = INITLEDSEQS;
		String received = "";
		int action = -1;

		/*Get passed argument and convert to int*/
		received = server.arg(INITLEDSEQS);
		action = received.toInt();

		//Send specific command to connected microcrontoller depending on passed action
		switch (action) {
			case START:
				received = "0";
				break;
			case STOP:
				received = "1";
				break;
			case RESTART:
				received = "2";
				break;
		}

		Serial.println(sret);
		Serial.println(received);

		/*Notify user of what data was received*/
		response = "<h1>" + sret + " - " + received + " - processed by handleInitLEDSeqs()</h1><br><br>";

		/*Free malloc'd data*/
		//free(&dBytes);
		//free(ps);
	}
	else {
		return;
	}

	//Send http response to user
	server.send(200, "text/html", response);
}

void PassStructViaWiFiClass::handleAddStruct() {
	/*Declare variables*/
	String response = "";

	/*Verify arg PINSETUPARG or CHANGEPIXELCOLORARG is present*/
	if (server.hasArg(PINSETUPARG)) {
		String sret = "";
		String received = "";

		/*Create byte array that can hold bytes coming across WiFi*/
		char *dBytes = (char*) malloc(server.arg(PINSETUPARG).length());

		/*Crreate PinSetup pointer to hold array once bytes received*/
		//PinSetup **ps = (PinSetup**)malloc(server.arg(PINSETUPARG).length());
		
		/*Copy bytes from WiFi into dBytes variable*/
		server.arg(PINSETUPARG).getBytes((unsigned char*)dBytes, (server.arg(PINSETUPARG).length()), 0U);
		received += dBytes;

		/*Create array of PinSetup structs from byte data recived*/
		//ps = (PinSetup**)dBytes;

		/*Loop until done reading from serial so we can write back to user*/
		//while (Serial.available());

		/*Loop through array and send values back to user for reference*/
		//for (int i = 0; i < (sizeof(ps)/sizeof(PinSetup)); i++) {
		//	sret = "For element " + String(i) + " we have the following values: "
		//		+ "NumPixels = " + String(ps[i]->numPixels)
		//		+ "Datapin = " + String(ps[i]->dataPin)
		//		+ "ClockPin = " + String(ps[i]->clockPin)
		//		+ "Dotstar_Brg = " + String(ps[i]->brg)
		//		+ "\r\n";
		//}

		//Serial.println(sret);
		Serial.println(received);
		/*Notify user of what data was received*/
		server.send(200, "text/plain", sret);
		server.send(200, "text/plain", received);

		/*Free malloced data*/
		//free(&dBytes);
		//free(ps);
	}
	else if (server.hasArg(CHANGEPIXELCOLORARG)) {
		
	}

	response += "<h1>In Add Struct Handler</h1><br><br>";
	server.send(200, "text/html", response);
}

void PassStructViaWiFiClass::handleRemoveStruct() {
	String response = "";
	response += "<h1>In Remove Struct Handler</h1><br><br>";
	server.send(200, "text/html", response);
}

void PassStructViaWiFiClass::handleListStructs() {
	String response = "";
	response += "<h1>In List Structs Handler</h1><br><br>";
	server.send(200, "text/html", response);
}

void PassStructViaWiFiClass::handleNotFound() {
	String response = "";
	response += "<h1>In Not Found Handler</h1><br><br>";
	server.send(200, "text/html", response);
}

void  PassStructViaWiFiClass::Add_Struct(PinSetup **ps) {

}

void  PassStructViaWiFiClass::Add_Struct(ChangePixelColor **cpc) {

}

void  PassStructViaWiFiClass::Remove_Struct(PinSetup *ps) {

}

void  PassStructViaWiFiClass::Remove_Struct(ChangePixelColor *cpc) {

}

void  PassStructViaWiFiClass::Replace_Struct(PinSetup *ps, PinSetup *nps) {

}

void  PassStructViaWiFiClass::Replace_Struct(ChangePixelColor *cpc, ChangePixelColor *ncpc) {

}

void  PassStructViaWiFiClass::List_Structs() {

}


PassStructViaWiFiClass PassStructViaWiFi;

