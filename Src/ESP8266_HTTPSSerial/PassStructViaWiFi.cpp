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
	#include "PassStructViaWiFi.h"
#endif

ESP8266WebServer server(80);//Setup webserver port

/*Blank default constructor*/
PassStructViaWiFiClass::PassStructViaWiFiClass(){}

/*
	Method: setupWebURIs
	This method sets up valid user-defined web request URIs that the webserver will accept and handle

	Parameters: None

	Returns: bool - true if successful when setting up URIs, false if unsuccessful when setting up URIs
*/
bool PassStructViaWiFiClass::setupWebURIs() {
	bool bret = false;

	__try {
		/*Setup Web request URIs*/
		server.on("/", std::bind(&PassStructViaWiFiClass::handleRoot, this));
		server.on("/ready", std::bind(&PassStructViaWiFiClass::handleReady, this));
		server.on("/init_led_seqs", std::bind(&PassStructViaWiFiClass::handleInitLEDSeqs, this));
		server.on("/update_performance_time", std::bind(&PassStructViaWiFiClass::handleUpdatePerformanceTime, this));
		server.onNotFound(std::bind(&PassStructViaWiFiClass::handleNotFound, this));

		bret = true;
	}
	__catch(const std::exception& e) {
		//Do nothing - bret is left as false
	}

	return bret;
}

/*
	Method begin:
		This method will initialize hostname using ESP_ + chipID, connect to WiFi Access Point with specified 
		ssid and passphrase designated in header file, setup accepted web URIs, and start web server 
	Parameters: None

	Returns: bool - true if web uris setup & web server started successcully, false if either uris setup or web server
		start fails
*/
bool PassStructViaWiFiClass::begin() {
	//Declare variables
	bool bret = true;

	/*Set hostname*/
	sprintf(hostString, "ESP_%06X", ESP.getChipId());
	//WiFi.hostname(hostString);

	/*Connect to AP*/
	WiFi.begin(ssid, password);
	//Serial.println("");

	/*Wait for connection*/
	while (WiFi.status() != WL_CONNECTED) {
		delay(500);
		//Serial.println(".");
	}

	/*Display connection info*/
	/*Serial.println("");
	Serial.print("Connected to ");
	Serial.println(ssid);
	Serial.print("IP address: ");
	Serial.println(WiFi.localIP());
	Serial.print("Hostname: ");
	Serial.println(WiFi.hostname());*/

	/*Setup Web request URIs*/
	if (!setupWebURIs()) {
		//Serial.println("Error setting up Web URIs...");
		bret = false;
	}

	__try {
		/*Start server*/
		server.begin();
	}
	__catch (const std::exception& e) {
		//Set bret to false
		bret = false;
	}

	return bret;
}

/*
	Method: handleClient
	This method will call the defined webserver's handle client method, which looks for incoming
	http requests and client connections

	Parameters: none

	Returns: bool - false if call to webserver client handle fails, true if call to webserver client handle is successful
*/
bool PassStructViaWiFiClass::handleClient() {
	bool bret = false;

	__try {
		//Call webserver's client handling method
		server.handleClient();

		bret = true;
	}
	__catch(const std::exception& e) {
		//Do nothing - bret is left as false
	}

	return bret;
}

/*
	Method handleRoot:
		Sends http response in the form of text/html when user navigates to http://ipaddress:80/ that describes
		the different uris, parameters, and args used in each

	Parameter: none

	Returns: bool - true if successful in sending response, false if unsuccessful in sending response
*/
bool PassStructViaWiFiClass::handleRoot(void) {
	bool bret = false;

	__try {
		String response = "";
		response += "<h1>Welcome to ESP8266 Struct Passing Testing</h1><br><br>";
		response += "Test using these sub-uris: /add_struct, /remove_struct, list_structs, /ready?READY=(Y), /init_led_seqs?INITLEDSEQS=(0-Start)(1-Stop)(2-Restart)";
		server.send(200, "text/html", response);

		bret = true;
	}
	__catch (const std::exception& e) {
		//Serial.println(e.what());
	}

	return bret;
}

/*
	Method handleRoot:
		Sends http response in the form of text/html when user navigates to an invalid uri on
		http://ipaddress:80/

	Parameter: none

	Returns: bool - true if successful in sending response, false if unsuccessful in sending response
*/
bool PassStructViaWiFiClass::handleNotFound() {
	//Declare variables
	String response = "";
	bool bret = false;

	__try {
		//Send respone to user
		response += "<h1>In Not Found Handler</h1><br><br>";
		server.send(200, "text/html", response);

		bret = true;
	}
	__catch (const std::exception& e) {

	}
}

/*
	Method handleReady
		This method will send "YES" as an http response to the calling user and attached microcontroller
		via serial interface

	Parameters: none

	Returns: bool - true if necessary parameter and arg are found, false if paramter or necessary arg are not found,
		or if sending response fails
*/
bool PassStructViaWiFiClass::handleReady() {
	/*Declare variables*/
	String response = "";
	bool bret = false;

	__try {
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

			//Send response to user
			server.send(200, "text/plain", response);
		}
	}
	__catch (const std::exception& e) {
		//Serial.println(e.what());
	}

	return bret;
}
/*
	Function handleInitLEDSeqs
		This method will send commands to the attached microcontroller via serial interface which
		gives a signal to start/stop/restart LED Lighting Sequences 
		(0 for start, 1 for stop - which pauses LED performance, 
		2 for restart - which stops and restarts performance to the beginning).  It also sends
		a response back to the user in the html form which echoes what parameter and arg was received

	Parameters: none

	Returns: true if necessary parameter and arg are found, false if paramter or necessary arg are not found,
		or if sending response fails
*/
bool PassStructViaWiFiClass::handleInitLEDSeqs() {
	/*Declare variables*/
	String response = "";
	bool bret = false;

	__try {
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

			//Send http response to user
			server.send(200, "text/html", response);

			bret = true;
		}
	}
	__catch (const std::exception& e) {
		//Serial.println(e.what());
	}

	return bret;
}

/*
	Function handleUpdatePerformanceTime
		This method will send commands to the attached microcontroller via serial interface which
		gives a signal to update the Effect Manager's performance time to specified value.  It also sends
		a response back to the user in the html form which echoes what parameter and arg was received

	Parameters: none

	Returns: true if necessary parameter and arg are found, false if paramter or necessary arg are not found,
	or if sending response fails
*/
bool PassStructViaWiFiClass::handleUpdatePerformanceTime() {
	/*Declare variables*/
	String response = "";
	bool bret = false;

	__try {
		/*Verify arg UPT is present*/
		if (server.hasArg(UPDATEPERFORMANCETIME)) {
			String sret = UPDATEPERFORMANCETIME;
			String received = "";

			/*Get passed argument and convert to int*/
			received = server.arg(UPDATEPERFORMANCETIME);

			//Send specific command to connected microcrontoller depending on passed action
			Serial.println(sret);
			Serial.println(received);

			/*Notify user of what data was received*/
			response = "<h1>" + sret + " - " + received + " - processed by handleUpdatePerformanceTime()</h1><br><br>";

			//Send http response to user
			server.send(200, "text/html", response);

			bret = true;
		}
	}
	__catch(const std::exception& e) {
		//Serial.println(e.what());
	}

	return bret;
}

//PassStructViaWiFiClass PassStructViaWiFi;

