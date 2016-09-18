// Strip.h

#ifndef _STRIP_h
#define _STRIP_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

//#ifndef _SEQUENCESCHEDULER_h
	#include "SequenceScheduler.h"
//#endif

#ifndef _ADAFRUIT_DOT_STAR_H_
	#include "Adafruit_DotStar\Adafruit_DotStar.h"
#endif

//Colors
#define CLEAR	0x000000
#define WHITE	0xFFFFFF
#define RED		0xFF0000
#define GREEN	0x00FF00
#define BLUE	0x0000FF
#define YELLOW	0xFFFF00
#define CYAN	0x00FFFF
#define MAGENTA	0xFF00FF

//Lighting Effect Types
#define ALLCLEAR 0
#define RAINBOW 1
#define LOADCOLOR 2
#define BOUNCEBACK 3
#define FLOWTHROUGH 4

//136 bits = 17 bytes
struct LightingEffect {
	uint8_t lightsequence;
	uint16_t totPixels;
	uint16_t effectPixels;
	uint32_t color;
	uint16_t delayTime;
	uint16_t duration;
	uint16_t bounces;
	uint16_t iterations;
};

class StripClass
{
	protected:

	public:
		StripClass();
		boolean UpdateStripData(StripInfo *stripData, LightingEffect *effects);
		void updateCounter();

	private:
		void allClear();
		void rainbow(uint16_t iterations, uint16_t delayTime);
		void loadColor(uint32_t color, uint16_t delayTime);
		void bounceBack(uint32_t color, uint16_t delayTime, uint16_t bounces, uint16_t pixels);
		void flowThrough(uint32_t color, uint16_t delayTime, uint16_t iterations, uint16_t pixels);
		void resetGlobalVars();
};

//extern StripClass Strip;

#endif

