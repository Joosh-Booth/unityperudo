package com.psyjabo.server;


import java.security.SecureRandom;
import java.util.Random;

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 *
 * @author jjosh
 */
class Dice{
   
  
    static int[] getDice(int diceLeft){
        Random dice = new Random();
        int diceToSend[]=new int[diceLeft];
        for(int i = 0; i<diceLeft;i++)
        {
            int d = 0;
            while(d<=0)
            {
                d = (dice.nextInt() % 7) ;
            }
            diceToSend[i] =d;
        }
    return diceToSend;
    }
    
}