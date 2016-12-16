/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created:
	Last Modified: 12/16/2016
	Class: Effects Manager.cpp
	Class Description:
		This class manages multiple strips connected to a single ProTrinket 5V microcontroller to
		make sure they light up during proper times, and in synchronization with each other.

*/

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
/*
	Method: Update
	Parameters: elapsedTime: unsigned long
	Return: void
	Description: itterates through each strip currently being managed and updates their current
		time in the performance

*/
void EffectsManager::Update(unsigned long elapsedTime) {
	//Declare variables

	//Add elapsed time to performance time
	performanceElapsedTime += elapsedTime;

	//Update all strips with elapsed time
	for (int i = 0; i < countStrips; i++) {
		//Update strip and track information return (effectNum and performance information)
		strips2Manage[i].Update(performanceElapsedTime);

		//Serial.print("Just updated strip ");
		//Serial.println(i + 1);
	}
}
/*
	Method: getElapsedTime
	Parameters: currentTime: unsigned long
	Return: elapsed time: uint16_t
	Description: Given the current time, calculates time since this method was last called.

*/
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
/*
	Method: resumePerformance
	Parameters: performanceTime: unsigned long
	Return: true if sucesfully resumed, false otherwise
	Description: Resumes all strips from their paused states.

*/
bool EffectsManager::resumePerformance(unsigned long performanceTime) {
	prevTime = -1;

	//Find current sequence time based on performance time
	for (int i = 0; i < countStrips; i++) {
		strips2Manage[i].findCurrentSeqFromPerformanceTime(performanceTime);
	}
}
/*
	Method: resetPerformance
	Parameters: none
	Return: true if sucesfully reset, false otherwise
	Description: resets all variables to initial values

*/
bool EffectsManager::resetPerformance() {
	//Reset global variables
	performanceElapsedTime = 0;
	prevTime = -1;

	//Reset strips' global variables
	for (int i = 0; i < countStrips; i++) {
		strips2Manage[i].resetPerformance();
	}
}
/*
	Method: clearStrips
	Parameters: none
	Return: true if sucesfully cleared, false otherwise
	Description: turns all LEDs on all strips off.
*/
bool EffectsManager::clearStrips() {
	//Clear strips
	for (int i = 0; i < countStrips; i++) {
		Effects::allClear(strips2Manage[i].getStrip(), strips2Manage[i].getLightingSequences(), 0, NULL);
		strips2Manage[i].getStrip()->clear();
	}
}
/*
	Method: findCurrentSeqFromPerformanceTime
	Parameters: performanceTime: unsigned long
	Return: true if sucesfully retrieved, false otherwise

*/
bool EffectsManager::findCurrentSeqFromPerformanceTime(unsigned long performanceTime) {
	//Reset global variables
	performanceElapsedTime = performanceTime;
	prevTime = performanceTime;

	//Find current sequence time based on performance time
	for (int i = 0; i < countStrips; i++) {
		strips2Manage[i].findCurrentSeqFromPerformanceTime(performanceTime);
	}
}
/*
	Method: getStrips
	Parameters: none
	Return: Strips2Manage: Strip * 
	Description: Return a pointer to managed strips

*/
Strip* EffectsManager::getStrips() {
	return strips2Manage;
}
/*
	Method: getPerformanceElapsedTime
	Parameters: none
	Return: performanceElapsedTime: unsigned long
	Description: Return the current elapsed tiem of the performance

*/
unsigned long EffectsManager::getPerformanceElapsedTime() {
	return performanceElapsedTime;
}
