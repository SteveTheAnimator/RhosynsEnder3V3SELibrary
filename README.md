# Rhosyn's Ender3 V3 SE Library
A simple little library using System.IO.Ports to let you send gcode commands to your ender3 v3 se and lets you print models.
This library only works on windows for now and requires the .NET 9.0 SDK (I think).

## Installation
Just download the DLL and add it to your project references.

## Usage

Sending a list of commands using Print
```csharp
using RhosynsEnder3V3SELibrary;

public class Program
{
	public static void Main(string[] args)
	{
		// change COM7 to your printer's port, you can find it using SerialPort.GetPortNames()
		SerialPort port = new SerialPort("COM7", 115200);
        Ender3V3SE printer = new Ender3V3SE(new COMSystem(ref port));
        printer.Initialize();

        // send a G28 command to home the printer
        GCode gcode = new GCode(new List<GCodeCommand>
        {
            new GCodeCommand("G28", 0), // command, line number
        });

        // then, send it to print.
        string printResponse = await printer.Print(gcode, s => Console.WriteLine(s));
        Console.WriteLine($"print job response:\n{printResponse}");

        // finally, close the port
        port.Close();
	}
}
```
Sending a command using SendCommand()
```csharp
using RhosynsEnder3V3SELibrary;

public class Program
{
    public static void Main(string[] args)
    {
        // change COM7 to your printer's port, you can find it using SerialPort.GetPortNames()
        SerialPort port = new SerialPort("COM7", 115200);
        Ender3V3SE printer = new Ender3V3SE(new COMSystem(ref port));
        printer.Initialize();

        // send a G28 command to home the printer
        string response = await printer.SendCommand("G28");
        Console.WriteLine($"response:\n{response}");

        // finally, close the port
        port.Close();
    }
}
```
Printing a .gcode using PrintFromFile()
```csharp
using RhosynsEnder3V3SELibrary;

public class Program
{
    public static void Main(string[] args)
    {
        // change COM7 to your printer's port, you can find it using SerialPort.GetPortNames()
        SerialPort port = new SerialPort("COM7", 115200);
        Ender3V3SE printer = new Ender3V3SE(new COMSystem(ref port));
        printer.Initialize();

        // send a .gcode file to print
        string response = await printer.PrintFromFile("path/to/your/file.gcode", s => Console.WriteLine(s));
        Console.WriteLine($"print job response:\n{response}");

        // finally, close the port
        port.Close();
    }
}
```

## Potential Usages
Now, this library can't really be used much other than just for a gimmick because it only works on windows but you could for example make a little program where you can draw things and get them to print. I don't really know though