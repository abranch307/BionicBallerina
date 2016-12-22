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

//WiFi handling specific variables
bool endSeqs = false, firstRX = true;

//End of WiFi Setup***********************************************************************************************************

//Allocate Lighting Effect Memory for MCU: MCU1**************************************************************************************

//Effects specific variables
unsigned long localElapsedTime, temp;
int8_t  numStrips = 1, numEffects1 = 17;
uint16_t numPixels1 = 26;

//Allocate memory for effects manager and steup
EffectsManager effectsManager;
EffectsManagerUpdateReturn *uRet = (EffectsManagerUpdateReturn*)calloc(1, sizeof(EffectsManagerUpdateReturn));

//Allocate memory for strips
Strip *strips = (Strip*)calloc(numStrips, sizeof(Strip));

//Allocation memory for Lighting Sequences. Number of seqs variables must equal numStrips.
LightingSequence* seqs1 = (LightingSequence*)calloc(numEffects1, sizeof(LightingSequence));

//End of Allocate Lighting Effect Memory for MCU: MCU1**************************************************************************************

//Tenp variables
uint16_t elapsedTime, prevTime;

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

  //Initialize Lighting Effects for MCU: MCU1*****************************************************************************************

  seqs1[0] = {FLOWTHROUGH, numPixels1, "4,0,4,0,4,0,4,0,4,0,4,0,4,0,4,0,4,0,4,0 ", 800, 9000, 100, 100, 100, 20, 800};
  seqs1[1] = {BOUNCEBACK, numPixels1, "4,0,4,0,0,0,0,0,0,0,4,0,4,0,0,0,0,0,0,0 ", 400, 4000, 100, 100, 100, 50, 600};
  seqs1[2] = {FLOWTHROUGH, numPixels1, "4,0,4,4,0,4,0,4,0,4,4,0,4,4,0,4,0,4,0,4 ", 200, 4000, 100, 100, 100, 80, 200};
  seqs1[3] = {BOUNCEBACK, numPixels1, "4,1,4,0,0,0,0,0,0,0,4,1,4,0,0,0,0,0,0,0 ", 100, 4000, 100, 100, 100, 120, 100};
  seqs1[4] = {ALLCLEAR, numPixels1, "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 ", 0, 3000, 0, 0, 255, 0, 0};
  seqs1[5] = {LOADCOLOR, numPixels1, "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 ", 500, 5000, 0, 0, 100, 150, 500};
  seqs1[6] = {LOADCOLOR, numPixels1, "4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4 ", 300, 3000, 0, 0, 255, 150, 300};
  seqs1[7] = {LOADCOLOR, numPixels1, "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 ", 3000, 3000, 0, 0, 150, 100, 300};
  seqs1[8] = {LOADCOLOR, numPixels1, "4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4 ", 7000, 7000, 0, 0, 100, 100, 200};
  seqs1[9] = {LOADCOLOR, numPixels1, "1,4,1,4,1,4,1,4,1,4,1,4,1,4,1,4,1,4,1,4 ", 6000, 6000, 0, 0, 150, 150, 400};
  seqs1[10] = {LOADCOLOR, numPixels1, "4,1,4,1,4,1,4,1,4,1,4,1,4,1,4,1,4,1,4,1 ", 2000, 2000, 0, 0, 150, 150, 400};
  seqs1[11] = {FLOWTHROUGH, numPixels1, "1,1,4,1,1,4,1,1,4,1,1,1,4,1,1,4,1,1,4,1 ", 100, 8000, 100, 100, 150, 150, 100};
  seqs1[12] = {FLOWTHROUGH, numPixels1, "4,4,1,4,4,1,4,4,1,4,4,4,1,4,4,1,4,4,1,4 ", 100, 2000, 100, 100, 150, 150, 100};
  seqs1[13] = {FLOWTHROUGH, numPixels1, "1,1,1,4,1,1,1,4,1,1,1,1,1,4,1,1,1,4,1,1 ", 100, 3000, 100, 100, 150, 200, 100};
  seqs1[14] = {FLOWTHROUGH, numPixels1, "4,1,4,1,4,1,4,1,4,1,4,1,4,1,4,1,4,1,4,1 ", 100, 2000, 100, 100, 150, 200, 100};
  seqs1[15] = {BOUNCEBACK, numPixels1, "0,0,0,4,1,1,4,0,0,0,0,0,0,4,1,1,4,0,0,0 ", 100, 6000, 100, 100, 150, 200, 100};
  seqs1[16] = {BOUNCEBACK, numPixels1, "0,4,4,1,1,1,1,4,4,0,0,4,4,1,1,1,1,4,4,0 ", 100, 7000, 100, 100, 255, 255, 100};
  
  //End of Initialize Lighting Effects for MCU: MCU1*****************************************************************************************
  
  //Initialize Strips for MCU: MCU1***************************************************************************************************
  
  strips[0] = Strip(numPixels1, 4, 5, DOTSTAR_RGB, seqs1, numEffects1);
  strips[0].getStrip()->begin();
  strips[0].getStrip()->show();
  
  
  //End of Initialize Strips for MCU: MCU1***************************************************************************************************

   
  //Initialize effects manager
  effectsManager = EffectsManager(strips, numStrips);
  effectsManager.effectsManagerUpdateRet = uRet;
  
  //End of Initialize Effects Manager for MCU: MCU1***************************************************************************************

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

