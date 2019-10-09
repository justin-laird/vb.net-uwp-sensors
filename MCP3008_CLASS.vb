Imports Windows.Devices.Enumeration
Imports Windows.Devices.Spi
Imports System.Collections.Generic

''' <summary>
''' Defines an interface to the MCP3008 ADC via the SPI interface.
''' </summary>
Public Class MCP3008_CLASS
    Implements IDisposable
    Private SPI_SETTINGS As SpiConnectionSettings = Nothing
    Private SPI_DEVICE As SpiDevice = Nothing

    ''' <summary>
    ''' Gets the settings used on the SPI interface to communicate to the MCP3008.
    ''' </summary>
    ''' <returns></returns>
    Public Property Settings As SpiConnectionSettings
        Get
            Return SPI_SETTINGS
        End Get
        Private Set(ByVal value As SpiConnectionSettings)
            SPI_SETTINGS = value
        End Set
    End Property

    ''' <summary>
    ''' Gets the underlying SpiDevice instance used by this instance
    ''' of Windows.Devices.Sensors.Mcp3008.
    ''' </summary>
    ''' <returns></returns>
    Public Property Device As SpiDevice
        Get

            If SPI_DEVICE Is Nothing Then
                'Throw New NotInitializedException()
            End If

            Return SPI_DEVICE
        End Get
        Private Set(ByVal value As SpiDevice)
            SPI_DEVICE = value
        End Set
    End Property

    Public Enum InputConfiguration
        SingleEnded = 1
        Differential = 0
    End Enum

    Public Class Channel
        Public Property CH_ID As Integer
        Public Property InputConfiguration As InputConfiguration

        Friend Sub New(ByVal selection As InputConfiguration, ByVal id As Integer)
            CH_ID = id
            InputConfiguration = selection
        End Sub

    End Class

    ''' <summary>
    ''' Initializes a new instance of the Windows.Devices.Sensors.Mcp3008 with 
    ''' the specified Chip Select Line (0 or 1).
    ''' </summary>
    ''' <param name="chipSelectLine">The Chip Select Line the MCP3008 
    ''' is physically connected to. This value is either 0 or 1 on the 
    ''' Raspberry Pi.</param>
    Public Sub New(ByVal chipSelectLine As Integer)
        Settings = New SpiConnectionSettings(chipSelectLine) With {.ClockFrequency = 1000000, .Mode = SpiMode.Mode0, .SharingMode = SpiSharingMode.Exclusive}
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the Windows.Devices.Sensors.Mcp3008 with 
    ''' the specified SpiConnectionSettings.
    ''' </summary>
    ''' <param name="settings">An instance of SpiConnectionSettings that specifies 
    ''' the parameters of the SPI connection for the MCP3008.</param>
    Public Sub New(ByVal settings As SpiConnectionSettings)
        Me.Settings = settings
    End Sub

    ''' <summary>
    ''' Defines all available channels on the MCP3008.
    ''' </summary>
    Class Channels
        ''' <summary>
        ''' A list of all available channels on the MCP3008. Single0 is the first item
        ''' followed by Single1, Single 2 and so on. Differential0 is the 8th item followed by
        ''' Differential1, Differential2 and so on.
        ''' </summary>
        Public ReadOnly All As IList(Of Channel) = New List(Of Channel)(New Channel() {Single0, Single1, Single2, Single3, Single4, Single5, Single6, Single7, Differential0, Differential1, Differential2, Differential3, Differential4, Differential5, Differential6, Differential7})

        ''' <summary>
        ''' Specifies the single Channel 0 (pin 1).
        ''' </summary>
        Public Shared Single0 As Channel = New Channel(InputConfiguration.SingleEnded, 0)
        ''' <summary>
        ''' Specifies the single Channel 1 (pin 2).
        ''' </summary>
        Public Shared Single1 As Channel = New Channel(InputConfiguration.SingleEnded, 1)
        ''' <summary>
        ''' Specifies the single Channel 2 (pin 3).
        ''' </summary>
        Public Shared Single2 As Channel = New Channel(InputConfiguration.SingleEnded, 2)
        ''' <summary>
        ''' Specifies the single Channel 3 (pin 4).
        ''' </summary>
        Public Shared Single3 As Channel = New Channel(InputConfiguration.SingleEnded, 3)
        ''' <summary>
        ''' Specifies the single Channel 4 (pin 5).
        ''' </summary>
        Public Shared Single4 As Channel = New Channel(InputConfiguration.SingleEnded, 4)
        ''' <summary>
        ''' Specifies the single Channel 5 (pin 6).
        ''' </summary>
        Public Shared Single5 As Channel = New Channel(InputConfiguration.SingleEnded, 5)
        ''' <summary>
        ''' Specifies the single Channel6 (pin 7).
        ''' </summary>
        Public Shared Single6 As Channel = New Channel(InputConfiguration.SingleEnded, 6)
        ''' <summary>
        ''' Specifies the single Channel 7 (pin 8).
        ''' </summary>
        Public Shared Single7 As Channel = New Channel(InputConfiguration.SingleEnded, 7)
        ''' <summary>
        ''' Specifies the differential Channel 0 (+) and Channel 1 (-)
        ''' </summary>
        Public Shared Differential0 As Channel = New Channel(InputConfiguration.Differential, 0)
        ''' <summary>
        ''' Specifies the differential Channel 0 (-) and Channel 1 (+)
        ''' </summary>
        Public Shared Differential1 As Channel = New Channel(InputConfiguration.Differential, 1)
        ''' <summary>
        ''' Specifies the differential Channel 2 (+) and Channel 3 (-)
        ''' </summary>
        Public Shared Differential2 As Channel = New Channel(InputConfiguration.Differential, 2)
        ''' <summary>
        ''' Specifies the differential Channel 2 (-) and Channel 3 (+)
        ''' </summary>
        Public Shared Differential3 As Channel = New Channel(InputConfiguration.Differential, 3)
        ''' <summary>
        ''' Specifies the differential Channel 4 (+) and Channel 5 (-)
        ''' </summary>
        Public Shared Differential4 As Channel = New Channel(InputConfiguration.Differential, 4)
        ''' <summary>
        ''' Specifies the differential Channel 4 (-) and Channel 5 (+)
        ''' </summary>
        Public Shared Differential5 As Channel = New Channel(InputConfiguration.Differential, 5)
        ''' <summary>
        ''' Specifies the differential Channel 6 (+) and Channel 7 (-)
        ''' </summary>
        Public Shared Differential6 As Channel = New Channel(InputConfiguration.Differential, 6)
        ''' <summary>
        ''' Specifies the differential Channel 6 (-) and Channel 7 (+)
        ''' </summary>
        Public Shared Differential7 As Channel = New Channel(InputConfiguration.Differential, 7)
    End Class

    ''' <summary>
    ''' Initializes the MCP3008 by establishing a connection on the SPI interface.
    ''' </summary>
    ''' <returns></returns>
    Public Async Function Initialize() As Task
        If SPI_DEVICE Is Nothing Then
            Dim selector As String = SpiDevice.GetDeviceSelector(String.Format("SPI{0}", Me.Settings.ChipSelectLine))
            Dim deviceInfo = Await DeviceInformation.FindAllAsync(selector)
            SPI_DEVICE = Await SpiDevice.FromIdAsync(deviceInfo(0).Id, Settings)
        Else
            'Throw New AlreadyInitializedException()
        End If
    End Function

    ''' <summary>
    ''' Reads an integer value from the specified port. The value of the port can
    ''' be a number from0 to 7.
    ''' </summary>
    ''' <param name="channel">An integer specifying the port to read from. This is
    ''' a value from 0 to 7.</param>
    ''' <returns>The integer value of the specified port.</returns>
    Public Function Read(ByVal channel As Channel) As MCP3008_READING
        Dim returnValue As MCP3008_READING = New MCP3008_READING(0)

        If SPI_DEVICE IsNot Nothing Then
            Dim readBuffer As Byte() = New Byte(2) {}
            Dim writeBuffer As Byte() = New Byte(2) {channel.InputConfiguration, channel.CH_ID + 8 << 4, &H0}

            SPI_DEVICE.TransferFullDuplex(writeBuffer, readBuffer)

            'returnValue.RAW_VALUE = (readBuffer(2) + (readBuffer(1) * 255)) - 255
            returnValue.RAW_VALUE = Convert.ToInt32(BitConverter.ToString(readBuffer, 0).Replace("-", ""), 16)
            'returnValue.RAW_VALUE = readBuffer(2) + ((readBuffer(1) & &H3) << 8)
            'returnValue.RAW_VALUE = (readBuffer(0) & &H1) << 9 Or readBuffer(1) << 1 Or readBuffer(2) >> 7

        Else
            'Throw New NotInitializedException()
        End If

        Return returnValue

    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        If SPI_DEVICE IsNot Nothing Then
            SPI_DEVICE.Dispose()

        End If
    End Sub

End Class
