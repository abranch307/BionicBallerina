// Effects.h

#ifndef _EFFECTS_h
#define _EFFECTS_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#ifndef _ADAFRUIT_DOT_STAR_H_
	#include <Adafruit_DotStar\Adafruit_DotStar.h>
#endif

#ifndef _STRUCTS_h
	#include "Structs.h"
#endif

class Strip;

//Colors
#define CLEAR	0x000000
#define WHITE	0xFFFFFF
#define RED		0xFF0000
#define GREEN	0x00FF00
#define BLUE	0x0000FF
#define YELLOW	0xFFFF00
#define CYAN	0x00FFFF
#define MAGENTA	0xFF00FF
#define ORANGE  0xFFA500

//Lighting Effect Types
#define ALLCLEAR 0
#define RAINBOW 1
#define LOADCOLOR 2
#define BOUNCEBACK 3
#define FLOWTHROUGH 4

class Effects {

	public:
		static void allClear(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t* currentSequence);
		static void rainbow(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t* currentSequence, uint16_t* i, uint16_t* p0, uint16_t* p1, uint16_t* p2, uint16_t* p3, uint16_t* p4, uint16_t* p5);
		static void loadColor(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t* currentSequence);
		static void bounceBack(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t* currentSequence);
		static void flowThrough(Adafruit_DotStar* strip, LightingSequence* lseqs, uint16_t* currentSequence);
	private:
		//Disallow creating instance of object
		Effects() {};
	protected:

};

#endif

