using UnityEngine;
using System;
using System.IO.Ports;
using System.IO;
using System.Linq;

public class ProjectorPort {

	public const int PROJECTOR_BAUD_RATE = 115200; // the serial port is capable of transferring a maximum of 115200 bits per second
    public const int PROJECTOR_DATA_BITS = 8;
    public const string PROJECTOR_PARITY_CHECK = "None";
    public const string PROJECTOR_STOP_BIT = "One";


    public const int PROJECTOR_READ_TIMEOUT_MS = 250;
    public const int PROJECTOR_WRITE_TIMEOUT_MS = 250;
    public const string PROJECTOR_NEWLINE = "\n";
    public static bool isOSWindows = SystemInfo.operatingSystem.ToLower().Contains("windows");

    protected string portName;
    protected int baudrate = PROJECTOR_BAUD_RATE;
    protected SerialPort _port;

    // getter / setter
    public bool IsPortInitialized { get; protected set; }

    public bool IsPortSupported { get; protected set; }

    public bool IsWorking {
        get {
            return IsPortInitialized && IsPortSupported;
        }
    }

	// Constructor 
    public ProjectorPort(string portName, int baudrate, string parityCheck, int dataLength, string stopBit) {
        IsPortInitialized = false;
        IsPortSupported = false;
        this.portName = portName;
        this.baudrate = baudrate;

        _port =
			// Create new serial COM port : e.g.  ("\\.\COM10", 115200)
            new SerialPort((isOSWindows ? @"\\.\" : "") + portName, baudrate) {
                ReadTimeout = PROJECTOR_READ_TIMEOUT_MS,
                WriteTimeout = PROJECTOR_WRITE_TIMEOUT_MS,
                NewLine = PROJECTOR_NEWLINE,
                DataBits = dataLength,
                Parity = (Parity) Enum.Parse(typeof(Parity), parityCheck),
                StopBits = (StopBits) Enum.Parse(typeof(StopBits), stopBit)
            };

        Debug.Log(_port.StopBits);
        Debug.Log(_port.Parity);
    }

    public ProjectorPort(string portName, int baudrate) : this(portName, baudrate, PROJECTOR_PARITY_CHECK, PROJECTOR_DATA_BITS, PROJECTOR_STOP_BIT) {}

    public ProjectorPort(string portName) : this(portName, PROJECTOR_BAUD_RATE, PROJECTOR_PARITY_CHECK, PROJECTOR_DATA_BITS, PROJECTOR_STOP_BIT) {}


//	use protected if you only want a subclass to access the method.
    protected void WriteCommand (string _command, char[] separators = null) {
        string command = _command;
        if (separators != null) {
            command = new string(_command
                .Split(separators)
                .Select(s => (char)Convert.ToInt32(s, s.Contains("0x") ? 16 : 10))
                .ToArray());
        }

        _port.DiscardInBuffer ();
        string toWrite = FormatCommand(command);
        Debug.Log("write (" + toWrite.Length + ") : " + toWrite);
        _port.Write(toWrite);
    }
//	use protected virtual if you only want a subclass to access the method but also provide the ability for the subclass to extend or override it.
    protected virtual string FormatCommand(string cmd) {
		// helper for children (e.g. BenqProjectorPort) to format the command
        return cmd;
    }

    protected string ReadCommand () {
        if (isOSWindows) {
            string readStr = "";
            string charStr = "";
            bool isReading = true;
            while (isReading) {
                try {
                    char c = (char) _port.ReadChar();
                    charStr += (int) c + " ";
                    readStr += c;
                } catch (TimeoutException e) {
                    Debug.Log(e.Message);
                    Debug.Log("Received: " + charStr);
                    isReading = false;
                }
            }
            return readStr;
        } else {
            return _port.BytesToRead > 0 ? _port.ReadExisting() : "";
        }
    }

//	internal is for assembly scope (i.e. only accessible from code in the same .exe or .dll) namespace
    internal string SendCommand (string command, char[] separators = null) {
        WriteCommand (command, separators);
        return ReadCommand ();
    }

    public bool Open () {
        bool hasNoError = true;
        try {
			_port.Close();
            _port.Open ();
            IsPortInitialized = true;

            Debug.Log("Test the projector");
            IsPortSupported = TestIfPortSupported();
        } catch (IOException e) {
            IsPortInitialized = false;
//            Debug.LogError (e.Message);
//            Debug.LogError ("Err on port: " + portName);
            hasNoError = false;
        }
        return hasNoError;
    }

    public void Close () {
        if (_port.IsOpen) {
            _port.Close ();
        }
    }

    // TODO:
    // need a proper and general way to test whether a port is supported in general
    // Otherwise, the children could override this function to provide test function
    protected virtual bool TestIfPortSupported () {
        return true;
    }
}