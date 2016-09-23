#include <colors.h>
#include <Adafruit_DotStar.h>
#include <avr/power.h>

#define NUMPIXELS 30
#define DATAPIN    4
#define CLOCKPIN   5
#define DATAPIN2   8
#define CLOCKPIN2  6

#define ALLCLEAR   'a'
#define BOUNCEBACK 'b'
#define FLOWTHROUGH 'c'
#define SELECTPIXEL 'd'
#define SELECTPIXELSHOW 'e'
#define LOADCOLOR 'f'
#define CDELAY 'g'
#define DELAY 'h'         
 

Adafruit_DotStar strip = Adafruit_DotStar( NUMPIXELS, DATAPIN, CLOCKPIN, DOTSTAR_RGB);



struct strips{
uint16_t dpin;
uint8_t cpin;
uint8_t nled;
uint8_t cscheme;
};

struct instruction{
  char function;
  uint32_t color;
  uint32_t delayTime;
  int itterations;
  uint16_t pixels;
};


struct strips strip1;
struct instruction instructions[10];


void setup() {

  

  strip1.dpin = 4;
  strip1.cpin = 5;
  strip1.nled = 30;
  strip1.cscheme = DOTSTAR_RGB;
  
  /*Adafruit_DotStar*/ strip = Adafruit_DotStar( strip1.nled, strip1.dpin, strip1.cpin, strip1.cscheme);

  
  #if defined(__AVR_ATtiny85__) && (F_CPU == 16000000L)
  clock_prescale_set(clock_div_1); // Enable 16 MHz on Trinket
  #endif


  



  strip.begin();  //initialize pins for output
  

  strip.setBrightness(5);  //set brightness, duh

  strip.show(); //turn off all LED immediately

  
  
}

  

void loop() {

 

 

  instructions[0].function = FLOWTHROUGH;
  instructions[0].color = GREEN;
  instructions[0].delayTime = 100;
  instructions[0].itterations = 2;
  instructions[0].pixels = 5;

 instructions[1].function = ALLCLEAR;


 instructions[2].function = BOUNCEBACK;
 instructions[2].color = WHITE;
 instructions[2].delayTime = 50;
 instructions[2].pixels = 3;
 instructions[2].itterations = 2;

 instructions[3].function = SELECTPIXELSHOW;
 instructions[3].color = RED;
 instructions[3].delayTime = 50;
 instructions[3].pixels = 28;
 instructions[3].itterations = 2;

 instructions[4].function = SELECTPIXELSHOW;
 instructions[4].color = CYAN;
 instructions[4].delayTime = 50;
 instructions[4].pixels = 25;
 instructions[4].itterations = 2;


 instructions[5].function = BOUNCEBACK;
 instructions[5].color = RED;
 instructions[5].delayTime = 50;
 instructions[5].pixels = 5;
 instructions[5].itterations = 4;


 instructions[6].function = CDELAY;
 instructions[6].delayTime = 10000;


 instructions[7].function = SELECTPIXELSHOW;
 instructions[7].color = RED;
 instructions[7].delayTime = 50;
 instructions[7].pixels = 28;
 instructions[7].itterations = 2;

 instructions[8].function = SELECTPIXELSHOW;
 instructions[8].color = CYAN;
 instructions[8].delayTime = 50;
 instructions[8].pixels = 25;
 instructions[8].itterations = 2;


 instructions[9].function = DELAY;
 instructions[9].delayTime = 10000;




  int m;
 for(m = 0; m < 10; m++)
  handler(instructions[m]);
   
}


void handler(struct instruction instructions) {

  if(instructions.function == LOADCOLOR)
    loadColor(instructions.color, instructions.delayTime);
    
  else if(instructions.function == ALLCLEAR)
    allClear();
    
  else if(instructions.function == BOUNCEBACK)
    bounceBack(instructions.color, instructions.delayTime, instructions.itterations, instructions.pixels);
    
  else if(instructions.function == FLOWTHROUGH)
    flowThrough(instructions.color, instructions.delayTime, instructions.itterations, instructions.pixels);
    
  else if(instructions.function == SELECTPIXEL)
    selectPixel(instructions.pixels, instructions.color);
    
  else if(instructions.function == SELECTPIXELSHOW)
    selectPixelShow(instructions.pixels, instructions.color);
    
  else if(instructions.function == CDELAY)  
    cleardelay(instructions.delayTime);
  else if(instructions.function == DELAY)
    delay(instructions.delayTime);
  else
    rainbow(2, 300);
  
  
}

/*
 * Clear the strip, then delay.
 */
void cleardelay(uint32_t delayt)
{
  allClear();
  delay(delayt);
}

/*
 * Turns designated pixel into a color, and immediately shows
 */
void selectPixelShow(uint16_t pixel, uint32_t color) {
  
  strip.setPixelColor(pixel, color);
  strip.show();
}

/*
 * Turns designated pixel into a color, does not show
 */
void selectPixel(uint16_t pixel, uint32_t color) {
  
  strip.setPixelColor(pixel, color);
  
}

/*
 * Update strip
 */
void show() {
  strip.show();
}

/*
 * Clear entire strip
 */
void allClear() {
  for(int i = 0; i < NUMPIXELS; i++)
    strip.setPixelColor(i, CLEAR);
    strip.show();
}

/*
 * Rainbow for testing purposes. If we get to a rainbow without it being designated, something wrong happened.
 */
void rainbow(int itterations, uint32_t delayTime) {
int p0 = 0, p1 = 1, p2 = 2, p3 = 3, p4 = 4, p5 = 5;
for(int i = 0; i < itterations; i++)
 {
  for(int j = 0; j < NUMPIXELS; j++)
  {
    strip.setPixelColor(p0++, 0x000000);
    strip.setPixelColor(p1++, 0xFF0000);
    strip.setPixelColor(p2++, 0xFFA500);
    strip.setPixelColor(p3++, 0xFFFF00);
    strip.setPixelColor(p4++, 0x008000);
    strip.setPixelColor(p5++, 0x0000FF);
    strip.setPixelColor(p1-2, 0x000000);

    if(i+1 != itterations){
    if(p0 == 30)
      p0 = 0;
    if(p1 ==30)
      p1 = 0;
    if(p2 == 30)
      p2 = 0;
    if(p3 == 30)
      p3 = 0;
    if(p4 == 30)
      p4 = 0;
    if(p5 == 30)
      p5 = 0;
    }
    
    strip.show();
    delay(delayTime);
  }
 }

 p0 = -1;
 p1 = 0;
 p2 = 1;
 p3 = 2;
 p4 = 3;
 p5 = 4;
}

/*
 * Make entire strip a color for a certain amount of time
 */
void loadColor(uint32_t color, uint32_t delayTime) {
  for( int i = 0; i < NUMPIXELS; i++)
  {
    strip.setPixelColor(i, color);
    strip.show();
    delay(delayTime);
  }
}

/*
 * Bounce back and forth between beginning and end of strip
 * Possible expansion: Designate beginning and ending points
 */
void bounceBack(uint32_t color, uint32_t delayTime, int bounces, uint16_t pixels) {

  boolean forward = true;
  int tail = 0;
  int head = pixels;
  for(int i = 0; i < pixels; i++) {
    strip.setPixelColor(i, color);
  }

  strip.show();

  while(bounces > 0) {

    while(forward) {
      if(head < NUMPIXELS) {
        strip.setPixelColor(head++, color);
        strip.setPixelColor(tail++, CLEAR);
      }
      else {
        forward = false;
      }

      strip.show();
      delay(delayTime);
    }
    while(!forward) {
      if(tail >= 0) {
        strip.setPixelColor(head--, CLEAR);
        strip.setPixelColor(tail--, color);
      }
      else {
        forward = true;
      }

      strip.show();
      delay(delayTime);
    }

    bounces--;
  }
}

/*
 * Color bar of length pixels flows through entire strip a designated amount of times, delaying between updates
 */
void flowThrough(uint32_t color, uint32_t delayTime, int itterations, uint16_t pixels) {
  int head = 0;
  int tail = 0 - pixels;
  

for(int i = 0; i < itterations; i++) {
  for(int j = 0; j < NUMPIXELS; j++) {
    strip.setPixelColor(head++, color);
    strip.setPixelColor(tail++, CLEAR);

    if(i+1 != itterations) {
    if(head >= NUMPIXELS) {
      head = 0;
    }
    }
    if(tail >= NUMPIXELS) {
      tail = 0;
    }
    

    strip.show();
    delay(delayTime);
  }
}
}

