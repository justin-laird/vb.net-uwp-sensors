
''' <summary>
''' Contains the values read from a channel on the MCP3008.
''' </summary>
Public Class MCP3008_READING
    ''' <summary>
    ''' Gets the actual value read from the channel.
    ''' </summary>
    ''' <returns></returns>
    Public Property RAW_VALUE As Integer

    ''' <summary>
    ''' Gets a normalized value in the range of 0 to 1.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property NormalizedValue As Single
        Get
            Return RAW_VALUE / 1023.0F
        End Get
    End Property

    ''' <summary>
    ''' Creates a default instance of Windows.Devices.Sensors.Mcp3008Reading.
    ''' </summary>
    Sub New()
    End Sub

    ''' <summary>
    ''' Creates a default instance of Windows.Devices.Sensors.Mcp3008Reading
    ''' with the given raw value.
    ''' </summary>
    ''' <param name="rawValue"></param>
    Sub New(ByVal rawValue As Integer)
        RAW_VALUE = rawValue
    End Sub

End Class
