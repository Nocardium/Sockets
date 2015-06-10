Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports Microsoft.VisualBasic
Imports System.Threading

Module Module1
    Public Data As String = Nothing

    Sub Main()
        'A byte array to store both recieved and sent messages. This syntax ensures that any
        'new byte array declared as bytes will be of size 1024, using As New Byte(1024) {}
        Dim bytes() As Byte = New [Byte](1024) {}

        'Set up a new endpoint to act as the address and port for the socket by looking up ip on DNS
        Dim LocalHost As IPAddress = Dns.GetHostEntry(Dns.GetHostName).AddressList(3)
        Dim NewEndPoint As New IPEndPoint(LocalHost, 4567)

        Console.WriteLine(LocalHost)

        'Create the new socket
        Dim NewSocket As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

        NewSocket.Bind(NewEndPoint)


        'Tell socket to listen for up to 10 queued messages
        NewSocket.Listen(10)

        'Set up an infinte loop for communication
        While True
            Console.WriteLine("Awaiting connection from client...")


            'If there is no incomming communication, the application will pause here
            'Socket.accept returns a new socket, so a new socket has to created to accept the incomming connection
            Dim Handler As Socket = NewSocket.Accept()

            Console.WriteLine("Connection established")

            'Just make sure data is empty
            Data = Nothing



            'Set another infinite loop for recieving messages
            While True
                'Set up a new byte array for messages
                bytes = New Byte(1024) {}

                'AcceptedSocket.receive(ByteArray) assigns the incomming buffer to the byte array, but returns an integer
                Dim DataRecieved As Integer = Handler.Receive(bytes)

                Console.WriteLine("Data recieved")

                'Make a string equal to itself + the character values of bytes, starting a the first character and
                'for the number of characters in DataReceived
                Data += Encoding.ASCII.GetChars(bytes, 0, DataRecieved)

                'if the string contains EOF, exit the receiving loop
                If InStr(Data, "<EOF>") > 0 Then
                    Exit While
                End If

                'Print the text in Data to the screen
                Console.WriteLine("Text: {0}", Data)

                'Encode the message as bytes again
                Dim Message As Byte() = Encoding.ASCII.GetBytes(Data)

                'Send the message
                Handler.Send(Message)

                Data = ""

            End While

            'Finish the socket
            Handler.Shutdown(SocketShutdown.Both)
            Handler.Close()

            Console.WriteLine("Connection closed.")
            Console.ReadLine()
        End While

    End Sub

End Module
