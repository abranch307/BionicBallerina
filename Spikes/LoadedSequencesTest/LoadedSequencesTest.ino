#include <colors.h>
#include <Adafruit_DotStar.h>
#include <avr/power.h>

#define NUMPIXELS 30
#define DATAPIN    4
#define CLOCKPIN   5
#define DATAPIN2   8
#define CLOCKPIN2  6

Adafruit_DotStar strip = Adafruit_DotStar( NUMPIXELS, DATAPIN, CLOCKPIN, DOTSTAR_RGB);

void setup() {
  #if defined(__AVR_ATtiny85__) && (F_CPU == 16000000L)
  clock_prescale_set(clock_div_1); // Enable 16 MHz on Trinket
  #endif

  strip.begin();  //initialize pins for output

  strip.setBrightness(10);  //set brightness, duh

  strip.show(); //turn off all LED immediately
  
}

int i;

int
  p0 = -1,
  p1 = 0,
  p2 = 1,
  p3 = 2,
  p4 = 3,
  p5 = 4;

  

void loop() {
            //color/delay/itterations/pixels
 loadColor(WHITE,50);
 flowThrough(BLUE, 50, 2, 5);
 allClear();
 bounceBack(GREEN, 50, 1, 5);
 allClear();
 loadColor(RED, 50);
 flowThrough(CYAN, 50, 2, 5);
 allClear();
 rainbow(1, 50);
 allClear();
}

void allClear() {
  for(i = 0; i < NUMPIXELS; i++)
    strip.setPixelColor(i, 0x000000);
    strip.show();
}

void rainbow(int itterations, int delayTime) {

for(i = 0; i < itterations; i++)
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

void loadColor(uint32_t color, int delayTime) {
  for( i = 0; i < NUMPIXELS; i++)
  {
    strip.setPixelColor(i, color);
    strip.show();
    delay(delayTime);
  }
}

void bounceBack(uint32_t color, int delayTime, int bounces, int pixels) {

  boolean forward = true;
  int tail = 0;
  int head = pixels;
  for(i = 0; i < pixels; i++) {
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

void flowThrough(uint32_t color, int delayTime, int itterations, int pixels) {
  int head = 0;
  int tail = 0 - pixels;

for(i = 0; i < itterations; i++) {
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

