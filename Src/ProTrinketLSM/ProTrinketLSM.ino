/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created: 
	Last Modified: 12/16/2016
	Class: ProTrinketLSM.ino
	Class Description:
		This is the main file for the Lighting Sequence Manager program. It initializes a ProTrinket 5V
		microcontroller with the given input. Input can either be received from Composer effects GUI or manually
		entered below under the 'setup values' section and in the 'setup' method.
		Variables that need to be changed include: 
			(1): numStrips: Number of strips we are setting effects up for.
			(2): numEffects: For each strip we must have a different instance of this variable. For example, two 
				 strips, we should have numEffects1, and numEffects2 for first and second strip respectively.
			(3): numPixels: Just as in numEffects, we require different instances of this variable for each strip.
				 This declares how many pixels are in each strip.
			(4): seqs: For each strip there must be at least one instance of this variable. it is an array that is
				 initialized with the sequences we want each strip to perform. 
			(5): strips: For each strip there must be at least one instance of this variable. This variable is 
				 located under the "initialize strip" section. We associate with each strip variable the number of
				 pixels, the data and clock pin, what coloring pattern we are following (RGB, RBG, GBR, GRB, BGR, 
				 BRG), which seqs variable is associated with the strip, and how many effects are in the seqs variable.

*/

#include "Structs.h"
#include <SPI.h>
#include "Adafruit_DotStar.h"
#include "Strip.h"
#include "EffectsManager.h"
#include "Effects.h"

//Declare setup values

unsigned long localElapsedTime; //Keep track of when we are in the program

int8_t numStrips = 2, numEffects1 = 5, numEffects2 = 3;
uint16_t numPixels1 = 120, numPixels2 = 5;
bool endSeqs = false;

//Allocate memory for effects manager and setup
EffectsManager effectsManager;
EffectsManagerUpdateReturn *uRet = (EffectsManagerUpdateReturn*)calloc(numStrips, sizeof(EffectsManagerUpdateReturn));

//Allocate memory for strips
Strip *strips = (Strip*)calloc(numStrips, sizeof(Strip));;

//Allocation memory for Lighting Sequences. Number of seqs variables must equal numStrips.
LightingSequence* seqs1 = (LightingSequence*)calloc(numEffects1, sizeof(LightingSequence));
LightingSequence* seqs2 = (LightingSequence*)calloc(numEffects2, sizeof(LightingSequence));

void setup()
{

	/*Order of arguments for seqs variables:
	Effect name, number of pixels in strip, which color value to pass to each pixel. How often in miliseconds to 
	update, length of time for effect in miliseconds, bounces (only used if less than amount for time), iterations 
	(only used if less than amount for time), Initial brightness [0-255], increment brightness, time in miliseconds
	to increment brightness.
	*/
	
	/*Initialize serial interface*/
	Serial.begin(115200);
	Serial.println("Capturing serial output on ProTrinket");

	//Setup lighting sequences 1
	
	seqs1[0] = { RAINBOW, numPixels1, "0,0,0,0,0 ", 200, 7000, 1, 2, 100, 0, 0 };//rainbow effect
	seqs1[1] = { FLOWTHROUGH, numPixels1, "0,1,2,0,2,1,0 ", 200, 11000, 0, 44, 100, 0, 0 };//flowthrough effect
	seqs1[2] = { LOADCOLOR, numPixels1, "0,4,0,4,0 ", 500, 4000, 0, 0, 10, 0, 0 };//load color effect
	seqs1[3] = { BOUNCEBACK, numPixels1, "0,0,1,2,0 ", 200, 4000, 4, 2, 150, 0, 0 };//bounceback effect
	seqs1[4] = { FLOWTHROUGH, numPixels1, "0,4,0,4,0 ", 200, 11000, 0, 44, 100, 0, 0 };//flowthrough effect

	//Initialize strip 1
	strips[0] = Strip(numPixels1, 8, 6, DOTSTAR_RGB, seqs1, numEffects1);
	strips[0].getStrip()->begin();
	strips[0].getStrip()->show();

	//Setup lighting sequences 2
	seqs2[0] = { LOADCOLOR, numPixels2, "5,4,3,2,1 ", 5000, 5000, 0, 0 };//load color effect
	seqs2[1] = { FLOWTHROUGH, numPixels2, "0,2,3,2,0 ", 200, 5000, 0, 20 };//flowthrough effect
	//seqs2[2] = { CLEAR, numPixels2, " ", 0, 0, 0, 0 };//clear effect
	seqs2[2] = { BOUNCEBACK, numPixels2, "0,3,3,2,0 ", 200, 8000, 8, 8 };//bounceback effect
	//seqs2[4] = { RAINBOW, numPixels2, "5,4,3,2,1 ", 200, 6000, 1, 2 };//rainbow effect

	//Initiliaze strip 2
	strips[1] = Strip(numPixels2, 4, 5, DOTSTAR_RGB, seqs2, numEffects2);
	strips[1].getStrip()->begin();
	strips[1].getStrip()->show();
	
	//Initiliaze effects manager
	effectsManager = EffectsManager(strips, numStrips);
	effectsManager.effectsManagerUpdateRet = uRet;

	Serial.print("Strip 2 details - the number of sequences are ");
	Serial.println(effectsManager.getStrips()[1].getCountSeqs());
	Serial.println();
	//strip.begin(); // Initialize pins for output
	//strip.show();  // Turn all LEDs off ASAP
}


/*
	After setup is done above, this function loops infinitely. Calling the effectsManager's
	update method to perform the sequences above.
*/
void loop()
{
	//Update elapsed time in EffectsManager object
	localElapsedTime = effectsManager.getElapsedTime(millis());

	effectsManager.Update(localElapsedTime);
	
	Serial.print("Elapsed time is : ");
	Serial.println(localElapsedTime);

	Serial.println(effectsManager.effectsManagerUpdateRet->performanceElapsedTime);
	Serial.println();
}

