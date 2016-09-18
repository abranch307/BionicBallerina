
//Setup struct for userinformation
struct userInfo{
  char username[20];
  int num;
};
enum UserMethods{LED, USERINFO};

String inputString = "";         // a string to hold incoming data
boolean stringComplete = false;  // whether the string is complete

void setup() {
  //Turn on hardware serial interface (RX & TX)
  Serial.begin(115200);

  //Turn A0 on for ESP8266 CH_P
  pinMode(A0, OUTPUT);
  digitalWrite(A0, HIGH);
  
  // reserve 200 bytes for the inputString:
  inputString.reserve(200);
}

void loop() {
  if (stringComplete) {
    Serial.println(inputString);
    // clear the string:
    inputString = "";
    stringComplete = false;
  }
}

//This method is a built in function that handles RX data coming in
void serialEvent(){
  String str;
  byte buff[200];
  boolean methFound = false;
  int state, i = 0;
  UserMethods meth;

  Serial.println("I made into interrupt...");
  stringComplete = true;
  
  //Loop through received data
  while(Serial.available()){
    //If a method has been found, then skip to use method, otherwise
    //keep looking for method in RX info
    if(!methFound){
      //Read string from RX
      str = Serial.readStringUntil('\n');
      str.trim();
      inputString += "The read string was " + str + "\n";
      
      //Verify if valid method found
      if(str.equals("LED")){
        //Serial.println("LED string found!");
        inputString += "LED string found!\n";
        meth = LED;
        methFound = true;
      }else if(str.equals("USERINFO")){
        //Serial.println("USERINFO string found!");
        inputString += "USERINFO string found!\n";
        meth = USERINFO;
        methFound = true;
      }
    }else{
      inputString += "I'm in the meth found portion!\n";
      switch(meth){
        case LED:
          //Read int into state and change pin
          state = Serial.parseInt();
          inputString += "The state will be changed to " + String(state);
          if(state == 0){
            pinMode(8,OUTPUT);
            digitalWrite(8,LOW);
          }else{
            pinMode(8,OUTPUT);
            digitalWrite(8,HIGH);
          }
          break;
        case USERINFO:
          userInfo ui;
          String tstr;
          tstr = Serial.readStringUntil('\n');
          tstr.toCharArray(ui.username, (tstr.length()+1));
          tstr = Serial.readStringUntil('\n');
          ui.num = tstr.toInt();

          inputString += "The username is " + String(ui.username) + ", ";
          inputString += "The number is " + String(ui.num);
          
          break;
      }
    }
  }
}

