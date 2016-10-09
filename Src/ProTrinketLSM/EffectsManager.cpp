// 
// 
// 

#ifndef _EFFECTSMANAGER_h
	#include "EffectsManager.h"
#endif

EffectsManager::EffectsManager(void) {

}

EffectsManager::~EffectsManager() {
	strips2Manage = NULL;
}

EffectsManager::EffectsManager(Strip *strips, int8_t numStrips) {
	strips2Manage = strips;
	performanceElapsedTime = 0;
	prevTime = -1;
	countStrips = numStrips;
}

void EffectsManager::Update(unsigned long elapsedTime) {
	//Declare variables

	//Add elapsed time to performance time
	performanceElapsedTime += elapsedTime;
	effectsManagerUpdateRet->performanceElapsedTime = performanceElapsedTime;

	//Update all strips with elapsed time
	for (int i = 0; i < countStrips; i++) {
		//Update strip and track information return (effectNum and performance information)
		strips2Manage[i].Update(performanceElapsedTime);
	}
}

uint16_t EffectsManager::getElapsedTime(unsigned long currentTime) {
	//Declare variables
	uint16_t elapsedTime;
	
	//Set prevTime to currentTime if not currently set
	if (prevTime == -1) {
		prevTime = currentTime;
	}

	//Get elapsed time then set previous time
	elapsedTime = currentTime - prevTime;
	prevTime = currentTime;

	return elapsedTime;
}

bool EffectsManager::resetPerformance() {
	//Reset global variables
	performanceElapsedTime = 0;
	prevTime = -1;

	//Reset strips' global variables
	
}

bool EffectsManager::findCurrentSeqFromPerformanceTime(unsigned long performanceTime) {
	//Reset global variables
	performanceElapsedTime = performanceTime;
	prevTime = performanceTime;

	//Reset strips' global variables
	for (int i = 0; i < countStrips; i++) {
		strips2Manage[i].findCurrentSeqFromPerformanceTime(performanceTime);
	}
}

Strip* EffectsManager::getStrips() {
	return strips2Manage;
}