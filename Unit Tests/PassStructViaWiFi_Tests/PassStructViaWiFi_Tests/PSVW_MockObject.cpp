/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created:
	Last Modified:
	Class: PassStructViaWiFi.h
	Class Description:
		This class creates Mock objects of methods from PassStructViaWiFi class to do
		unit tests against
*/

#include "stdafx.h"
#include "PSVW_MockObject.h"
#include <string>

PSVW_MockObject::PSVW_MockObject(){}

PSVW_MockObject::~PSVW_MockObject(){}

/*
	Mock object that imitates the PassStructViaWiFi's begin method.  This mock object
	verifies passed ssid and passphrase match default ssid and passphrase.  The mock object also
	simulates the concatenation of ESP_ with ESP8266's chip id to form the hostname that user will
	eventually see
*/
bool PSVW_MockObject::begin_Test(const char* SSID, const char* PassPhrase, const char* ESPSuffix) {
	//Compare variables
	bool bret = false;
	char hostString[16] = { 0 };//Define DNS Hostname
	
	//Verify sprintf combines prefix and suffix correctly
	sprintf(hostString, "ESP_%s", ESPSuffix);
	if (strcmp(hostString, espID) != 0) {
		return bret;
	}

	//Verify passed SSID is equal to default
	if (strcmp(SSID, ssid) != 0) {
		return bret;
	}

	//Verify passed PassPhrase is equal to default
	if (strcmp(PassPhrase, passphrase) != 0) {
		return bret;
	}

	bret = true;
	return bret;
}

/*
	Mock object that imitates the PassStructViaWiFi's handleReady method.  This mock object
	verifies passed URI has valid Uri, Param, and Args.
*/
bool PSVW_MockObject::handleReady(char URI[]) {
	//Declare variables
	URIParamArgReturn* upar = returnUriParamArg(URI);
	bool bret = true;

	//Verify Uri is valid
	if (strcmp(upar->uri, "ready") != 0) {
		bret = false;
	}

	//Verify Parameter is valid
	if (strcmp(upar->param, "READY") != 0) {
		bret = false;
	}

	//Verify Arg is valid
	if (strcmp(upar->arg, "Y") != 0) {
		bret = false;
	}

	free(upar);
	return bret;
}

/*
	Mock object that imitates the PassStructViaWiFi's handleInitLEDSeqs method.  This mock object
	verifies passed URI has valid Uri, Param, and Args.
*/
bool PSVW_MockObject::handleInitLEDSeqs(char URI[]) {
	//Declare variables
	URIParamArgReturn* upar = returnUriParamArg(URI);
	bool bret = true;

	//Verify Uri is valid
	if (strcmp(upar->uri, "init_led_seq") != 0) {
		bret = false;
	}

	//Verify Parameter is valid
	if (strcmp(upar->param, "INITLEDSEQ") != 0) {
		bret = false;
	}

	//Verify Arg is valid
	if (strcmp(upar->arg, "0") != 0) {
		bret = false;
	}

	free(upar);
	return bret;
}

/*
	Method: returnUriParamArg
		This method receives a URI (ex. http://192.168.0.101/init_led_seq?INITLEDSEQ=0) and parses
		the last uri (init_led_seq), parameter (INITLEDSEQ), and arg (0) and returns a struct which
		holds all three values as strings

	Parameters: URI - string in the format of an http request string

	Returns: URIParamArgReturn* - a struct which holds the found uri, parameter, and arg in separate char* variables
*/
URIParamArgReturn* PSVW_MockObject::returnUriParamArg(char URI[]) {
	//Declare variables
	URIParamArgReturn* upar = (URIParamArgReturn*)calloc(1, sizeof(URIParamArgReturn));
	char *token, *SavePtr, *tString = (char*)calloc(100, sizeof(char));//Hold token and string placement from strtok_r
	rsize_t strmax = strlen(URI);

	//Set first token  and copy into temoprary string
	token = strtok_s(URI, "/", &SavePtr);
	strcpy(tString, token);

	//Loop through tokens until last forward slash is found
	while ((token = strtok_s(NULL, "/", &SavePtr)) != NULL)
	{
		//Free temp string then reset
		strcpy(tString, token);
	 }

	//Split last part of uri into uri, parameter, and arg

	//Get uri and set in struct
	token = strtok_s(tString, "?", &SavePtr);
	upar->uri = (char*)calloc(strlen(token), sizeof(char));
	strcpy(upar->uri, token);

	//Get param and arg and set in struct
	token = strtok_s(NULL, "?", &SavePtr);
	strcpy(tString, token);

	//Set param
	token = strtok_s(tString, "=", &SavePtr);
	upar->param = (char*)calloc(strlen(token), sizeof(char));
	strcpy(upar->param, token);

	//Set arg
	token = strtok_s(NULL, "?", &SavePtr);
	upar->arg = (char*)calloc(strlen(token), sizeof(char));
	strcpy(upar->arg, token);

	return upar;
}