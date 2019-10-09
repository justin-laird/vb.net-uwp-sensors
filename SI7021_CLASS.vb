Imports Windows.Devices.I2C

Public Structure SI7021_Data
    Public TEMPERATURE As Byte
    Public HUMIDITY As Byte
    Public TEMP_INT As Decimal
    Public HUM_INT As Decimal
End Structure

Public Class SI7021_CLASS
    Implements IDisposable

    Private SI7021_SENSOR As I2cDevice
    Private Const SI7021_ADDR As Byte = &H40

    Private Const MEASURE_RH_HOLD_MASTER As Byte = &HE5
    'Private Const MEASURE_RH_NO_HOLD_MASTER As Byte = &HF5
    Private Const MEASURE_TEMP_HOLD_MASTER As Byte = &HE3
    'Private Const MEASURE_TEMP_NO_HOLD_MASTER As Byte = &HF3
    Private Const READ_TEMP_FROM_PREV_RH As Byte = &HE0
    Private Const RESET As Byte = &HFE
    'Private Const WRITE_RH_T_USER_REG As Byte = &HE6
    Private Const READ_RH_T_USER_REG As Byte = &HE7
    'Private Const WRITE_HEATER_CTRL_REG As Byte = &H51
    'Private Const READ_HEATER_CTRL_REG As Byte = &H11
    'Private Const READ_ID_BYTE_1 As Byte = &HFA
    'Private Const READ_ID_BYTE_2 As Byte = &HFC
    'Private Const READ_FIRMWARE_REV As Byte = &H4

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
        Dim readBuf As Byte() = New Byte(1) {}
        Dim RAW_NUM As Integer

        SI7021_SENSOR.Write(New Byte() {RESET})
        Threading.Thread.Sleep(50)

        SI7021_SENSOR.WriteRead(New Byte() {MEASURE_RH_HOLD_MASTER}, readBuf)

        RAW_NUM = Convert.ToInt32(BitConverter.ToString(readBuf, 0).Replace("-", ""), 16)

        PROBE_DATA.HUMIDITY = readBuf(1)
        PROBE_DATA.HUM_INT = ((125 * RAW_NUM) / 65536) - 6

        SI7021_SENSOR.WriteRead(New Byte() {READ_TEMP_FROM_PREV_RH}, readBuf)

        RAW_NUM = Convert.ToInt32(BitConverter.ToString(readBuf, 0).Replace("-", ""), 16)

        PROBE_DATA.TEMPERATURE = readBuf(1)
        PROBE_DATA.TEMP_INT = ((175.72 * RAW_NUM) / 65536) - 46.85

        Return PROBE_DATA

    End Function

End Class
