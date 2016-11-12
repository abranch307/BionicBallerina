// 
// 
// 

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

//CommaDelimitReturn Effects::cdrPixel, Effects::cdrColor;

void Strip::Update(unsigned long currentPerformanceTime) {
	//Declare variables
	unsigned long roundedDuration = 0;
	uint16_t hd = 0, tl = 0;  //temporary head and tail placement for bounces

	//Get current Duration
	currentDuration = currentPerformanceTime - prevSeqTimesAccumulated;

	//Exit if currentSequence is > total sequences (call allClear function for strip)
	if (currentSequence >= countSeqs) {
		if (!proceed) {
			//Call allClear for Strip
			Effects::allClear(&strip, lseqs, 0, this);

			//Reset global variables
			resetGlobalVars();

			Serial.print(" CurrentSequence ");
			Serial.println(currentSequence);

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
	roundedDuration = (unsigned long)((float)currentDuration/(float)200) * 200;
	if (prevDuration == -1) {
		proceed = true;
		prevDuration = roundedDuration;

		//stripUpdateRet->effectNum = 555;
	}
	else if (roundedDuration != prevDuration) {
		proceed = true;
		prevDuration = roundedDuration;

		//stripUpdateRet->effectNum = 555;
	}

	//Set current duration to rounded duration
	currentDuration = roundedDuration;

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
				hd = getHeadTailofLED("HEAD");
				tl = getHeadTailofLED("TAIL");

				Effects::bounceBack(&strip, lseqs, currentSequence, &init, &forward, &i, &tail, &head, &bounces, hd, tl, this);
				break;
			case FLOWTHROUGH:
				if (i == -1) {
					if (virtualPixelIndexArray != NULL) {
						free(virtualPixelIndexArray);
					}
					virtualPixelIndexArray = (int16_t*)calloc(lseqs[currentSequence].totalPixels, sizeof(int16_t));
					
					////Set default values
					//for (int elem = 0; elem < lseqs[currentSequence].totalPixels; elem++) {
					//	Serial.print("Pixel elem: ");
					//	Serial.print(elem);
					//	Serial.print(" is Virtual Pixel elem: ");
					//	Serial.print(virtualPixelIndexArray[elem]);
					//	Serial.println();
					//}
				}
				//List all values
				/*for (int elem = 0; elem < lseqs[currentSequence].totalPixels; elem++) {
					Serial.print("Pixel elem: ");
					Serial.print(elem);
					Serial.print(" is Virtual Pixel elem: ");
					Serial.print(virtualPixelIndexArray[elem]);
					Serial.println();
				}*/

				Effects::flowThrough(&strip, lseqs, currentSequence, &i, false, &p0, &p1, &p2, &p3, &p4, &p5, virtualPixelIndexArray, this);
				break;
		}

		//Reset proceed to false (works with exiting if current sequence is greater and this placement is important)
		proceed = false;
	}
}

void Strip::setColor(uint16_t pixel, uint32_t color) {
	strip.setPixelColor(pixel, color);
}

Adafruit_DotStar* Strip::getStrip() {
	return &strip;
}

bool Strip::getProceed() {
	return proceed;
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

uint16_t Strip::getHeadTailofLED(const char* Type) {
	//Declare variables
	int elem = 0, tailOrHead = -1;

	//Exit if lseq is null or colors is null
	if (lseqs[currentSequence].colors == NULL) {
		return tailOrHead;
	}

	//Set nextBegPosition to 0
	Effects::cdrColor.nextBegPosition = 0;

	for (elem = 0; elem < lseqs[currentSequence].totalPixels; elem++) {
		//Get next color token from comma delimited string
		Effects::getNextCommaDelimitedColorToken(lseqs[currentSequence].colors, Effects::cdrColor.nextBegPosition);

		if (Effects::cdrColor.value != 0) {
			tailOrHead = elem;

			if (strcmp(Type, "TAIL") == 0) {
				break;
			}
		}
	}

	return tailOrHead;
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

	return true;
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

LightingSequence* Strip::getLightingSequences() {
	return lseqs;
}
