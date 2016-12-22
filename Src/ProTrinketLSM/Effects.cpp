/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created:
	Last Modified: 12/16/2016
	Class: Effects.cpp
	Class Description: Methods within are pre programmed effects that the strips of LEDs can implement. Any method below
		could be implemented just with the setSinglePixelColor method, but for ease more are provided.
	

*/ 

#ifndef _EFFECTS_h
	#include "Effects.h"
#endif

#include <string.h>

CommaDelimitReturn Effects::cdrColor;

/*
	Method: allClear
	Parameters: strip: Adafruit_DotStar*, lseqs: LightingSequence*, currentSequence: uint16_t, PassedStripClass: Strip*
	Return: void
	Description: Turns all LEDs off on the given strip. 
	
*/
void Effects::allClear(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t currentSequence, Strip* PassedStripClass) {
	//Exit if invalid pointers passed
	if (strip == NULL) {
		return;
	}
	else if (lseqs == NULL) 
	{
		return;
	}

	//Exit if pixel colors is null
	if (lseqs[currentSequence].colors == NULL) {
		return;
	}

	//Loop through all pixels and set color to clear
	for (int i = 0; i < lseqs[currentSequence].totalPixels; i++) {
		setSinglePixelColor(strip, lseqs, i, 0);
	}

	//Show new effects
	strip->show();
}
/*
	Method: loadColor
	Parameters: strip: Adafruit_DotStar*, lseqs: LightingSequence*, currentSequence: uint16_t, shiftPixelToLoad: uint16_t, PassedStripClass: Strip*
	Return: void
	Description: Sets a group of pixels to values given in PassedStripClass's color field, in positions specified by PassedStripClass.
	
*/
void Effects::loadColor(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t currentSequence, uint16_t shiftPixelToLoad, Strip* PassedStripClass) {
	//Exit if invalid pointers passed
	if (strip == NULL) {
		return;
	}
	else if (lseqs == NULL)
	{
		return;
	}

	//Exit if pixelElems or colorElems is null
	if (lseqs[currentSequence].colors == NULL) {
		return;
	}

	//Declare variables
	int elem = 0, colorValue = 0;//, tot = lseqs[currentSequence].totalPixels;
	
	//Reset nextBegPositions then continue
	cdrColor.nextBegPosition = 0;

	for (elem = 0; elem < lseqs[currentSequence].totalPixels; elem++) {
		//Set nextBegPoistion for colors and get next color
		getNextCommaDelimitedColorToken(lseqs[currentSequence].colors, cdrColor.nextBegPosition);

		//Convert return color index to actual color
		setSinglePixelColor(strip, lseqs, (elem + shiftPixelToLoad), cdrColor.value);
	}

	//Show new effects
	strip->show();
}
/*
	Method: bounceBack
	Parameters: strip: Adafruit_DotStar*, lseqs: LightingSequence*, currentSequence: uint16_t, init: bool*, forward: bool*, shiftPixelsBy: int16_t, tail: int16_t, head: int16_t, bounces: int16_t, initHead: uint16_t, initTail: uint16_t, PassedStripClass: Strip*
	Return: void
	Description: Given positions in PassedStripClass, take a block of LEDs and shift them forwards along the strip until they hit the end, then shift them backward until they hit the forward. 
	
*/
void Effects::bounceBack(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t currentSequence, bool* init, bool* forward, int16_t *shiftPixelsBy, int16_t* tail, int16_t* head, uint16_t* bounces, uint16_t initHead, uint16_t initTail, Strip* PassedStripClass) {
	//Exit if invalid pointers passed
	if (strip == NULL) {
		return;
	}
	else if (lseqs == NULL)
	{
		return;
	}

	//Initilialize if first time in
	if (*init) {
		//Simply set colors, setup values, and exit
		loadColor(strip, lseqs, currentSequence, 0, PassedStripClass);

		*init = false;
		*bounces = lseqs[currentSequence].bounces;
		*shiftPixelsBy = 0;
		*head = getHeadTailofLED("HEAD", lseqs[currentSequence].colors, lseqs[currentSequence].totalPixels);
		*tail = getHeadTailofLED("TAIL", lseqs[currentSequence].colors, lseqs[currentSequence].totalPixels);

		return;
	}

	if (*bounces > 0) {
		if (*forward) {
			//Add 1 to shiftPixelsBy
			*shiftPixelsBy = *shiftPixelsBy + 1;
			*head = *head + 1;
			*tail = *tail + 1;

			if (*head >= lseqs[currentSequence].totalPixels)
			{
				*forward = false;
				*bounces = *bounces + 1;
				*shiftPixelsBy = *shiftPixelsBy - 1;
				*head = *head - 1;
				*tail = *tail - 1;
			}

			//Load pixel colors
			loadColor(strip, lseqs, currentSequence, *shiftPixelsBy, PassedStripClass);
		}
		else {
			//Subtract 1 from shiftPixelsBy
			*shiftPixelsBy = *shiftPixelsBy - 1;
			*head = *head - 1;
			*tail = *tail - 1;

			if (*tail < 0)
			{
				*forward = true;
				*bounces = *bounces + 1;
				*shiftPixelsBy = *shiftPixelsBy + 1;
				*head = *head + 1;
				*tail = *tail + 1;
			}

			//Load pixel colors
			loadColor(strip, lseqs, currentSequence, *shiftPixelsBy, PassedStripClass);
		}
	}

	//Show effect on led
	strip->show();
}
/*
	Method: flowThrough
	Parameters: strip: Adafruit_DotStar*, lseqs: LightingSequence*, currentSequence: uint16_t, i: int16_t*, isRainbow: bool, p0: int16_t, p1: int16_t, p2: int16_t, p3: int16_t, p4: int16_t, p5: int16_t, virtualPixelIndexArray: int16_t, PassedStripClass: Strip*
	Return: void
	Description: Given colors and positions in PassedStripClass, take a sequence of LEDs, and move them forward in the strip. Once the end is reached, move each pixel one by one to the beginning of the strip. 
	
*/
void Effects::flowThrough(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t currentSequence, int16_t* i, bool isRainbow, int16_t* p0, int16_t* p1, int16_t* p2, int16_t* p3, int16_t* p4, int16_t* p5, int16_t* virtualPixelIndexArray, Strip* PassedStripClass) {
	//Exit if invalid pointers passed
	if (strip == NULL) {
		return;
	}
	else if (lseqs == NULL)
	{
		return;
	}

	//Declare variables
	int elem = 0, colorValue = 0;

	//Add 1 to i if i is -1 and reset ps
	if (*i < 0) {
		//Add 1 to i making i = 0
		*i = *i + 1;

		if (isRainbow) {
			//Reset p values
			*p0 = -1, *p1 = 0, *p2 = 1, *p3 = 2, *p4 = 3, *p5 = 4;
		}
		else {
			//Reset nextBegPositions then continue
			cdrColor.nextBegPosition = 0;

			//Set virual pixel elements to default element indexes
			for (elem = 0; elem < lseqs[currentSequence].totalPixels; elem++) {
				//Add 1 to pixel's shift value
				virtualPixelIndexArray[elem] = elem - 1;
			}
		}
	}

	if (isRainbow) {
		//Verify if p0 is greater than numPixels1, and if so add 1 to shiftPixelsBy
		if (*p0 >= lseqs[currentSequence].totalPixels) {
			//Add 1 to shiftPixelsBy and reset j
			*i = *i + 1;

			//Reset p values
			//*p0 = -1, *p1 = 0, *p2 = 1, *p3 = 2, *p4 = 3, *p5 = 4;
		}

		if (*i <= lseqs[currentSequence].iterations) {
			strip->setPixelColor((*p0)++, CLEAR);
			strip->setPixelColor((*p1)++, RED);
			strip->setPixelColor((*p2)++, ORANGE);
			strip->setPixelColor((*p3)++, YELLOW);
			strip->setPixelColor((*p4)++, GREEN);
			strip->setPixelColor((*p5)++, BLUE);
			//strip->setPixelColor(*p1 - 2, CLEAR);

			if (*p0 >= lseqs[currentSequence].totalPixels)
			{
				*p0 = 0;
			}
			if (*p1 >= lseqs[currentSequence].totalPixels)
			{
				*p1 = 0;
			}
			if (*p2 >= lseqs[currentSequence].totalPixels)
			{
				*p2 = 0;
			}
			if (*p3 >= lseqs[currentSequence].totalPixels)
			{
				*p3 = 0;
			}
			if (*p4 >= lseqs[currentSequence].totalPixels)
			{
				*p4 = 0;
			}
			if (*p5 >= lseqs[currentSequence].totalPixels)
			{
				*p5 = 0;
			}
		}
	}
	else {
		//Verify if first element is greater than total pixels, and if so add 1 to shiftPixelsBy
		if (virtualPixelIndexArray[0] >= lseqs[currentSequence].totalPixels) {
			//Add 1 to shiftPixelsBy and reset j
			*i = *i + 1;
		}

		if (*i <= lseqs[currentSequence].iterations) {
			//Reset nextBegPositions then continue
			cdrColor.nextBegPosition = 0;

			for (elem = 0; elem < lseqs[currentSequence].totalPixels; elem++) {
				//Add 1 to pixel's shift value
				virtualPixelIndexArray[elem] = virtualPixelIndexArray[elem] + 1;

				//Verify pixel shift is not over end of led strip
				if (virtualPixelIndexArray[elem] >= lseqs[currentSequence].totalPixels) {
					//Change virtual index to 0
					virtualPixelIndexArray[elem] = 0;
				}

				//Set nextBegPoistion for colors and get next color
				getNextCommaDelimitedColorToken(lseqs[currentSequence].colors, cdrColor.nextBegPosition);

				//Set pixel color
				setSinglePixelColor(strip, lseqs, virtualPixelIndexArray[elem], cdrColor.value);
			}
		}
		else {
			allClear(strip, lseqs, currentSequence, PassedStripClass);
		}
	}

	strip->show();
}

/*
	Method: setSinglePixelColor
	Parameters: strip: Adafruit_DotStar*, lseqs: LightingSequence*, pixelElem: uint16_t, color: uint16_t
	Return: void
	Description: set a single pixel to a color
	
*/
void Effects::setSinglePixelColor(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t pixelElem, uint16_t color) {
	//Exit if invalid pointers passed
	if (strip == NULL) {
		return;
	}
	else if (lseqs == NULL)
	{
		return;
	}

	//Convert return color index to actual color
	switch (color)
	{
	case 0:
		strip->setPixelColor(pixelElem, CLEAR);
		break;
	case 1:
		strip->setPixelColor(pixelElem, WHITE);
		break;
	case 2:
		strip->setPixelColor(pixelElem, RED);
		break;
	case 3:
		strip->setPixelColor(pixelElem, GREEN);
		break;
	case 4:
		strip->setPixelColor(pixelElem, BLUE);
		break;
	case 5:
		strip->setPixelColor(pixelElem, YELLOW);
		break;
	case 6:
		strip->setPixelColor(pixelElem, CYAN);
		break;
	case 7:
		strip->setPixelColor(pixelElem, MAGENTA);
		break;
	case 8:
		strip->setPixelColor(pixelElem, ORANGE);
		break;
	default:
		strip->setPixelColor(pixelElem, CLEAR);
		break;
	}
}

/*
	Method: updateBrightness
	Parameters: strip: Adafruit_DotStar*, lseqs: LightingSequence*, uint16_t: currentSequence
	Return: void
	Description: If the incrBrightness variable is not zero, increment or decrement brightness by that value, otherwise set to default brightness specified in sequence.
				 If max brightness or minimum brightness is reached while using incrBrightness, multiply incrBrightness by -1.
	
*/
void Effects::updateBrightness(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t currentSequence) {
	//Update brightness if necessary
	if (lseqs[currentSequence].incrBrightness != 0) {
		//Add incr to brightness
		lseqs[currentSequence].brightness = lseqs[currentSequence].brightness + lseqs[currentSequence].incrBrightness;

		if (lseqs[currentSequence].brightness > 255) {
			lseqs[currentSequence].brightness = 255;
			lseqs[currentSequence].incrBrightness = lseqs[currentSequence].incrBrightness * -1;
		}
		else if (lseqs[currentSequence].brightness < 0) {
			lseqs[currentSequence].brightness = 0;
			lseqs[currentSequence].incrBrightness = lseqs[currentSequence].incrBrightness * -1;
		}

		//Set brightness
		strip->setBrightness(lseqs[currentSequence].brightness);
	}
	else {
		//Set brightness
		strip->setBrightness(lseqs[currentSequence].brightness);
	}
}

/*
	Method: getNextCommaDelimitedColorToken
	Parameters: String: const char*, BegPosition: uint16_t
	Return: void
	Description: split a given string into color/position pairs. 
	
*/
void Effects::getNextCommaDelimitedColorToken(const char* String, uint16_t BegPosition) {
	//Declare variables
	CommaDelimitReturn iret;
	char charAccum[5] = { '\0', '\0', '\0', '\0', '\0' };
	uint16_t elem = -1;

	//Set default return values
	iret.nextBegPosition = -1;
	iret.value = -1;

	//Loop through all pixel elements numbers
	for (int i = BegPosition; i < strlen(String); i++)
	{
		//If we reach a comma, turn accumulated chars into a number and set that pixel to clear
		if (String[i] == ',') {
			//Set number value to next beginning
			iret.nextBegPosition = ++i;
			iret.value = atoi(charAccum);

			break;
		}
		else {
			//Verify if this will be last iteration
			if (i == (strlen(String) - 1)) {
				//Add 1 to elem
				elem++;

				//Add char to position in numAccum array
				charAccum[elem] = String[i];

				//Set number value to next beginning
				iret.nextBegPosition = i + 1;
				iret.value = atoi(charAccum);

				break;
			}
			else {
				//Continue accumulating chars to String

				//Add 1 to elem
				elem++;

				//Add char to position in numAccum array
				charAccum[elem] = String[i];
			}
		}
	}

	cdrColor.nextBegPosition = iret.nextBegPosition;
	cdrColor.value = iret.value;
}
/*
	Method: getHeadTailofLED
	Parameters: Type: const char *, Colors: const char *, TotalPixels: uint16_t
	Return: tailOrHead: uint16_t
	Description: determine the head or tail (depending on when called) of the block of LEds in a sequence
	
*/
uint16_t Effects::getHeadTailofLED(const char* Type, const char* Colors, uint16_t TotalPixels) {
	//Declare variables
	int elem = 0, tailOrHead = -1;

	//Exit if lseq is null or colors is null
	if (Colors == NULL) {
		return tailOrHead;
	}

	//Set nextBegPosition to 0
	cdrColor.nextBegPosition = 0;

	for (elem = 0; elem < TotalPixels; elem++) {
		//Get next color token from comma delimited string
		getNextCommaDelimitedColorToken(Colors, cdrColor.nextBegPosition);

		if (cdrColor.value != 0) {
			tailOrHead = elem;

			if (strcmp(Type, "TAIL") == 0) {
				break;
			}
		}
	}

	return tailOrHead;
}
