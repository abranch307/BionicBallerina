// 
// 
// 

#ifndef _EFFECTS_h
	#include "Effects.h"
#endif

void Effects::allClear(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t* currentSequence) {
	//Declare variables
	char *pixelElems, *SavePtr, *tName;//Hold pixelElems and string placement from strtok_r
	uint16_t i = 0;

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
	//Add 1 to i if i is -1 and reset ps
	if (*i < 0) {
		*i++;

		//Reset p values
		*p0 = -1, *p1 = 0, *p2 = 1, *p3 = 2, *p4 = 3, *p5 = 4;
	}

	//Verify if p0 is greater than numPixels, and if so add 1 to i
	if (*p0 >= lseqs[*currentSequence].totalPixels) {
		//Add 1 to i and reset j
		*i++;
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

void Effects::loadColor(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t* currentSequence) {
	//Declare variables
	char *pixelElems, *colorElems, *SavePtr, *SavePtr2, *tName;//Hold pixelElems and string placement from strtok_r
	uint16_t i = 0;

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

	//Set first pixelElems and color elems
	pixelElems = strtok_r((char*)lseqs[*currentSequence].effectedPixels, ",", &SavePtr);
	colorElems = strtok_r((char*)lseqs[*currentSequence].colors, ",", &SavePtr2);


	//Exit if pixelElems or colorElems is null
	if (pixelElems == NULL || colorElems == NULL) {
		return;
	}

	//Loop through tokens and set struct values as needed
	do
	{
		strip->setPixelColor(atoi(pixelElems), atoi(colorElems));
	} while ((pixelElems = strtok_r(NULL, ",", &SavePtr)) != NULL && (colorElems = strtok_r(NULL, ",", &SavePtr2)));

	//Show new effects
	strip->show();
}

void Effects::bounceBack(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t* currentSequence) {

}

void Effects::flowThrough(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t* currentSequence) {

}
