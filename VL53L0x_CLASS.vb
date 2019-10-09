Imports Windows.Devices.I2C

Public Class VL53L0X_CLASS
    Implements IDisposable

    Public Structure VL53L0XData
        Public DISTANCE As Integer
        Public AMBIENT As Integer
        Public SIGNAL As Integer
    End Structure

    Private VL530L0X_SENSOR As I2cDevice
    Private Const VL53L0X_ADDR As Byte = &H29
    Private Const VL53L0X_IDENTIFICATION_MODEL_ID As Byte = &HC0
    Private Const VL53L0X_IDENTIFICATION_REVISION_ID As Byte = &HC2
    Private Const VL53L0X_FINAL_RANGE_CONFIG_MIN_COUNT_RATE_RTN_LIMIT As Byte = &H44
    Private Const VL53L0X_FINAL_RANGE_CONFIG_TIMEOUT_MACROP_HI As Byte = &H71

    Public Async Function INITIALIZE_ASYNC(ByVal LONG_RANGE As Boolean, Optional ByVal MCPS_RATE As Decimal = Nothing) As Task
        Dim I2C_SETTINGS = New I2cConnectionSettings(VL53L0X_ADDR) With {.BusSpeed = I2cBusSpeed.FastMode}
        Dim I2C_CONTROLLER As I2cController = Await I2cController.GetDefaultAsync()

        VL530L0X_SENSOR = I2C_CONTROLLER.GetDevice(I2C_SETTINGS)

        If LONG_RANGE = True Then
            If MCPS_RATE <> Nothing Then
                If Not MCPS_RATE < 0 Or Not MCPS_RATE > 511.99 Then
                    VL530L0X_SENSOR.Write(New Byte() {&H0, &H1})
                    VL530L0X_SENSOR.Write(New Byte() {VL53L0X_FINAL_RANGE_CONFIG_MIN_COUNT_RATE_RTN_LIMIT, MCPS_RATE * &H80})

                End If
            End If
        End If
    End Function

    Public Function READ() As VL53L0XData
        VL530L0X_SENSOR.Write(New Byte() {&H0, &H1})

        Dim READ_BUFFER As Byte() = New Byte(11) {}

        VL530L0X_SENSOR.WriteRead(New Byte() {&H14}, READ_BUFFER)

        Dim SENSOR_DATA As VL53L0XData = New VL53L0XData()
        With SENSOR_DATA
            .DISTANCE = BitConverter.ToInt16(READ_BUFFER, 10)
            .AMBIENT = BitConverter.ToInt16(READ_BUFFER, 6)
            .SIGNAL = BitConverter.ToInt16(READ_BUFFER, 8)
        End With

        Return SENSOR_DATA

    End Function

    Public Function READ_CUSTOM(COMMAND As Byte()) As Byte()
        VL530L0X_SENSOR.Write(New Byte() {&H0, &H1})
        Dim READ_BUFFER As Byte() = New Byte(24) {}

        VL530L0X_SENSOR.WriteRead(COMMAND, READ_BUFFER)

        Return READ_BUFFER

    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        VL530L0X_SENSOR.Dispose()

    End Sub

End Class
