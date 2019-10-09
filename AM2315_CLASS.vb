Imports Windows.Devices.I2C

Public Structure AM2315_Data
    Public HUMIDITY As Decimal
    Public TEMPERATURE As Decimal
    Public TIMESTAMP As Date
End Structure

Public Class AM2315_CLASS
    Implements IDisposable

    Private ReadOnly AM2315_ADDRESS As Byte = &H5C
    Private AM2315_SENSOR As I2cDevice
    Private IS_INITIALIZED As Boolean = False

    Public Sub Dispose() Implements IDisposable.Dispose
        AM2315_SENSOR.Dispose()
    End Sub

    Public Async Function INITIALIZE_AM2315_ASYNC() As Task
        Dim SETTINGS As New I2cConnectionSettings(AM2315_ADDRESS) With {.BusSpeed = I2cBusSpeed.FastMode}

        Dim CONTROLLER As I2cController = Await I2cController.GetDefaultAsync()
        AM2315_SENSOR = CONTROLLER.GetDevice(SETTINGS)

        IS_INITIALIZED = True

    End Function

    Public Function READ_PROBE_DATA() As AM2315_Data
        Dim PROBE_DATA As New AM2315_Data()
        Dim READ_BUFFER As Byte() = New Byte(7) {}

        AM2315_SENSOR.Write(New Byte() {&H3, &H0, &H4})
        Threading.Thread.Sleep(10)
        AM2315_SENSOR.Read(READ_BUFFER)

        Dim HUMIDITY As Decimal = Math.Round(((READ_BUFFER(2) * 256) + READ_BUFFER(3)) / 10, 2)
        Dim TEMP As Decimal = Math.Round((((READ_BUFFER(4) And &H7F) * 256) + READ_BUFFER(5)) / 10, 2)

        If READ_BUFFER(4) >> 7 > 0 Then TEMP = -TEMP

        PROBE_DATA.HUMIDITY = HUMIDITY
        PROBE_DATA.TEMPERATURE = TEMP
        PROBE_DATA.TIMESTAMP = Date.Now

        Return PROBE_DATA

    End Function

End Class
