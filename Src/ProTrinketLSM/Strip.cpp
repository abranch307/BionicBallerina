// 
// 
// 

#include "Strip.h"

Strip::Strip(void) {

}

Strip::~Strip() {
	lseqs = NULL;
}

Strip::Strip(uint16_t NumPixels, uint8_t Datapin, uint8_t ClockPin, uint8_t RGB, LightingSequence* initSeq, uint16_t numSeqs) {
	strip = Adafruit_DotStar(NumPixels, Datapin, ClockPin, DOTSTAR_RBG);
	lseqs = initSeq;
	countSeqs = numSeqs;
}

void Strip::Update(unsigned long currentPerformanceTime) {
	//Declare variables
	unsigned long roundedDuration = 0;

	//Get current Duration
	currentDuration = currentPerformanceTime - prevSeqTimesAccumulated;

	//Set return values
	stripUpdateRet->currentDuration = currentDuration;
	stripUpdateRet->currentSequence = currentSequence;
	stripUpdateRet->performanceElapsedTime = currentPerformanceTime;
	stripUpdateRet->prevSeqTimesAccumulated = prevSeqTimesAccumulated;

	//Exit if currentSequence is > total sequences (call allClear function for strip)
	if (currentSequence >= countSeqs) {
		//Call allClear for Strip
		Effects::allClear(&strip, lseqs, &currentSequence);

		if (!proceed) {
			//Reset global variables
			resetGlobalVars();

			//Set effectNum in return struct
			stripUpdateRet->effectNum = -1;

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
	}

	////Do i need to keep track of last currentDuration so that when currentDuration is changed to int, we don't update more than 1?
	////If currentDuration is .5, this would round up to 1.  If the next time in, the currentDuration is 1.2, this would round up to 1 as well...
	////I may need to keep track of lass duration processed (being 1), so once .5 is done, it won't do 1.2 as one...
	////Or, since currentDuration is milliseconds, will it never = 0?  delaytime = 2000, currentDuration = 2001, won't % to 0...
	//
	//Round duration down to thousandths and compare against last duration
	roundedDuration = (unsigned long)((float)currentDuration/(float)1000) * 1000;
	if (prevDuration == -1) {
		proceed = true;
		prevDuration = roundedDuration;
	}
	else if (roundedDuration != prevDuration) {
		proceed = true;
		prevDuration = roundedDuration;
	}

	//Set effectNum in info return struct
	stripUpdateRet->effectNum = -1;

	//If delaytime % counter is zero, then perform next peformance of lighting effect
	if ((lseqs[currentSequence].delayTime % roundedDuration) == 0 && proceed) {
		switch (lseqs[currentSequence].lightsequence) {
			case ALLCLEAR:
				Effects::allClear(&strip, lseqs, &currentSequence);
				stripUpdateRet->effectNum = ALLCLEAR;
				break;
			case RAINBOW:
				Effects::rainbow(&strip, lseqs, &currentSequence, &i, &p0, &p1, &p2, &p3, &p4, &p5);
				stripUpdateRet->effectNum = RAINBOW;
				break;
			case LOADCOLOR:
				Effects::loadColor(&strip, lseqs, &currentSequence, 0, false, false);
				stripUpdateRet->effectNum = LOADCOLOR;
				break;
			case BOUNCEBACK:
				Effects::bounceBack(&strip, lseqs, &currentSequence,&init, &forward, &i, &tail, &head, &bounces, getHeadofLED(), getTailofLED());
				stripUpdateRet->effectNum = BOUNCEBACK;
				break;
			case FLOWTHROUGH:
				//Effects::flowThrough(lseqs[currentSequence].colors, lseqs[currentSequence].delayTime, lseqs[currentSequence].iterations, lseqs[currentSequence].effectedPixels);
				stripUpdateRet->effectNum = FLOWTHROUGH;
				break;
		}
	}
}

uint16_t Strip::getCurrentSeq() {
	return currentSequence;
}

uint16_t Strip::getCountSeqs() {
	return countSeqs;
}

unsigned long Strip::getCurrentDuration() {
	return currentDuration;
}

unsigned long Strip::getPrevDuration() {
	return prevDuration;
}

unsigned long Strip::getPrevSeqTimesAccumulated() {
	return prevSeqTimesAccumulated;
}

uint8_t Strip::getEffectNum() {
	return lseqs[currentSequence].lightsequence;
}

uint16_t Strip::getHeadofLED() {
	//Declare variables
	char *pixelElem, *SavePtr, *tName;//Hold pixelElem and string placement from strtok_r
	uint16_t head = -1;

	//Set first pixelElem and color elems
	pixelElem = strtok_r((char*)lseqs[currentSequence].effectedPixels, ",", &SavePtr);

	//Exit if pixelElem or colorElem is null
	if (pixelElem == NULL) {
		return head;
	}

	//Loop through tokens and set struct values as needed
	do
	{
		head = atoi(pixelElem);
	} while ((pixelElem = strtok_r(NULL, ",", &SavePtr)) != NULL);

	return head;
}

uint16_t Strip::getTailofLED() {
	//Declare variables
	char *pixelElem, *SavePtr, *tName;//Hold pixelElem and string placement from strtok_r
	uint16_t tail = -1;

	//Set first pixelElem and color elems
	pixelElem = strtok_r((char*)lseqs[currentSequence].effectedPixels, ",", &SavePtr);

	//Exit if pixelElem or colorElem is null
	if (pixelElem == NULL) {
		return tail;
	}

	tail = atoi(pixelElem);

	return tail;
}

/*
	Should i always be resetting the p values?
*/
bool Strip::resetGlobalVars() {
	//currentSequence = 0;
	countSeqs = 0, currentDuration = 0, prevDuration = -1, prevSeqTimesAccumulated = 0;
	proceed = false, init = true, forward = true;
	counter1 = 0, counter2 = 0, i = -1, j = -1, p0 = -1, p1 = 0, p2 = 1, p3 = 2, p4 = 3, p5 = 4, tail = 0, head = 0, bounces = 0;

	return true;
}

bool Strip::resetPerformance() {
	//Reset global variables
	currentSequence = 0;

	resetGlobalVars();
}

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

LightingSequence* Strip::getCurrentLightingSequence() {
	return &lseqs[currentSequence];
}
