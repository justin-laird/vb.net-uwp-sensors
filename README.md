# VB.Net Windows 10 IoT Core UWP Sensors from Adafruit - Tested with a Raspberry Pi 3

A collection of VB.Net classes that I've written for several sensors since there is a general lack of VB.Net examples for Windows 10 IoT Core and UWP.

Current Sensors include:
  * AM2315 - I2C Temperature/Humidity Sensor
  * MCP3008 - 8-Channel 10-Bit ADC With SPI Interface
  * SI1145 - Digital UV Index / IR / Visible Light Sensor
  * SI7021 - I2C Temperature & Humidity Sensor
  * VL53L0X - Time of Flight Distance Sensor

I've referenced many sources in python, C#, and Java in an effort to get these working. All the classes work for me at the time of this writing, although some(all?) of them could be cleaned up and improved but this is enough to get most people started. I'm not sure I'll keep these updated unless there is a noticable interest in them.

Sensor notes:

  * AM2315 sensor is flaky, it's important to add a total of 2 10K ohm resistors even though Adafruit's walkthrough said they're not needed on a Raspberry Pi. One from VCC to SDA and the other from VCC to SCL. This was the ONLY way I could get this sensor to work. I also doubt that I'm sending the correct byte array {&H3, &H0, &H4} for the write. It works on about 50% of the write and read commands and throws two different exceptions: 'System.IO.FileNotFoundException' and 'Slave address was not acknowledged' so I just account for this in the error handling. I have a feeling the sensor needs a wakeup command first, followed by a command to return data. According to Adafruit it can only capture one reading every 2 seconds, with a 10ms delay between the write and read. The Adafruit settings seem to provide the least amount of exceptions. During my testing anything above a 5 second wait between readings would not capture any readings at all and only throw slave address exceptions. Also anything above 30ms or below 5ms wait to read after the write would throw similar exceptions.
  
    First create your object:
    ```vb
    Private AM2315_DEVICE As AM2315_CLASS = New AM2315_CLASS()
    ```
    
    Then initialize your sensor:
    ```vb
    Await Task.WhenAll(AM2315_DEVICE.INITIALIZE_AM2315_ASYNC())
    ```
    
    Finally get your data:
    ```vb
    Dim AM2315_DATA As AM2315_Data = AM2315_DEVICE.READ_PROBE_DATA()
    Dim DEC_TEMP As Decimal = Math.Round((AM2315_DATA.TEMPERATURE * (9 / 5)) + 32, 2)
    Dim DEC_RH As Decimal = Math.Round(AM2315_DATA.HUMIDITY, 2)
     ```
    
    
  * VL53L0X sensor is complicated and there is little official documentation for it. My code only has it working with the default settings. I would love for someone to figure out the long range settings because my code does not do that.
    
    First create your object:
    ```vb
    Private VL53L0X_DEVICE As VL53L0X_CLASS = New VL53L0X_CLASS()
    ```
    
    Then initialize your sensor:
    ```vb
    Await Task.WhenAll(VL53L0X_DEVICE.INITIALIZE_ASYNC(False, 0))
    ```
    
    Finally get your data:
    ```vb
    Dim VL53L0X_DATA As VL53L0X_CLASS.VL53L0XData = VL53L0X_DEVICE.READ()
     ```
     
     
  * SI1145 sensor is also complicated but I believe I have it working correctly. The chip has to be reset on initialization and also have the default settings added back on initialization before proper readings can take place, otherwise it'll just read zeros everytime. 
    
    First create your object:
    ```vb
    Private SI1145_DEVICE As New SI1145_CLASS()
    ```
    
    Then initialize your sensor:
    ```vb
    Await Task.WhenAll(SI1145_DEVICE.Initialize_SI1145_Async())
    ```
    
    Finally get your data:
    ```vb
    Dim SI1145_DATA As SI1145_Data = SI1145_DEVICE.READ_PROBE_DATA()
     ```
     
     
  * SI7021 is easy to work with and documented all over the place. 
      
    First create your object:
    ```vb
    Private SI7021_DEVICE As New SI7021_CLASS()
    ```
    
    Then initialize your sensor:
    ```vb
    Await Task.WhenAll(SI7021_DEVICE.Initialize_SI7021_Async())
    ```
    
    Finally get your data:
    ```vb
    Dim SI7021_DATA As SI7021_Data = SI7021_DEVICE.Read()
    DEC_RH = Math.Round(SI7021_DATA.HUM_INT, 2)
    DEC_TEMP = Math.Round((SI7021_DATA.TEMP_INT * (9 / 5)) + 32, 2)
     ```
     
     
  * MCP3008 is also easy to work with, the code is broken into two different classes but could be combined. I was testing the MCP3008 with a water level sensor and it was giving me weird numbers, but I found out the problem was the water level meter, not the MCP3008. I just never put the code 100% back together because I went a different direction with a more accurate sensor (VL53L0X). The code was adapted from a couple different C# examples but I can not find the originals. If you recoginize it please let me know so I can give proper credit. 

    First create your object:
    ```vb
    Private MCP3008_DEVICE As MCP3008_CLASS = New MCP3008_CLASS(0)
    ```
    
    Then initialize your sensor:
    ```vb
    Await Task.WhenAll(MCP3008_DEVICE.Initialize())
    ```
    
    Finally get your data:
    ```vb
    Dim MCP3008_DATA As MCP3008_READING = MCP3008_DEVICE.Read()
     ```
