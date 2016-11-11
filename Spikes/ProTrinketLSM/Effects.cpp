// 
// 
// 

#ifndef _EFFECTS_h
	#include "Effects.h"
#endif

#include <string.h>

CommaDelimitReturn Effects::cdrPixel, Effects::cdrColor;

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

	//int tot = lseqs[currentSequence].totalPixels;
	
	//Loop through all pixels and set color to clear
	for (int i = 0; i < lseqs[currentSequence].totalPixels; i++) {
		strip->setPixelColor(i, CLEAR);
	}

	//Show new effects
	strip->show();
}

void Effects::rainbow(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t currentSequence, int16_t* i, int16_t* p0, int16_t* p1, int16_t* p2, int16_t* p3, int16_t* p4, int16_t* p5, Strip* PassedStripClass) {
	//Exit if invalid pointers passed
	if (strip == NULL) {
		return;
	}
	else if (lseqs == NULL)
	{
		return;
	}

	//Add 1 to shiftPixelsBy if shiftPixelsBy is -1 and reset ps
	if (*i < 0) {
		//Add 1 to i making i = 0
		*i = *i + 1;

		//Reset p values
		*p0 = -1, *p1 = 0, *p2 = 1, *p3 = 2, *p4 = 3, *p5 = 4;
	}

	//Verify if p0 is greater than numPixels, and if so add 1 to shiftPixelsBy
	if (*p0 >= lseqs[currentSequence].totalPixels) {
		//Add 1 to shiftPixelsBy and reset j
		*i = *i + 1;

		//Reset p values
		*p0 = 0, *p1 = 1, *p2 = 2, *p3 = 3, *p4 = 4, *p5 = 5;
	}

	if (*i <= lseqs[currentSequence].iterations) {
		strip->setPixelColor((*p0)++, CLEAR);
		strip->setPixelColor((*p1)++, RED);
		strip->setPixelColor((*p2)++, ORANGE);
		strip->setPixelColor((*p3)++, YELLOW);
		strip->setPixelColor((*p4)++, GREEN);
		strip->setPixelColor((*p5)++, BLUE);
		strip->setPixelColor(*p1 - 2, CLEAR);

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

	strip->show();
}

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
		switch (cdrColor.value)
		{
		case 0:
			strip->setPixelColor(elem + shiftPixelToLoad, CLEAR);
			break;
		case 1:
			strip->setPixelColor(elem + shiftPixelToLoad, WHITE);
			break;
		case 2:
			strip->setPixelColor(elem + shiftPixelToLoad, RED);
			break;
		case 3:
			strip->setPixelColor(elem + shiftPixelToLoad, GREEN);
			break;
		case 4:
			strip->setPixelColor(elem + shiftPixelToLoad, BLUE);
			break;
		case 5:
			strip->setPixelColor(elem + shiftPixelToLoad, YELLOW);
			break;
		case 6:
			strip->setPixelColor(elem + shiftPixelToLoad, CYAN);
			break;
		case 7:
			strip->setPixelColor(elem + shiftPixelToLoad, MAGENTA);
			break;
		case 8:
			strip->setPixelColor(elem + shiftPixelToLoad, ORANGE);
			break;
		default:
			strip->setPixelColor(elem + shiftPixelToLoad, CLEAR);
			break;
		}

		Serial.print("Pixel elem and color equal: ");
		Serial.print(elem);
		Serial.print(", ");
		Serial.println(colorValue);
	}

	//Show new effects
	strip->show();
}

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
		*head = initHead;
		*tail = initTail;

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

void Effects::flowThrough(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t currentSequence, int16_t* i, bool isRainbow, int16_t* p0, int16_t* p1, int16_t* p2, int16_t* p3, int16_t* p4, int16_t* p5, Strip* PassedStripClass) {
	//Exit if invalid pointers passed
	if (strip == NULL) {
		return;
	}
	else if (lseqs == NULL)
	{
		return;
	}

	//Add 1 to shiftPixelsBy if shiftPixelsBy is -1 and reset ps
	if (*i < 0) {
		//Add 1 to i making i = 0
		*i = *i + 1;

		if (isRainbow) {
			//Reset p values
			*p0 = -1, *p1 = 0, *p2 = 1, *p3 = 2, *p4 = 3, *p5 = 4;
		}
	}

	if (isRainbow) {
		//Verify if p0 is greater than numPixels, and if so add 1 to shiftPixelsBy
		if (*p0 >= lseqs[currentSequence].totalPixels) {
			//Add 1 to shiftPixelsBy and reset j
			*i = *i + 1;

			//Reset p values
			*p0 = 0, *p1 = 1, *p2 = 2, *p3 = 3, *p4 = 4, *p5 = 5;
		}

		if (*i <= lseqs[currentSequence].iterations) {
			strip->setPixelColor((*p0)++, CLEAR);
			strip->setPixelColor((*p1)++, RED);
			strip->setPixelColor((*p2)++, ORANGE);
			strip->setPixelColor((*p3)++, YELLOW);
			strip->setPixelColor((*p4)++, GREEN);
			strip->setPixelColor((*p5)++, BLUE);
			strip->setPixelColor(*p1 - 2, CLEAR);

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

	strip->show();
}

/*
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

	Effects::cdrColor.nextBegPosition = iret.nextBegPosition;
	Effects::cdrColor.value = iret.value;
}
