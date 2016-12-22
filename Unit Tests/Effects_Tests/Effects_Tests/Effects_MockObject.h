#pragma once

// Effects.h

/*
Author: Aaron Branch, Zach Jarmon, Peter Martinez
Created:
Class:
Class Description:

*/


#ifndef _STRUCTS_h
#include "../../../Unit Tests/Adafruit_DotStar/Structs.h"
#endif

//Colors
#define CLEAR	0x000000
#define WHITE	0xFFFFFF
#define RED		0xFF0000
#define GREEN	0x00FF00
#define BLUE	0x0000FF
#define YELLOW	0xFFFF00
#define CYAN	0x00FFFF
#define MAGENTA	0xFF00FF
#define ORANGE  0xFFA500

//Lighting Effect Types
#define ALLCLEAR 0
#define RAINBOW 1
#define LOADCOLOR 2
#define BOUNCEBACK 3
#define FLOWTHROUGH 4

class Effects_MockObject {

public:
	static bool allClear_Test( LightingSequence* lseqs, uint16_t currentSequence);
	static bool loadColor_Test(LightingSequence* lseqs, uint16_t currentSequence, uint16_t shiftPixelToLoad);
	static bool bounceBack_Test(LightingSequence* lseqs, uint16_t currentSequence, bool* init, bool* forward, int16_t *shiftPixelsBy, int16_t* tail, int16_t* head, uint16_t* bounces, uint16_t initHead, uint16_t initTail);
	static bool flowThrough_Test(LightingSequence* lseqs, uint16_t currentSequence, int16_t* i, bool isRainbow, int16_t* p0, int16_t* p1, int16_t* p2, int16_t* p3, int16_t* p4, int16_t* p5, int16_t* virtualPixelIndexArray);
	static bool setSinglePixelColor_Test( LightingSequence* lseqs, uint16_t pixelElem, uint16_t color);
	static bool updateBrightness_Test(LightingSequence* lseqs, uint16_t currentSequence);
	static bool getNextCommaDelimitedColorToken_Test(const char* String, uint16_t BegPosition);
	static uint16_t getHeadTailofLED_Test(const char* Type, const char* Colors, uint16_t TotalPixels);
	static CommaDelimitReturn cdrColor;
	static int theStrip[3];
private:
	//Disallow creating instance of object
	Effects_MockObject() {};
protected:

};