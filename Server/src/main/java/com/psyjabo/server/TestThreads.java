/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.psyjabo.server;

import java.io.BufferedReader;
import java.io.DataInputStream;
import java.io.InputStreamReader;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.HashMap;
import java.util.Map;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

/**
 *
 * @author jjosh
 */
public class TestThreads {
    public static void main(String arg[]){
               
        Thread thread = new Thread(new Runnable(){
            @Override
            public void run(){
                try{
                    ServerSocket server = new ServerSocket(3000);
                    while(true){
                        Socket s = server.accept();
                        DataInputStream dataInput = new DataInputStream(s.getInputStream());
                        String userName = dataInput.readUTF();
                        System.out.println(userName);
                        System.out.println("Server Accepting Thread comleted loop");
                    }
        }catch(Exception e){}
        
    
            }});
        thread.start();
        
    }  
}
