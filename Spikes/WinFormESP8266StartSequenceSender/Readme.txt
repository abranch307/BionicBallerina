WinForm ESP8266 Start Sequence Sender

This program will present a Windows Form where the user can enter the number of MCUs in which to send upcoming action to, the ip address of each microcontroller, the action to perform (Start LED Sequence, Stop LED Sequence, Restart LED Sequence), and send the action to an ESP8266 module which will then send the command via an HTTP request to the attached MCU via serial.

In the sending stage, a thread is created for every ip address.  An initial request is sent to every microcontroller and every thread waits until it receives a response from its designated ip address.  (The ESP8266 will block and wait for upcoming command).  Once all threads have confirmation, the action will be sent to all threads at once via an HTTP request.