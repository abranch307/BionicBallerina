#pragma once

/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created:
	Last Modified:
	Class: PassStructViaWiFi.h
	Class Description:
		This class creates Mock objects of methods from PassStructViaWiFi class to do
		unit tests against
*/

/*
*/
struct URIParamArgReturn {
	char* uri;
	char* param;
	char* arg;
};

class PSVW_MockObject
{
	protected:

	public:
		PSVW_MockObject();
		~PSVW_MockObject();
		bool begin_Test(const char* SSID, const char* PassPhrase, const char* ESPSuffix);
		bool handleReady(char URI[]);
		bool handleInitLEDSeqs(char URI[]);

	private:
		URIParamArgReturn* returnUriParamArg(char URI[]);
		const char *ssid = "DOTSTARCOMPOSER", *passphrase = "dotstar1234", *espID = "ESP_C102B", *espSuffix = "C102B";
};

