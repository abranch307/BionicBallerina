/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created:
	Last Modified: 12/16/2016
	Class: Strip.cpp
	Class Description: Class keeps track of information associated with different strips of LEDs. Uses all basic information associated with Adafruit_DotStar
		class and adds more to keep track of.
	

*/ 

#ifndef _EFFECTS_h
	#include "Effects.h"
#endif

#include "Strip.h"

Strip::Strip(void) {

}

Strip::~Strip() {
	lseqs = NULL;
}

Strip::Strip(uint16_t NumPixels, uint8_t Datapin, uint8_t ClockPin, uint8_t RGB, LightingSequence* initSeq, uint16_t numSeqs) {
	strip = Adafruit_DotStar(NumPixels, Datapin, ClockPin, DOTSTAR_RGB);
	lseqs = initSeq;
	countSeqs = numSeqs;
}


/*
	Method: Update
	Parameters: currentPerformanceTime: unsigned long
	Return: void
	Description: This is where most of the work is done. Make sure we aren't passed designated time or sequence, read the next sequence, and call
		the method that will update the strip of LEDs. 
	
*/
void Strip::Update(unsigned long currentPerformanceTime) {
	//Declare variables
	unsigned long roundedDuration = 0;
	uint16_t hd = 0, tl = 0;  //temporary head and tail placement for bounces

	//Get current Duration
	currentDuration = currentPerformanceTime - prevSeqTimesAccumulated;

	Serial.println();

	//Exit if currentSequence is > total sequences (call allClear function for strip)
	if (currentSequence >= countSeqs) {
		if (!proceed) {
			//Call allClear for Strip
			Effects::allClear(&strip, lseqs, 0, this);

			//Reset global variables
			resetGlobalVars();

			//Serial.print(" CurrentSequence ");
			//Serial.println(currentSequence);

			//Set proceed to true so we don't do this over and over again after effects has stopped
			proceed = true;
		}
		return;
	}

	//Set proceed to false to not initially process lighting sequence
	proceed = false;

	////Move to next sequence if duration is greater than current duration
	if (currentDuration > lseqs[currentSequence].duration) {
		//Add current lighting sequence's duration time to prevSeqTimesAccumulated to restart currentDuration
		prevSeqTimesAccumulated += lseqs[currentSequence].duration;

		//Move to next sequence and reset current duration to 0
		currentSequence++;
		currentDuration = 0;
		prevDuration = -1;

		if (currentSequence < countSeqs) {
			//Set brighness here
			Effects::updateBrightness(&strip, lseqs, currentSequence);
		}

		//Reset necessary global variables
		i = -1;
		init = true;
	}

	////Do i need to keep track of last currentDuration so that when currentDuration is changed to int, we don't update more than 1?
	////If currentDuration is .5, this would round up to 1.  If the next time in, the currentDuration is 1.2, this would round up to 1 as well...
	////I may need to keep track of lass duration processed (being 1), so once .5 is done, it won't do 1.2 as one...
	////Or, since currentDuration is milliseconds, will it never = 0?  delaytime = 2000, currentDuration = 2001, won't % to 0...
	//
	//Round duration down to thousandths and compare against last duration
	roundedDuration = (unsigned long)((float)currentDuration/(float)10) * 10;
	if (prevDuration == -1) {
		proceed = true;
		prevDuration = roundedDuration;

		if (currentSequence < countSeqs) {
			//Set brighness here
			Effects::updateBrightness(&strip, lseqs, currentSequence);
		}
	}
	else if (roundedDuration != prevDuration) {
		proceed = true;
		prevDuration = roundedDuration;
	}

	//Set current duration to rounded duration
	currentDuration = roundedDuration;

	//Update brightness if necessary
	if (lseqs[currentSequence].incrBrightness != 0 && (roundedDuration % lseqs[currentSequence].brightnessDelayTime) == 0 && proceed) {
		Effects::updateBrightness(&strip, lseqs, currentSequence);
	}

	//If delaytime % counter is zero, then perform next peformance of lighting effect
	if ((roundedDuration % lseqs[currentSequence].delayTime) == 0 && proceed) {
		switch (lseqs[currentSequence].lightsequence) {
			case ALLCLEAR:
				Effects::allClear(&strip, lseqs, currentSequence, this);
				break;
			case RAINBOW:
				Effects::flowThrough(&strip, lseqs, currentSequence, &i, true, &p0, &p1, &p2, &p3, &p4, &p5, NULL, this);
				break;
			case LOADCOLOR:
				Effects::loadColor(&strip, lseqs, currentSequence, 0, this);
				break;
			case BOUNCEBACK:
				Effects::bounceBack(&strip, lseqs, currentSequence, &init, &forward, &i, &tail, &head, &bounces, hd, tl, this);
				break;
			case FLOWTHROUGH:
				Effects::flowThrough(&strip, lseqs, currentSequence, &i, false, &p0, &p1, &p2, &p3, &p4, &p5, virtualPixelIndexArray, this);
				break;
		}

		//Reset proceed to false (works with exiting if current sequence is greater and this placement is important)
		proceed = false;
	}
}

/*
	Method: setColor
	Parameters: pixel: uint16_t, color: uint32_t
	Return: void
	Description: set a single pixel to a single color.
	
*/
void Strip::setColor(uint16_t pixel, uint32_t color) {
	strip.setPixelColor(pixel, color);
}

/*
	Method: getStrip
	Parameters: none
	Return: strip: Adafruit_DotStar *
	Description: Return point to Adafruit_DotStar object associated with this strip
	
*/
Adafruit_DotStar* Strip::getStrip() {
	return &strip;
}

/*
	Method: getProceed
	Parameters: none
	Return: proceed: bool
	Description: Return proceed
	
*/
bool Strip::getProceed() {
	return proceed;
}

/*
	Method: getCurrentSeq
	Parameters: none
	Return: uint16_t
	Description: Return currentSequence
	
*/
uint16_t Strip::getCurrentSeq() {
	return currentSequence;
}
/*
	Method: getCountSeqs
	Parameters: none
	Return: countSeqs: uint16_t
	Description: Return countSeqs
	
*/
uint16_t Strip::getCountSeqs() {
	return countSeqs;
}

/*
	Method: getCurrentDuration
	Parameters: none
	Return: currentDuration: unsigned long
	Description: Return currentDuration
	
*/
unsigned long Strip::getCurrentDuration() {
	return currentDuration;
}

/*
	Method: getPrevDuration
	Parameters: none
	Return: prevDuration: unsigned long
	Description: Return prevDuration
	
*/
unsigned long Strip::getPrevDuration() {
	return prevDuration;
}

/*
	Method:  getPrevSeqTimesAccumulated
	Parameters: none
	Return: prevSeqTimesAccumulated: unsigned long
	Description: Return prevSeqTimesAccumulated
	
*/
unsigned long Strip::getPrevSeqTimesAccumulated() {
	return prevSeqTimesAccumulated;
}

/*
	Method: getEffectNum
	Parameters: none
	Return: current effect we are on.
	Description: Return current effect number.
	
*/
uint8_t Strip::getEffectNum() {
	return lseqs[currentSequence].lightsequence;
}

/*
	Method: resetGlobalVars
	Parameters: none
	Return: true of succesfull, false if not
	Description: Reset variables associated with sequences, so the next one can use them without issue.
	
*/
bool Strip::resetGlobalVars() {
	//currentSequence = 0;
	currentDuration = 0, prevDuration = -1, prevSeqTimesAccumulated = 0;
	proceed = false, init = true, forward = true;
	counter1 = 0, counter2 = 0, i = -1, j = -1, p0 = -1, p1 = 0, p2 = 1, p3 = 2, p4 = 3, p5 = 4, tail = 0, head = 0, bounces = 0;

	return true;
}
/*
	Method: resetPerformance
	Parameters: none
	Return: true if succesfull, false otherwise
	Description: reset all sequence variables, and set currentSequence to 0.
	
*/
bool Strip::resetPerformance() {
	//Reset global variables
	currentSequence = 0;

	resetGlobalVars();

	return true;
}

/*
	Method: findCurrentSeqFromPerformanceTime
	Parameters: performanceTime: unsigned long
	Return: true if found, false otherwise.
	Description: Based on current time, make sure we are using the correct sequence. Correct currentSequence variable if not.
	
*/
bool Strip::findCurrentSeqFromPerformanceTime(unsigned long performanceTime) {
	//Declare variables
	LightingSequence temp;
	int16_t i = 0;

	//Reset global variables
	resetPerformance();

	//Exit if currentTime == 0
	if (performanceTime == 0) {
		return true;
	}

	//Loop and find current sequence with respect to passed currentTime
	temp = lseqs[i];
	while (&temp != NULL) {
		//Verify if current prevSeqTimesAccumulated plus this record's duration is >= currentTime
		if ((prevSeqTimesAccumulated + temp.duration) > performanceTime) {
			//Exit since we're at the correct currentSequence
			break;
		}
		else
		{
			//Add 1 to currentSequence
			currentSequence++;

			//Add to prevSeqTimesAccumulated
			prevSeqTimesAccumulated += (temp.duration);

			//Move to next sequence
			i++;
			temp = lseqs[i];
		} 
	}
	
}

/*
	Method:  getCurrentLightingSequence
	Parameters: none
	Return: LightingSequence*
	Description: return the current sequence being run by the strip.
	
*/
LightingSequence* Strip::getCurrentLightingSequence() {
	return &lseqs[currentSequence];
}

/*
	Method: getLightingSequences
	Parameters: none
	Return: lseqs: LightingSequence *
	Description: Return lighting sequences associated with this strip
	
*/
LightingSequence* Strip::getLightingSequences() {
	return lseqs;
}
