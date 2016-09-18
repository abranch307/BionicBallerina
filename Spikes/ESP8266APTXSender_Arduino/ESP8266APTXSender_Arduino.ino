/*
 * Copyright (c) 2015, Majenko Technologies
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 * 
 * * Redistributions of source code must retain the above copyright notice, this
 *   list of conditions and the following disclaimer.
 * 
 * * Redistributions in binary form must reproduce the above copyright notice, this
 *   list of conditions and the following disclaimer in the documentation and/or
 *   other materials provided with the distribution.
 * 
 * * Neither the name of Majenko Technologies nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

/* Create a WiFi access point and provide a web server on it. */

#include <ESP8266WiFi.h>
#include <WiFiClient.h> 
#include <ESP8266WebServer.h>

/* Set these to your desired credentials. */
char ssid[20] = "SeniorDesign4900LED";
//String password = "sdled";

String userInput = "", ledForm = "", userForm = "", rootForm = "";

struct userInfo{
  char username[20];
  int num;
};

ESP8266WebServer server(80);

/* Just a little test message.  Go to http://192.168.4.1 in a web browser
 * connected to this access point to see it.
 */
void handleRoot() {
  //Show user navigation page to all functions
  rootForm = "<form action=\"/\" method=\"get\">";
  rootForm += "<h1>You are connected</h1><br><br>";
  rootForm += "<h1>Navigate to required page from here...</h1><br><br>";
  rootForm += "<button type=\"submit\" formaction=\"/LED\"> LED State Here </button><br>";
  rootForm += "<button type=\"submit\" formaction=\"/USERINFO\"> User Info Here </button><br>";
  rootForm += "</form>";
  
  //Send response to user
	server.send(200, "text/html", rootForm);

  //Blank variable to save space
  rootForm = "";
}

//This method will send a 0 or 1 to Pro Trinket 5v, making pin
//8 turn on or of accordingly
void handleLED(){
  //Verify that led argument is present
  if(server.hasArg("LED")){
    //Set led value to state (should be 0 or 1)
    int state = server.arg("LED").toInt();

    //Wait until serial reading is done
    while(Serial.available());
    //Send 10 character code [LED] to Pro Trinket 5v so it knows
    //it will be receiving an int value
    userInput = "";
    userInput += "LED\n" + String(state);
    Serial.println(userInput);

    //Send message back to user
    server.send(200, "text/html", userInput);
  }

  //Send LED form back to user
  ledForm = "<form action=\"/LED\" method=\"get\">";
  ledForm += "LED (0 or 1): ";
  ledForm += "<input type=\"text\" name=\"LED\" value=\"0\">";
  ledForm += "<input type=\"submit\" value=\"Submit\">";
  ledForm += "</form>";
  
  server.send(200, "text/html",ledForm);
  
  ledForm = "";
}

void handleUserInfo(){
  //Process request if username and num arguments found
  if(server.hasArg("USERNAME") && server.hasArg("NUM")){
    userInfo ui;
    
    server.arg("USERNAME").toCharArray(ui.username, (server.arg("USERNAME").length()+1));
    ui.num = server.arg("NUM").toInt();
    
    while(Serial.available());
    userInput = "";
    userInput += "USERINFO\n" + String(ui.username) + "\n" + String(ui.num);
    Serial.println(userInput);

    //For testing purposes only
    userInput = "The username is " + String(ui.username);
    userInput += " and the number is " + String(ui.num);
    //Serial.println(userInput);

    //Send message back to user
    server.send(200, "text/html", userInput);
  }

  //Send user form back to user
  userForm = "<form action=\"/USERINFO\" method=\"get\">";
  userForm += "Username: ";
  userForm += "<input type=\"text\" name=\"USERNAME\">";
  userForm += "Favorite Number: ";
  userForm += "<input type=\"text\" name=\"NUM\">";
  userForm += "<input type=\"submit\" value=\"Submit\">";
  userForm += "</form>";
  
  server.send(200, "text/html",userForm);
  
  userForm = "";
}

void setup() {
	delay(1000);
	Serial.begin(115200);
	Serial.println();
	Serial.print("Configuring access point...");
	/* You can remove the password parameter if you want the AP to be open. */
	WiFi.softAP(ssid);

	IPAddress myIP = WiFi.softAPIP();
	Serial.print("AP IP address: ");
	Serial.println(myIP);
	server.on("/", handleRoot);
  server.on("/LED", handleLED);
  server.on("/USERINFO", handleUserInfo);
	server.begin();
	Serial.println("HTTP server started");
  userInput.reserve(200);
  ledForm.reserve(200);
  userForm.reserve(200);
  rootForm.reserve(200);
}

void loop() {
	server.handleClient();
}

//Handle RX interrupt

