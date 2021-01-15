using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Text;

namespace Client {
    
    public class ClientObject
    {
        public Socket clientSocket;
        bool connected;
        //Connect player to server
        public void Connect()
        {
            //Server address
            //IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse("192.168.56.1"), 3000);
            IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse("3.10.190.9"), 3000);

            //Create and connect socket to server
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(serverAddress);

            //Thread constantly running to receive messages
            connected = true;
            Thread thread = new Thread(new ThreadStart(ReceiveMessage));
            thread.Start();
        }

        public void ReceiveMessage() 
        {
           
                //Constantly Run
                while (connected)
                {
                     
                    //Receive length of message in bytes from first message
                    byte[] rcvLenBytes = new byte[4];
                    clientSocket.Receive(rcvLenBytes);

                    //Convert to int to get length as 4-byte integer
                    int rcvLen = System.BitConverter.ToInt32(rcvLenBytes, 0);

                    //Receive actual message
                    byte[] rcvBytes = new byte[rcvLen];
                    clientSocket.Receive(rcvBytes);
                    
                    //Convert to string
                    String rMessage = System.Text.Encoding.ASCII.GetString(rcvBytes);

            
                   
                    
                    if (rMessage.Length == 0) 
                    {
                    connected = false;
                    }

                    if (MessageRecevied != null)
                    {
                        MessageRecevied(rMessage);
                    }
                }
            
        }

        public void SendMessage(String message) 
        {
            //Get the length of the ASCII message in bytes 
            int toSendLen = System.Text.Encoding.ASCII.GetByteCount(message);

            //Encode message 
            byte[] toSendBytes = System.Text.Encoding.ASCII.GetBytes(message);

            //Convert value of the length in bytes into bytes
            byte[] toSendLenBytes = System.BitConverter.GetBytes(toSendLen);

            //Send the length of the message in bytes, then the message in bytes to the server
            Debug.Log("Sending message: " + message + " Length: " + toSendLen + ", toSendLenBytes: " + toSendLenBytes.Length + ", toSendBytes: " + toSendBytes.Length);
            clientSocket.Send(toSendLenBytes);
            clientSocket.Send(toSendBytes);
        }

        //Delegate function allows it to be set to a method with the same parameters
        public delegate void MessageHandler(String e);
        public event MessageHandler MessageRecevied;
    }

}
    


