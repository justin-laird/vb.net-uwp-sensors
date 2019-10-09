# vb.net-uwp-sensors
VB.Net Windows 10 IoT Core UWP Sensors from Adafruit - Tested with a Raspberry Pi 3

A collection of VB.Net classes that I've written for several sensors since there is a general lack of VB.Net examples for Windows 10 IoT Core and UWP.

Current Sensors include:
  * AM2315 - I2C Temperature/Humidity Sensor
  * MCP3008 - 8-Channel 10-Bit ADC With SPI Interface
  * SI1145 - Digital UV Index / IR / Visible Light Sensor
  * SI7021 - I2C Temperature & Humidity Sensor
  * VL53L0X - Time of Flight Distance Sensor

I've referenced many sources in python, C#, and Java in an effort to get these working. They all work for me at the time of this writing, although some(all?) of them could probably be cleaned up and improved. 

Sensor notes:

  * AM2315 sensor is flaky, it's important to add a total of 2 10K ohm resistors. One from VCC to SDA and the other from VCC to SCL. This was the ONLY way I could get this sensor to work. I also doubt that I'm sending the correct byte array {&H3, &H0, &H4} for the write. It works on about 50% of the write and read commands and throws two different exceptions: 'System.IO.FileNotFoundException' and 'Slave address was not acknowledged' so I just account for this in the error handeling. According to Adafruit it can only capture one reading every 2 seconds, with a 10ms delay between the write and read. The Adafruit settings seem to provide the least amount of exceptions. During my testing anything above a 5 second wait between readings would not capture any readings at all and only throw slave address exceptions. Also anything above 30ms or below 5ms wait to read after the write would throw similar exceptions.
  * VL53L0X sensor is complicated and there is little official documentation for it. My code only has it working with the default settings. I would love for someone to figure out the long range settings because my code does not do that.
  * SI1145 sensor is also complicated but I believe I have it working correctly. The chip has to be reset on initialization and also have the default settings added back on initialization before proper readings can take place, otherwise it'll just read zeros everytime. 
  * SI7021 is easy to work with and documented all over the place. 
  * MCP3008 is also easy to work with, the code is broken into two different classes but could be combined. I was testing the MCP3008 with a water level sensor and it was giving me weird numbers, but I found out the problem was the water level meter, not the MCP3008. I just never put the code 100% back together. 
