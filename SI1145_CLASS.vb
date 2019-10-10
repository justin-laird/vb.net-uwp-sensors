Imports Windows.Devices.I2C

Public Structure SI1145_Data
    Public IR_LIGHT As Integer
    Public UV_LIGHT As Integer
    Public VISABLE_LIGHT As Integer
    Public PROXIMITY As Integer
End Structure

Public Class SI1145_CLASS
    Implements IDisposable

    Private SI1145_SENSOR As I2cDevice
    Private Const SI1145_ADDRESS As Byte = &H60 'The adress for the i2c device
    Private Const SI1145_REG_PARTID As Byte = &H0 'Get the chip product ID
    Private Const SI1145_CHIP_STATE As Byte = &H30 'Get the chip state
    Private Const SI1145_REG_ALSVISDATA0 As Byte = &H22 '
    Private Const SI1145_REG_UVINDEX0 As Byte = &H2C '
    Private Const SI1145_REG_ALSIRDATA0 As Byte = &H24
    Private Const SI1145_REG_PS1DATA0 As Byte = &H26

    Private Const SI1145_RESET As Byte = &H1 'Command to reset the chip
    Private Const SI1145_CHLIST As Byte = &H1 'Command to set the read type of the chip
    Private Const SI1145_CHLIST_SETTINGS As Byte = &HF7 'Set all chlist options to on
    Private Const SI1145_CHLIST_CHLIST_ENUV As Byte = &H80
    Private Const SI1145_CHLIST_ENALSIR As Byte = &H20
    Private Const SI1145_CHLIST_ENALSVIS As Byte = &H10
    Private Const SI1145_CHLIST_ENPS1 As Byte = &H1
    Private Const SI1145_REG_PSLED21 As Byte = &HF
    Private Const SI1145_PARAM_PS1ADCMUX As Byte = &H7
    Private Const SI1145_PARAM_ADCMUX_LARGEIR As Byte = &H3
    Private Const SI1145_PARAM_PSLED12SEL As Byte = &H2
    Private Const SI1145_PARAM_PSLED12SEL_PS1LED1 As Byte = &H1
    Private Const SI1145_PARAM_PSADCGAIN As Byte = &HB
    Private Const SI1145_PARAM_PSADCOUNTER As Byte = &HA
    Private Const SI1145_PARAM_ADCCOUNTER_511CLK As Byte = &H70
    Private Const SI1145_PARAM_PSADCMISC As Byte = &HC
    Private Const SI1145_PARAM_PSADCMISC_RANGE As Byte = &H20
    Private Const SI1145_PARAM_PSADCMISC_PSMODE As Byte = &H4
    Private Const SI1145_PARAM_ALSIRADCMUX As Byte = &HE
    Private Const SI1145_PARAM_ADCMUX_SMALLIR As Byte = &H0
    Private Const SI1145_PARAM_ALSIRADCGAIN As Byte = &H1E
    Private Const SI1145_PARAM_ALSIRADCOUNTER As Byte = &H1D
    Private Const SI1145_PARAM_ALSIRADCMISC As Byte = &H1F
    Private Const SI1145_PARAM_ALSIRADCMISC_RANGE As Byte = &H20
    Private Const SI1145_PARAM_ALSVISADCGAIN As Byte = &H11
    Private Const SI1145_PARAM_ALSVISADCOUNTER As Byte = &H10
    Private Const SI1145_PARAM_ALSVISADCMISC As Byte = &H12
    Private Const SI1145_PARAM_ALSVISADCMISC_VISRANGE As Byte = &H20
    Private Const SI1145_REG_INTCFG As Byte = &H3 'Set the INT Pin drive 0= never driven 1= driven low when... p32
    Private Const SI1145_REG_IRQEN As Byte = &H4 'Set the IRQ interrupts
    Private Const SI1145_REG_PARAMRD As Byte = &H2E 'Address to pass messages from the chip to the host

    Private Const SI1145_REG_MEASRATE0 As Byte = &H8
    Private Const SI1145_REG_MEASRATE1 As Byte = &H9
    Private Const SI1145_REG_IRQMODE1 As Byte = &H5
    Private Const SI1145_REG_IRQMODE2 As Byte = &H6

    Private Const SI1145_REG_IRQSTAT As Byte = &H21
    Private Const SI1145_REG_COMMAND As Byte = &H18
    Private Const SI1145_REG_PARAMWR As Byte = &H17
    Private Const SI1145_PARAM_SET As Byte = &HA0
    Private Const SI1145_REG_HWKEY As Byte = &H7

    Private Const SI1145_REG_UCOEFF0 As Byte = &H13
    Private Const SI1145_REG_UCOEFF1 As Byte = &H14
    Private Const SI1145_REG_UCOEFF2 As Byte = &H15
    Private Const SI1145_REG_UCOEFF3 As Byte = &H16

    Private Const SI1145_PSALS_FORCE As Byte = &H7 'Set the mode to Forced

    Private IS_INITIALIZED As Boolean = False

    Private Function WRITE_ASYNC(writeBuffer As Byte()) As Task(Of Boolean)
        Dim arg_1D_0 As Boolean = False
        If IS_INITIALIZED Then
            SI1145_SENSOR.Write(writeBuffer)
            Return Task.FromResult(arg_1D_0)
        End If
        Throw New Exception()
    End Function

    Public Function WRITE_READ_ASYNC(writeBuffer As Byte(), readBuffer As Byte()) As Task(Of Boolean)
        Dim arg_1E_0 As Boolean = False
        If IS_INITIALIZED Then
            SI1145_SENSOR.WriteRead(writeBuffer, readBuffer)
            Return Task.FromResult(arg_1E_0)
        End If
        Throw New Exception()
    End Function


    Private Async Function WRITE_PARAMETER(ByVal parameter As Byte, ByVal value As Byte) As Task(Of Byte())
        Await WRITE_ASYNC(New Byte() {SI1145_REG_PARAMWR, value})
        Await WRITE_ASYNC(New Byte() {SI1145_REG_COMMAND, parameter Or SI1145_PARAM_SET})

        Dim READ_BUFFER As Byte() = New Byte(0) {}
        Await WRITE_READ_ASYNC(New Byte() {SI1145_REG_PARAMRD}, READ_BUFFER)

        'If READ_BUFFER(0) <> value Then
        'Throw New Exception()
        'End If

        Return READ_BUFFER

    End Function


    Protected Async Function RESET_CHIP_ASYNC() As Task
        Await WRITE_ASYNC(New Byte() {SI1145_REG_MEASRATE0, 0})
        Await WRITE_ASYNC(New Byte() {SI1145_REG_MEASRATE1, 0})
        Await WRITE_ASYNC(New Byte() {SI1145_REG_IRQEN, 0})
        Await WRITE_ASYNC(New Byte() {SI1145_REG_IRQMODE1, 0})
        Await WRITE_ASYNC(New Byte() {SI1145_REG_IRQMODE2, 0})
        Await WRITE_ASYNC(New Byte() {SI1145_REG_INTCFG, 0})
        Await WRITE_ASYNC(New Byte() {SI1145_REG_IRQSTAT, &HFF})
        Await WRITE_ASYNC(New Byte() {SI1145_REG_COMMAND, SI1145_RESET})
        Await Task.Delay(15)
        Await WRITE_ASYNC(New Byte() {SI1145_REG_HWKEY, &H17})
        Await Task.Delay(15)

    End Function

    Protected Async Function RENEW_CHIP() As Task

        'write UVindex measurement coefficients! Not necessary  these are defaults
        Await WRITE_ASYNC(New Byte() {SI1145_REG_UCOEFF0, &H29})
        Await WRITE_ASYNC(New Byte() {SI1145_REG_UCOEFF1, &H89})
        Await WRITE_ASYNC(New Byte() {SI1145_REG_UCOEFF2, &H2})
        Await WRITE_ASYNC(New Byte() {SI1145_REG_UCOEFF3, &H0})

        Dim result As Byte()
        'Enable all the sensors
        result = Await WRITE_PARAMETER(SI1145_CHLIST, SI1145_CHLIST_CHLIST_ENUV Or SI1145_CHLIST_ENALSIR Or SI1145_CHLIST_ENALSVIS Or SI1145_CHLIST_ENPS1) 'SI1145_CHLIST_SETTINGS)

        'Set chip reading settings
        Await WRITE_ASYNC(New Byte() {SI1145_REG_INTCFG, &H1})
        Await WRITE_ASYNC(New Byte() {SI1145_REG_IRQEN, &H1})

        '----------
        'program LED current
        Await WRITE_ASYNC(New Byte() {SI1145_REG_PSLED21, &H3}) '20mA for LED 1 only
        Await WRITE_PARAMETER(SI1145_PARAM_PS1ADCMUX, SI1145_PARAM_ADCMUX_LARGEIR)
        'prox sensor #1 uses LED #1
        Await WRITE_PARAMETER(SI1145_PARAM_PSLED12SEL, SI1145_PARAM_PSLED12SEL_PS1LED1)
        'fastest clocks, clock div 1
        Await WRITE_PARAMETER(SI1145_PARAM_PSADCGAIN, 0)
        'take 511 clocks to measure
        Await WRITE_PARAMETER(SI1145_PARAM_PSADCOUNTER, SI1145_PARAM_ADCCOUNTER_511CLK)
        'in prox mode, high range
        Await WRITE_PARAMETER(SI1145_PARAM_PSADCMISC, SI1145_PARAM_PSADCMISC_RANGE Or SI1145_PARAM_PSADCMISC_PSMODE)

        Await WRITE_PARAMETER(SI1145_PARAM_ALSIRADCMUX, SI1145_PARAM_ADCMUX_SMALLIR)
        'fastest clocks, clock div 1
        Await WRITE_PARAMETER(SI1145_PARAM_ALSIRADCGAIN, 0)
        'take 511 clocks to measure
        Await WRITE_PARAMETER(SI1145_PARAM_ALSIRADCOUNTER, SI1145_PARAM_ADCCOUNTER_511CLK)
        'in high range mode
        Await WRITE_PARAMETER(SI1145_PARAM_ALSIRADCMISC, SI1145_PARAM_ALSIRADCMISC_RANGE)

        'fastest clocks, clock div 1
        Await WRITE_PARAMETER(SI1145_PARAM_ALSVISADCGAIN, 0)
        'take 511 clocks to measure
        Await WRITE_PARAMETER(SI1145_PARAM_ALSVISADCOUNTER, SI1145_PARAM_ADCCOUNTER_511CLK)
        'in high range mode (not normal signal)
        Await WRITE_PARAMETER(SI1145_PARAM_ALSVISADCMISC, SI1145_PARAM_ALSVISADCMISC_VISRANGE)

        '----------
        'measurement rate for auto
        Await WRITE_ASYNC(New Byte() {SI1145_REG_MEASRATE0, &HFF}) '255 * 31.25uS = 8ms   0x08
        'auto run
        Await WRITE_ASYNC(New Byte() {SI1145_REG_COMMAND, SI1145_REG_COMMAND})

    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        SI1145_SENSOR.Dispose()
    End Sub

    Public Async Function Initialize_SI1145_Async() As Task
        Dim settings As New I2cConnectionSettings(SI1145_ADDRESS) With {.BusSpeed = I2cBusSpeed.FastMode}

        Dim controller As I2cController = Await I2cController.GetDefaultAsync()
        SI1145_SENSOR = controller.GetDevice(settings)

        IS_INITIALIZED = True

        Await RESET_CHIP_ASYNC()
        Await RENEW_CHIP()

    End Function

    Public Function READ_PROBE_DATA() As SI1145_Data
        Dim PROBE_DATA As New SI1145_Data()
        Dim READ_BUFFER As Byte() = New Byte(1) {}

        'GET VISIBLE LIGHT INTEGER
        SI1145_SENSOR.WriteRead(New Byte() {SI1145_REG_ALSVISDATA0}, READ_BUFFER)
        PROBE_DATA.VISABLE_LIGHT = BitConverter.ToInt16(New Byte() {READ_BUFFER(1), READ_BUFFER(0)}, 0)

        'GET UV LIGHT INTEGER
        SI1145_SENSOR.WriteRead(New Byte() {SI1145_REG_UVINDEX0}, READ_BUFFER)
        PROBE_DATA.UV_LIGHT = BitConverter.ToInt16(New Byte() {READ_BUFFER(1), READ_BUFFER(0)}, 0)

        'GET IR LIGHT INTEGER
        SI1145_SENSOR.WriteRead(New Byte() {SI1145_REG_ALSIRDATA0}, READ_BUFFER)
        PROBE_DATA.IR_LIGHT = BitConverter.ToInt16(New Byte() {READ_BUFFER(1), READ_BUFFER(0)}, 0)

        'GET PROXIMITY DISTANCE INTEGER
        SI1145_SENSOR.WriteRead(New Byte() {SI1145_REG_PS1DATA0}, READ_BUFFER)
        PROBE_DATA.PROXIMITY = BitConverter.ToInt16(New Byte() {READ_BUFFER(1), READ_BUFFER(0)}, 0)

        Return PROBE_DATA

    End Function

End Class
