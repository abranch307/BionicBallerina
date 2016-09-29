// 
// 
// 

#ifndef _EFFECTS_h
	#include "Effects.h"
#endif

void Effects::allClear(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t* currentSequence) {
	//Exit if invalid pointers passed
	if (strip == NULL) {

	}
	else if (lseqs == NULL) 
	{
		return;
	}
	else if (currentSequence == NULL)
	{
		return;
	}

	//Declare variables
	char *pixelElems, *SavePtr, *tName;//Hold pixelElems and string placement from strtok_r

	//Loop through tokens and set struct values as needed
	pixelElems = strtok_r((char*)lseqs[*currentSequence].effectedPixels, ",", &SavePtr);//Set first pixelElems

	//Exit if pixelElems is null
	if (pixelElems == NULL) {
		return;
	}

	do 
	{
		strip->setPixelColor(atoi(pixelElems), CLEAR);
	} while ((pixelElems = strtok_r(NULL, ",", &SavePtr)) != NULL);

	//Show new effects
	strip->show();
}

void Effects::rainbow(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t* currentSequence, uint16_t* i, uint16_t* p0, uint16_t* p1, uint16_t* p2, uint16_t* p3, uint16_t* p4, uint16_t* p5) {
	//Exit if invalid pointers passed
	if (strip == NULL) {

	}
	else if (lseqs == NULL)
	{
		return;
	}
	else if (currentSequence == NULL)
	{
		return;
	}

	//Add 1 to shiftPixelsBy if shiftPixelsBy is -1 and reset ps
	if (*i < 0) {
		*i++;

		//Reset p values
		*p0 = -1, *p1 = 0, *p2 = 1, *p3 = 2, *p4 = 3, *p5 = 4;
	}

	//Verify if p0 is greater than numPixels, and if so add 1 to shiftPixelsBy
	if (*p0 >= lseqs[*currentSequence].totalPixels) {
		//Add 1 to shiftPixelsBy and reset j
		*i++;

		//Reset p values
		*p0 = -1, *p1 = 0, *p2 = 1, *p3 = 2, *p4 = 3, *p5 = 4;
	}

	if (*i <= lseqs[*currentSequence].iterations) {
		strip->setPixelColor(*p0++, CLEAR);
		strip->setPixelColor(*p1++, RED);
		strip->setPixelColor(*p2++, ORANGE);
		strip->setPixelColor(*p3++, YELLOW);
		strip->setPixelColor(*p4++, GREEN);
		strip->setPixelColor(*p5++, BLUE);
		strip->setPixelColor(*p1 - 2, CLEAR);

		if (*p0 == lseqs[*currentSequence].totalPixels)
		{
			*p0 = 0;
		}
		if (*p1 == lseqs[*currentSequence].totalPixels)
		{
			*p1 = 0;
		}
		if (*p2 == lseqs[*currentSequence].totalPixels)
		{
			*p2 = 0;
		}
		if (*p3 == lseqs[*currentSequence].totalPixels)
		{
			*p3 = 0;
		}
		if (*p4 == lseqs[*currentSequence].totalPixels)
		{
			*p4 = 0;
		}
		if (*p5 == lseqs[*currentSequence].totalPixels)
		{
			*p5 = 0;
		}
	}
}

void Effects::loadColor(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t* currentSequence, uint16_t* shiftPixelToLoad, bool clearLastTailPixel, bool clearLastHeadPixel) {
	//Exit if invalid pointers passed
	if (strip == NULL) {

	}
	else if (lseqs == NULL)
	{
		return;
	}
	else if (currentSequence == NULL)
	{
		return;
	}

	//Declare variables
	char *pixelElems, *colorElems, *SavePtr, *SavePtr2, *tName;//Hold pixelElems and string placement from strtok_r
	uint16_t lastHead = 0;

	//Set first pixelElems and color elems
	pixelElems = strtok_r((char*)lseqs[*currentSequence].effectedPixels, ",", &SavePtr);
	colorElems = strtok_r((char*)lseqs[*currentSequence].colors, ",", &SavePtr2);


	//Exit if pixelElems or colorElems is null
	if (pixelElems == NULL || colorElems == NULL) {
		return;
	}

	//Clear last tail pixel if specified
	if (clearLastTailPixel) {
		strip->setPixelColor((atoi(pixelElems) + *shiftPixelToLoad - 1), CLEAR);
	}

	//Loop through tokens and set struct values as needed
	do
	{
		lastHead = (atoi(pixelElems) + *shiftPixelToLoad);
		strip->setPixelColor(lastHead, atoi(colorElems));
	} while ((pixelElems = strtok_r(NULL, ",", &SavePtr)) != NULL && (colorElems = strtok_r(NULL, ",", &SavePtr2)));

	//Clear last head pixel if specified
	if (clearLastHeadPixel) {
		strip->setPixelColor(lastHead + 1, CLEAR);
	}

	//Show new effects
	strip->show();
}

void Effects::bounceBack(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t* currentSequence, bool* init, bool* forward, uint16_t *shiftPixelsBy, uint16_t* tail, uint16_t* head, uint16_t* bounces, uint16_t initHead, uint16_t initTail) {
	//Exit if invalid pointers passed
	if (strip == NULL) {

	}
	else if (lseqs == NULL)
	{
		return;
	}
	else if (currentSequence == NULL)
	{
		return;
	}

	//Initilialize if first time in
	if (*init) {
		//Simply set colors, setup values, and exit
		loadColor(strip, lseqs, currentSequence, 0, false, false);

		*init = false;
		*bounces = lseqs[*currentSequence].bounces;
		*head = initHead;
		*tail = initTail;
		*shiftPixelsBy = 0;

		strip->show();

		return;
	}

	if (*bounces > 0) {
		if (*forward) {
			//Add 1 to shiftPixelsBy
			*shiftPixelsBy++;
			*head++;
			*tail++;

			if (*head < lseqs[*currentSequence].totalPixels) {
				//Load color with a shift and clear previous tail pixel
				loadColor(strip, lseqs, currentSequence, shiftPixelsBy, true, false);
			}
			else {
				*forward = false;
				*bounces++;
			}
		}
		else {
			//Subtract 1 from shiftPixelsBy
			*shiftPixelsBy--;
			*head--;
			*tail--;

			if (*tail >= 0) {
				//Load color with a shift and clear previous head pixel
				loadColor(strip, lseqs, currentSequence, shiftPixelsBy, false, true);
			}
			else {
				*forward = true;
				*bounces--;
			}
		}
	}

	strip->show();
}

void Effects::flowThrough(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t* currentSequence) {
	//Exit if invalid pointers passed
	if (strip == NULL) {

	}
	else if (lseqs == NULL)
	{
		return;
	}
	else if (currentSequence == NULL)
	{
		return;
	}


}
