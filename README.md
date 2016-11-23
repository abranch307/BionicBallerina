# DotStar Composer
Western Michigan University Project

Our client, Juliana Payson, is a hip replacement recovery patient who found a deeper love for aerobics and dance during her rehabilitation.  This project was developed to assist her in creating a system to control and synchronize DotStar leds via WiFi that are attached to multiple costumes with music playing from a laptop.  There is not an easy, off-the-shelf solution, so we designed one to fit her needs.

Key features in this project are:

  1. Compose, simulate, and generate sequenced lighting effects for DotStar LEDs on a compuater via a graphical user interface
  2. Store composed lighting effect projects into a database
  3. Load composed lighting effects onto Pro Trinket microcontroller
  4. Play composed lighting effects performance from Pro Trinket onto DotStar LEDs while staying synchronized with music via computer, WiFi Access Point, and ESP8266 WiFi module
  5. Hardware on performers are powered by a Lithium Ion Polymer battery, which is protected by PCMs (Protection Circuit Modules) from over-current, under-current, and over-discharge of cells

Some major components used/developed in this project are:

  1.  Laptop/Computer with wireless network card
  2.  DotStar Effects Composer (Windows Form Graphical User Interface)
  3.  MariaDB Database
  4.  Arduino Desktop App
  5.  WiFi Access Point (802.11n recommended w/ Omni-directional antennas)
  6.  ESP8266 WiFi Module (3.3V)
  7.  ESP8266 WiFi Module HTTP Handler
  8.  Pro Trinket microcontroller (5V)
  9.  Pro Trinket WiFi Module RX and Performance Manager
  10. DotStar LEDs
  11. Lithium Ion Polymer Battery
  12. Battery Balance Charger
  13. DC ?V to 5V DC Converter (UBEC - Universal Battery Elimination Circuit)
  14. Protection Circuit Module (PCM)
  15. Logic Level Converter (Bi-directional 3.3V to 5V)
  
See Operations_Maintenance_Manual in /Documenation directory
