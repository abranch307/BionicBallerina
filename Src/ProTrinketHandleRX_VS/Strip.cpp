// 
// 
// 

#include "Strip.h"

#ifndef _SEQUENCESCHEDULER_h
	#include "SequenceScheduler.h"
#endif

Adafruit_DotStar strip = (1, 1, 2, DOTSTAR_RBG);
LightingEffect *stripEffects;
uint16_t counter1, counter2, currentSequence, i, j, p0 = -1, p1 = 0, p2 = 1, p3 = 2, p4 = 3, p5 = 4;

/*
*/
StripClass::StripClass() {
	//Do nothing - this is for array initializers...
}

/*
*/
boolean StripClass::UpdateStripData(StripInfo *stripData, LightingEffect *effects) {
	boolean success = false;

	//Change number of pixels and pin setup to true setup
	strip.updateLength(stripData->NumPixels);
	strip.updatePins(stripData->Datapin, stripData->ClockPin);

	//Set lighting effects for this strip
	stripEffects = effects;

	//Change success to true
	success = true;
	
	return success;
}

/*
*/
void StripClass::updateCounter() {
	//Add 1 to counter
	counter1++;

	//Verify if end of current lighting effect is here, then move to next sequence if necessary
	if (counter1 > stripEffects[currentSequence].duration) {
		//Reset global variables
		resetGlobalVars();

		//Clear all effects on strip
		allClear();

		//Move to next sequence
		currentSequence++;
	}

	//Verify that we're not outside of sequences array
	if (counter1 >= (sizeof(stripEffects)/sizeof(LightingEffect))) {
		//Clear strip and return from this method...
		allClear();
		return;
	}

	//If delaytime % counter is zero, then perform next peformance of lighting effect
	if ((stripEffects[currentSequence].delayTime % counter1) == 0) {
		switch (stripEffects[currentSequence].lightsequence) {
			case ALLCLEAR:
				allClear();
				break;
			case RAINBOW:
				//Add 1 to i if i is -1 and reset ps
				if (i < 0) {
					i++;

					//Reset p values
					p0 = -1, p1 = 0, p2 = 1, p3 = 2, p4 = 3, p5 = 4;
				}

				//Add 1 to j
				j++;

				//Verify if j is greater than effectPixels, and if so reset j and add 1 to i
				if (j >= stripEffects[currentSequence].effectPixels) {
					//Add 1 to i and reset j
					i++;
					j = 0;
				}
				rainbow(stripEffects[currentSequence].iterations, stripEffects[currentSequence].delayTime);
				break;
			case LOADCOLOR:
				loadColor(stripEffects[currentSequence].color, stripEffects[currentSequence].delayTime);
				break;
			case BOUNCEBACK:
				bounceBack(stripEffects[currentSequence].color, stripEffects[currentSequence].delayTime, stripEffects[currentSequence].bounces, stripEffects[currentSequence].effectPixels);
				break;
			case FLOWTHROUGH:
				flowThrough(stripEffects[currentSequence].color, stripEffects[currentSequence].delayTime, stripEffects[currentSequence].iterations, stripEffects[currentSequence].effectPixels);
				break;
		}
	}
}

/*
*/
void StripClass::resetGlobalVars() {
	//Reset global variables
	i = -1;
	j = -1;
}

/*
*/
void StripClass::allClear() {
	uint16_t i = 0;
	for (i = 0; i < stripEffects[currentSequence].effectPixels; i++) {
		strip.setPixelColor(i, 0x000000);
	}
	strip.show();
}

/*
*/
void StripClass::rainbow(uint16_t iterations, uint16_t delayTime) {
	strip.setPixelColor(p0++, 0x000000);
	strip.setPixelColor(p1++, 0xFF0000);
	strip.setPixelColor(p2++, 0xFFA500);
	strip.setPixelColor(p3++, 0xFFFF00);
	strip.setPixelColor(p4++, 0x008000);
	strip.setPixelColor(p5++, 0x0000FF);
	strip.setPixelColor(p1 - 2, 0x000000);

	if (i + 1 != stripEffects[currentSequence].iterations) {
		if (p0 == 30)
			p0 = 0;
		if (p1 == 30)
			p1 = 0;
		if (p2 == 30)
			p2 = 0;
		if (p3 == 30)
			p3 = 0;
		if (p4 == 30)
			p4 = 0;
		if (p5 == 30)
			p5 = 0;
	}

	strip.show();
}

/*
*/
void StripClass::loadColor(uint32_t color, uint16_t delayTime) {
	for (i; i < stripEffects[currentSequence].effectPixels; i++)
	{
		strip.setPixelColor(i, color);
		strip.show();
	}
}

/*
*/
void StripClass::bounceBack(uint32_t color, uint16_t delayTime, uint16_t bounces, uint16_t pixels) {
	boolean forward = true;
	uint16_t tail = 0, i = 0;
	int head = pixels;
	for (i = 0; i < pixels; i++) {
		strip.setPixelColor(i, color);
	}

	strip.show();

	while (bounces > 0) {

		while (forward) {
			if (head < pixels) {
				strip.setPixelColor(head++, color);
				strip.setPixelColor(tail++, 0x0);
				//strip.setPixelColor(tail++, CLEAR);
			}
			else {
				forward = false;
			}

			strip.show();
			delay(delayTime);
		}
		while (!forward) {
			if (tail >= 0) {
				//strip.setPixelColor(head--, CLEAR);
				strip.setPixelColor(head--, 0x0);
				strip.setPixelColor(tail--, color);
			}
			else {
				forward = true;
			}

			strip.show();
		}

		bounces--;
	}
}

/*
*/
void StripClass::flowThrough(uint32_t color, uint16_t delayTime, uint16_t iterations, uint16_t pixels) {
	uint16_t head = 0, tail = 0 - pixels, i = 0;

	for (i = 0; i < iterations; i++) {
		for (int j = 0; j < pixels; j++) {
			strip.setPixelColor(head++, color);
			strip.setPixelColor(tail++, 0x0);
			//strip.setPixelColor(tail++, CLEAR);

			if (i + 1 != iterations) {
				if (head >= pixels) {
					head = 0;
				}
			}
			if (tail >= pixels) {
				tail = 0;
			}
			strip.show();
		}
	}
}
