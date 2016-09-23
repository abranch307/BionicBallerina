/*
 Name:		ProTrinket_LightingSequencesManager.ino
 Created:	9/22/2016 7:28:29 PM
 Author:	Abranch
*/

// the setup function runs once when you press reset or power the board
// !

#include "Effects.h"
#include "EffectsManager.h"
#include "Strip.h"

Strip strips[2];

void setup() {
	LightingSequence seqs1[4] = { {}, {}, {}, {}};
	LightingSequence seqs[3] = {};
	strips[1] = Strip(30, 4, 5, DOTSTAR_RGB);
	strips[2] = Strip(30, 8, 6, DOTSTAR_RGB);


}

// the loop function runs over and over again until power down or reset
void loop() {
  
}
