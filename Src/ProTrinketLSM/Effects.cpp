// 
// 
// 

#ifndef _EFFECTS_h
	#include "Effects.h"
#endif

#include <string.h>

void Effects::allClear(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t currentSequence, Strip* PassedStripClass) {
	//Set strip success to -1 by default
	//PassedStripClass->stripUpdateRet->effectSuccess = -1;

	//Exit if invalid pointers passed
	if (strip == NULL) {
		return;
	}
	else if (lseqs == NULL) 
	{
		return;
	}

	//Declare variables
	char *pixelElems = (char*)calloc((strlen(lseqs[currentSequence].effectedPixels)+1), sizeof(char));

	//Copy effected pixels and colorElems strings
	strncpy(pixelElems, lseqs[currentSequence].effectedPixels, strlen(lseqs[currentSequence].effectedPixels));

	//Serial.print("All of the pixel elements are ");
	//Serial.println(lseqs[currentSequence].effectedPixels);
	//Serial.print("Current sequence is ");
	//Serial.println(currentSequence);

	//Set first pixelElems and color elems
	pixelElems = strtok(pixelElems, ",");

	//Exit if pixelElems is null
	if (pixelElems == NULL) {
		return;
	}

	//Set strip success to 0 to show we've made it through invalid pointers
	//PassedStripClass->stripUpdateRet->effectSuccess = 0;

	do
	{
		//Serial.print("1 Pixel element is ");
		//Serial.println(atoi(pixelElems));

		strip->setPixelColor(atoi(pixelElems), CLEAR);
	} while ((pixelElems = strtok(NULL, ",")) != NULL);

	//Show new effects
	strip->show();

	if (pixelElems != NULL) 
	{
		free(pixelElems);
	}
	
	//Set strip success to 1 to show effect completed successfully
	//PassedStripClass->stripUpdateRet->effectSuccess = 1;
}

void Effects::rainbow(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t currentSequence, int16_t* i, int16_t* p0, int16_t* p1, int16_t* p2, int16_t* p3, int16_t* p4, int16_t* p5, Strip* PassedStripClass) {
	//Set strip success to -1 by default
	//PassedStripClass->stripUpdateRet->effectSuccess = -1;

	//Exit if invalid pointers passed
	if (strip == NULL) {
		return;
	}
	else if (lseqs == NULL)
	{
		return;
	}

	//Set strip success to 0 to show we've made it through invalid pointers
	PassedStripClass->stripUpdateRet->effectSuccess = 0;

	//Add 1 to shiftPixelsBy if shiftPixelsBy is -1 and reset ps
	if (*i < 0) {
		*i = *i + 1;

		//Reset p values
		*p0 = -1, *p1 = 0, *p2 = 1, *p3 = 2, *p4 = 3, *p5 = 4;

		//Set strip success to 0 to show we've made it through invalid pointers
		//PassedStripClass->stripUpdateRet->effectSuccess = 1;
	}

	/*PassedStripClass->stripUpdateRet->currentSequence = *p0;
	PassedStripClass->stripUpdateRet->currentDuration = *p1;
	PassedStripClass->stripUpdateRet->prevSeqTimesAccumulated = *p2;
	PassedStripClass->stripUpdateRet->effectNum = *i;*/

	//Verify if p0 is greater than numPixels, and if so add 1 to shiftPixelsBy
	if (*p0 >= lseqs[currentSequence].totalPixels) {
		//Add 1 to shiftPixelsBy and reset j
		*i = *i + 1;

		//Reset p values
		*p0 = -1, *p1 = 0, *p2 = 1, *p3 = 2, *p4 = 3, *p5 = 4;

		//Set strip success to 0 to show we've made it through invalid pointers
		//PassedStripClass->stripUpdateRet->effectSuccess = 2;
	}

	if (*i <= lseqs[currentSequence].iterations) {
		*p0 = *p0 + 1;
		*p1 = *p1 + 1;
		*p2 = *p2 + 1;
		*p3 = *p3 + 1;
		*p4 = *p4 + 1;
		*p5 = *p5 + 1;

		strip->setPixelColor(*p0, CLEAR);
		strip->setPixelColor(*p1, RED);
		strip->setPixelColor(*p2, ORANGE);
		strip->setPixelColor(*p3, YELLOW);
		strip->setPixelColor(*p4, GREEN);
		strip->setPixelColor(*p5, BLUE);
		strip->setPixelColor(*p1 - 2, CLEAR);

		if (*p0 == lseqs[currentSequence].totalPixels)
		{
			*p0 = 0;
		}
		if (*p1 == lseqs[currentSequence].totalPixels)
		{
			*p1 = 0;
		}
		if (*p2 == lseqs[currentSequence].totalPixels)
		{
			*p2 = 0;
		}
		if (*p3 == lseqs[currentSequence].totalPixels)
		{
			*p3 = 0;
		}
		if (*p4 == lseqs[currentSequence].totalPixels)
		{
			*p4 = 0;
		}
		if (*p5 == lseqs[currentSequence].totalPixels)
		{
			*p5 = 0;
		}

		//Set strip success to 1 to show effect completed successfully
		//PassedStripClass->stripUpdateRet->effectSuccess = 3;
	}

	strip->show();
}

void Effects::loadColor(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t currentSequence, uint16_t shiftPixelToLoad, bool clearLastTailPixel, bool clearLastHeadPixel, Strip* PassedStripClass) {
	//Set strip success to -1 by default
	//PassedStripClass->stripUpdateRet->effectSuccess = -1;

	//Exit if invalid pointers passed
	if (strip == NULL) {
		return;
	}
	else if (lseqs == NULL)
	{
		return;
	}

	//Declare variables
	char *pixelElems = (char*)calloc((strlen(lseqs[currentSequence].effectedPixels)+1), sizeof(char)),
		*colorElems = (char*)calloc((strlen(lseqs[currentSequence].colors)+1), sizeof(char)),
		*savePtr1, *savePtr2;
	uint16_t lastHead = 0;
	uint32_t color = 0;

	//Clear all pixels first
	Effects::allClear(strip, lseqs, currentSequence, PassedStripClass);

	//Copy effected pixels and colorElems strings
	strncpy(pixelElems, lseqs[currentSequence].effectedPixels, strlen(lseqs[currentSequence].effectedPixels));
	strncpy(colorElems, lseqs[currentSequence].colors, strlen(lseqs[currentSequence].colors));

	//Serial.print("All of the pixel elements are ");
	//Serial.println(pixelElems);
	//Serial.print("All of the color elements are ");
	//Serial.println(colorElems);

	//Set first pixelElems and color elems
	pixelElems = strtok_r(pixelElems, ",", &savePtr1);
	colorElems = strtok_r(colorElems, ",", &savePtr2);

	//Exit if pixelElems or colorElems is null
	if (pixelElems == NULL || colorElems == NULL) {
		if (pixelElems != NULL) {
			free(pixelElems);
		}
		if (colorElems != NULL) {
			free(colorElems);
		}
		if (savePtr1 != NULL) {
			free(savePtr1);
		}
		if (savePtr2 != NULL) {
			free(savePtr2);
		}

		Serial.println("Exiting prematurely b/c pixel or color is null...");
		return;
	}

	//Set strip success to 0 to show we've made it through invalid pointers
	//PassedStripClass->stripUpdateRet->effectSuccess = 0;

	//Clear last tail pixel if specified
	if (clearLastTailPixel) {
		strip->setPixelColor((atoi(pixelElems) + shiftPixelToLoad - 1), CLEAR);
	}

	//Loop through tokens and set struct values as needed
	do
	{
		lastHead = (atoi(pixelElems) + shiftPixelToLoad);
		color = atoi(colorElems);

		//Serial.print("The pixel elem is ");
		//Serial.println(lastHead);
		//Serial.print("The color element is ");
		//Serial.println(color);

		switch (color)
		{
			case 0:
				color = CLEAR;
				break;
			case 1:
				color = WHITE;
				break;
			case 2:
				color = RED;
				break;
			case 3:
				color = GREEN;
				break;
			case 4:
				color = BLUE;
				break;
			case 5:
				color = YELLOW;
				break;
			case 6:
				color = CYAN;
				break;
			case 7:
				color = MAGENTA;
				break;
			case 8:
				color = ORANGE;
				break;
			default:
				break;
		}
		
		strip->setPixelColor(lastHead, color);
		pixelElems = strtok_r(NULL, ",", &savePtr1);
		colorElems = strtok_r(NULL, ",", &savePtr2);
	} while (pixelElems != NULL);

	//Clear last head pixel if specified
	if (clearLastHeadPixel) {
		strip->setPixelColor(lastHead + 1, CLEAR);
	}

	//Show new effects
	strip->show();

	if (pixelElems != NULL) {
		free(pixelElems);
	}
	if (colorElems != NULL) {
		free(colorElems);
	}
	if (savePtr1 != NULL) {
		free(savePtr1);
	}
	if (savePtr2 != NULL) {
		free(savePtr2);
	}

	//Set strip success to 1 to show effect completed successfully
	//PassedStripClass->stripUpdateRet->effectSuccess = 1;
}

void Effects::bounceBack(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t currentSequence, bool* init, bool* forward, int16_t *shiftPixelsBy, int16_t* tail, int16_t* head, uint16_t* bounces, uint16_t initHead, uint16_t initTail, Strip* PassedStripClass) {
	//Set strip success to -1 by default
	PassedStripClass->stripUpdateRet->effectSuccess = -1;

	//Exit if invalid pointers passed
	if (strip == NULL) {
		return;
	}
	else if (lseqs == NULL)
	{
		return;
	}

	//Set strip success to 0 to show we've made it through invalid pointers
	PassedStripClass->stripUpdateRet->effectSuccess = 0;

	PassedStripClass->stripUpdateRet->currentSequence = *init;
	PassedStripClass->stripUpdateRet->currentDuration = *shiftPixelsBy;
	PassedStripClass->stripUpdateRet->prevSeqTimesAccumulated = *head;
	PassedStripClass->stripUpdateRet->effectNum = *tail;

	//Initilialize if first time in
	if (*init) {
		//Simply set colors, setup values, and exit
		loadColor(strip, lseqs, currentSequence, 0, false, false, PassedStripClass);

		*init = false;
		*bounces = lseqs[currentSequence].bounces;
		*shiftPixelsBy = 0;
		*head = initHead;
		*tail = initTail;

		//Set strip success to 1 to show effect completed successfully
		PassedStripClass->stripUpdateRet->effectSuccess = 1;

		return;
	}

	if (*bounces > 0) {
		if (*forward) {
			//Add 1 to shiftPixelsBy
			*shiftPixelsBy = *shiftPixelsBy + 1;
			*head = *head + 1;
			*tail = *tail + 1;

			if (*head < lseqs[currentSequence].totalPixels) {
				//Load color with a shift and clear previous tail pixel
				loadColor(strip, lseqs, currentSequence, *shiftPixelsBy, true, false, PassedStripClass);
			}
			else {
				*forward = false;
				*bounces = *bounces + 1;
			}
		}
		else {
			//Subtract 1 from shiftPixelsBy
			*shiftPixelsBy = *shiftPixelsBy - 1;
			*head = *head - 1;
			*tail = *tail - 1;

			if (*tail >= 0) {
				//Load color with a shift and clear previous head pixel
				loadColor(strip, lseqs, currentSequence, *shiftPixelsBy, false, true, PassedStripClass);
			}
			else {
				*forward = true;
				*bounces = *bounces - 1;
			}
		}
	}

	strip->show();

	//Set strip success to 1 to show effect completed successfully
	PassedStripClass->stripUpdateRet->effectSuccess = 2;
}

void Effects::flowThrough(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t currentSequence, Strip* PassedStripClass) {
	//Set strip success to -1 by default
	PassedStripClass->stripUpdateRet->effectSuccess = -1;

	//Exit if invalid pointers passed
	if (strip == NULL) {
		return;
	}
	else if (lseqs == NULL)
	{
		return;
	}

	//Set strip success to 0 to show we've made it through invalid pointers
	PassedStripClass->stripUpdateRet->effectSuccess = 0;

	//Set strip success to 1 to show effect completed successfully
	PassedStripClass->stripUpdateRet->effectSuccess = 1;
}
