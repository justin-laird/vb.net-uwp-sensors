Imports Windows.Devices.I2C

Public Structure SI7021_Data
    Public TEMPERATURE As Decimal
    Public HUMIDITY As Decimal
    Public TIMESTAMP As Date
End Structure

Public Class SI7021_CLASS
    Implements IDisposable

    Private SI7021_SENSOR As I2cDevice
    Private Const SI7021_ADDR As Byte = &H40

    Private Const MEASURE_RH_HOLD_MASTER As Byte = &HE5
    Private Const MEASURE_TEMP_HOLD_MASTER As Byte = &HE3
    Private Const READ_TEMP_FROM_PREV_RH As Byte = &HE0
    Private Const RESET As Byte = &HFE
    Private Const READ_RH_T_USER_REG As Byte = &HE7

    Public Async Function Initialize_SI7021_Async() As Task
        Dim settings As New I2cConnectionSettings(SI7021_ADDR)
        With settings
            .BusSpeed = I2cBusSpeed.FastMode
        End With

        Dim controller = Await I2cController.GetDefaultAsync()
        SI7021_SENSOR = controller.GetDevice(settings)
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        SI7021_SENSOR.Dispose()
    End Sub

    Public Function Read() As SI7021_Data
        Dim PROBE_DATA As New SI7021_Data()
        Dim READ_BUFFER As Byte() = New Byte(1) {}
        Dim RAW_NUM As Integer

        SI7021_SENSOR.Write(New Byte() {RESET})
        Threading.Thread.Sleep(50)

        SI7021_SENSOR.WriteRead(New Byte() {MEASURE_RH_HOLD_MASTER}, READ_BUFFER)
        RAW_NUM = BitConverter.ToInt16(New Byte() {READ_BUFFER(1), READ_BUFFER(0)}, 0)
        PROBE_DATA.HUMIDITY = ((125 * RAW_NUM) / 65536) - 6

        SI7021_SENSOR.WriteRead(New Byte() {READ_TEMP_FROM_PREV_RH}, READ_BUFFER)
        RAW_NUM = BitConverter.ToInt16(New Byte() {READ_BUFFER(1), READ_BUFFER(0)}, 0)
        PROBE_DATA.TEMPERATURE = ((175.72 * RAW_NUM) / 65536) - 46.85

        PROBE_DATA.TIMESTAMP = Date.Now

        Return PROBE_DATA

    End Function

End Class
