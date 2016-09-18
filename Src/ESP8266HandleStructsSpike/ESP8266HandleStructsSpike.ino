/**/
#include "PassStructViaWiFi.h"
#include <ESP8266mDNS.h>
#include <WiFiClient.h>
#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>

/*Setup ssid & password*/
const char *ssid = "ATT9i5Q6AE";
const char *password = "6ve+g+vew3bf";

/*Setup webserver handling class*/
PassStructViaWiFiClass psvwc;

/*Setup name of this device*/
const char *espName = "ESP8266-StructTest";

void setup()
{
	/*Initialize serial interface*/
	Serial.begin(115200);

	/*Connect to AP*/
	WiFi.begin(ssid, password);
	Serial.println("");

	/*Wait for connection*/
	while (WiFi.status() != WL_CONNECTED) {
		delay(500);
		Serial.println(".");
	}

	/*Display connection info*/
	Serial.println("");
	Serial.print("Connected to ");
	Serial.println(ssid);
	Serial.println(" IP address: ");
	Serial.println(WiFi.localIP());

	/*Setup dns name for easy finding of this micro controller*/
	if (MDNS.begin(espName)) {
		Serial.print("MDNS Responder started and name is ");
		Serial.println(espName);
	}

	/*Setup web request URIs and responses*/
	psvwc.begin();

	/*Notify developer HTTP server has started successfully*/
	Serial.println("HTTP server started");
}

void loop()
{
	/*Handle client requests*/
	psvwc.handleClient();
}