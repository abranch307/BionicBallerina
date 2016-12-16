#include "stdafx.h"
#include "Effects_MockObject.h"
int Effects_MockObject::theStrip[3] = { -1,-1,-1 };

bool Effects_MockObject::allClear_Test(LightingSequence* lseqs, uint16_t currentSequence) {
	bool bret = false;

	if ( lseqs == NULL) {
		return bret;
	}

	if (lseqs[currentSequence].colors == NULL) {
		return bret;
	}
	try {
		//Loop through all pixels and set color to clear
		for (int i = 0; i < lseqs[currentSequence].totalPixels; i++) {
			Effects_MockObject::theStrip[i] = 0;
		}

		//Show new effects
		bret = true;
	}
	catch (...) {
		bret = false;

	}
	
	return bret;
}

bool Effects_MockObject::loadColor_Test(LightingSequence* lseqs, uint16_t currentSequence, uint16_t shiftPixelToLoad) {
	bool bret = false;

	return bret;
}
bool Effects_MockObject::bounceBack_Test(LightingSequence* lseqs, uint16_t currentSequence, bool* init, bool* forward, int16_t *shiftPixelsBy, int16_t* tail, int16_t* head, uint16_t* bounces, uint16_t initHead, uint16_t initTail) {
	bool bret = false;

	return bret;
}
bool Effects_MockObject::flowThrough_Test(LightingSequence* lseqs, uint16_t currentSequence, int16_t* i, bool isRainbow, int16_t* p0, int16_t* p1, int16_t* p2, int16_t* p3, int16_t* p4, int16_t* p5, int16_t* virtualPixelIndexArray) {
	bool bret = false;

	return bret;
}
bool Effects_MockObject::setSinglePixelColor_Test(LightingSequence* lseqs, uint16_t pixelElem, uint16_t color) {
	bool bret = false;

	//Exit if invalid pointers passed
	if (lseqs == NULL)
	{
		return bret;
	}
	try {
		//Convert return color index to actual color
		switch (color)
		{
		case 0:
			Effects_MockObject::theStrip[pixelElem] = color;
			break;
		case 1:
			Effects_MockObject::theStrip[pixelElem] = color;
			break;
		case 2:
			Effects_MockObject::theStrip[pixelElem] = color;
			break;
		case 3:
			Effects_MockObject::theStrip[pixelElem] = color;
			break;
		case 4:
			Effects_MockObject::theStrip[pixelElem] = color;
			break;
		case 5:
			Effects_MockObject::theStrip[pixelElem] = color;
			break;
		case 6:
			Effects_MockObject::theStrip[pixelElem] = color;
			break;
		case 7:
			Effects_MockObject::theStrip[pixelElem] = color;
			break;
		case 8:
			Effects_MockObject::theStrip[pixelElem] = color;
			break;
		default:
			Effects_MockObject::theStrip[pixelElem] = color;
			break;
		}

		bret = true;
	}
	catch (...) {
		bret = false;
	}
	return bret;
}
bool Effects_MockObject::updateBrightness_Test(LightingSequence* lseqs, uint16_t currentSequence) {
	bool bret = false;

	return bret;
}
bool Effects_MockObject::getNextCommaDelimitedColorToken_Test(const char* String, uint16_t BegPosition) {
	bool bret = false;

	return bret;
}
uint16_t Effects_MockObject::getHeadTailofLED_Test(const char* Type, const char* Colors, uint16_t TotalPixels) {
	bool bret = false;

	return bret;
}