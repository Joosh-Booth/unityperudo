/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.psyjabo.server;

import java.io.BufferedReader;

import java.io.InputStreamReader;
import java.net.ServerSocket;
import java.net.Socket;

import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

/**
 *
 * @author jjosh
 */
public class Main {
    public static void main(String arg[])throws Exception{
               
        try( ServerSocket server = new ServerSocket(3000)){
            ExecutorService pool = Executors.newFixedThreadPool(6);
            GameThread game = new GameThread();
            int i = 0;
            while(true)
            {

                Socket s = server.accept();
                if(game.startCheck())
                {
                    System.out.println("New Game");
                    game = new GameThread();
                    i = 0;
                }
                pool.execute(game.players[i]=game.new Player(s,i));
                        i++;
                        game.playerCount=i;
               
            }
        
        }
       
        
    }

}

