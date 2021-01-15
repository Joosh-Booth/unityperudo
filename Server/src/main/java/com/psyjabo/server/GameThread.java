/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.psyjabo.server;


import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import static java.lang.Thread.sleep;
import java.net.Socket;
import java.util.Arrays;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author jjosh
 */
public class GameThread {
    volatile boolean started=false;
    boolean finished = false;
    volatile int playerCount=0;
    volatile Player[] players={null,null,null,null,null,null};
    volatile int[] guess={0,0,0};
    volatile int currentTurn = 0; 
    int round = 1;
    volatile int[] diceArray={0,0,0,0,0,0};
    volatile boolean doubting=false;
    ScheduledExecutorService executor = Executors.newScheduledThreadPool(1);
    
    public GameThread()
    {
      
        
        executor.scheduleAtFixedRate(pingPlayers, 0, 3, TimeUnit.SECONDS);
    }
    
    public synchronized void endTurn() throws IOException{
        currentTurn++;
        if(players[currentTurn]==null){
                currentTurn=0;   
            }
        while(players[currentTurn].diceLeft<1){
            currentTurn++;
            if(players[currentTurn]==null){
                currentTurn=0;
                
            }
        }
        
    }
    
    public void sendToAll(String message) throws IOException{
    switch(message){
        case "GUESS":
            for(Player player:players){
            if(player!=null){
                player.sendString("GUESS"+guess[0]+" "+
                    guess[1]+"\r\n");
            }
        }
            break;
        case "CURRENTTURN":
            for(Player player:players){
                if(player!=null){

                        player.sendString("CURRENTTURN"+players[currentTurn].id+"\r\n");                   
                }
            }
            break;
        }
    }
    
    public void sendToAll (String message,Player player) throws IOException{
        switch(message){
            case("PLAYERS"):
                String names = "PLAYERS"+player.name+" ";
                if(player.ready)
                    {
                        names=names+1+player.cupColour+player.diceColour;
                    }
                    else
                    {
                        names=names+0+player.cupColour+player.diceColour;
                    }
                for(Player array:players){
                    if(array!=null&&array.id!=player.id){
                        array.sendString(names);
                    }
                }
                System.out.println(names);
                break;
            case ("DOUBT"):          
              for(Player array1:players)
              {
                  if(array1!=null)
                  {
                    for(Player array2:players)
                    {
                      if(array2!=null)
                      {
                        if(array2.id!=array1.id)
                        {
                            String s="";
                            for(int i = 0; i<array2.dice.length;i++)
                            {
                                s = s+array2.dice[i]+" ";
                            }
                            array1.sendString("DICE"+array2.id+s);
                        }
                      }   
                    }
                  }
              }
                break;
            case ("WRONG"):
                for(Player array:players)
                {
                    if(array!=null)
                    {
                        array.sendString("WRONG"+player.id+" "+guess[0]+" "+guess[1]);
                    }
                }
                break;
            case ("CORRECT"):
                 
                for(Player array:players)
                {
                    if(array!=null)
                    {
                        array.sendString("CORRECT"+guess[2]+" "+guess[0]+" "+guess[1]);
                    }
                }
                break;
                
            case "CHECKAMOUNT":
            
            int id=player.id;
            System.out.println("player.id before loop = "+id);
            String string = "CHECKAMOUNT";
            int id2=-1;
            while(id!=id2){
                System.out.println("id2 in loop = "+id2);
                for(Player array:players){
                    if(array!=null){
                        int i = 0;
                        for(int j=0; j<player.diceLeft;j++){
                            
                            if(player.dice[j]==guess[1]){
                                i++;
                            }
                        }
                        if(i==1){
                            array.sendString("CHECKAMOUNT"+player.id+" has "+i+" "+guess[1]+".\r\n");
                        }else{
                            array.sendString("CHECKAMOUNT"+player.id+" has "+i+" "+guess[1]+"'s.\r\n");
                        }
                    }
                }
                id2=player.id;
                id2++;
                if(players[id2]==null){
                    System.out.println("player.id++ !=null, player id = "+player.id);
                    id2=0;
                    player=players[id2];
                }else{
                    System.out.println("player.id else statement= "+player.id);
                    player=players[id2];  
                }
                
                
            }
                break;
                
            //SEND THAT A PLAYER HAS CLICKED READY TO ALL OTHER PLAYERS, SEND 
            //PLAYER ID AND 1 INDICATING READY
            case ("READY"):
                for(Player array: players){
                    if(array!=null){
                        array.sendString("READY"+player.id+1);
                    }
                }
                break;
            //SEND THAT A PLAYER HAS CLICKED UNREADY TO ALL OTHER PLAYERS, SEND 
            //PLAYER ID AND 0 INDICATING NOT READY    
            case ("UNREADY"):
                for(Player array: players){
                    if(array!=null){
                        array.sendString("READY"+player.id+0);
                    }
                }
                break;
            
            case("DISCONNECTED"):
                for(Player array: players)
                {
                    if(array!=null && array!= player)
                    {
                        array.sendString("DISCONNECTED"+player.id);
                    }
                }
                break;
        }
    }
    
    public synchronized boolean readyCheck(){
        int i=0;
        for(Player player:players){
            if(player!=null&&player.ready){
                i++;
            }

        }
        if(i==playerCount&&i>1){
            System.out.println("Started = true");
            started = true;
            return true;
        }   
        return false;
    }
    
    public synchronized boolean startCheck(){
        return started;
    }
    
    public void winCheck() throws IOException
    {
        int id=-1;
        int count=0;
        for(Player player:players)
        {
            if(player!=null)
            {
                if(player.diceLeft>0)
                {
                 count++;
                 id = player.id;
                }
            }
        }
        if(count==1)
        {
            players[id].sendString("WINNER");
        }    
    }
    
    public int turnCheck(){
            return players[currentTurn].id;                   
    }
    
    public boolean shakeCheck(){
        for(Player player:players){
            if(player!=null&&player.shake<round){
                return false;
            }
        }
        return true;
    }
    
    public synchronized void shakeDice(Player player) throws IOException{
        System.out.println("curerntPlayer Shaking = "+player.id);
        
        if(player.diceLeft<1){
            throw new IllegalStateException("No dice left");
        }else if(player.shake==round){
            throw new IllegalStateException("Wait for new round");
        }else if(doubting==true){
            throw new IllegalStateException("Player is Doubting.");
        }
        String diceToSend="";
        player.dice = Dice.getDice(player.diceLeft);
        for(int i = 0; i < player.diceLeft; i++){
           // player.dice[i] = new Dice();
            //player.dice[i].getDice();
            diceArray[(player.dice[i])-1]++;
            diceToSend=diceToSend+player.dice[i]+" ";
        }
        System.out.println("Dice to send: "+diceToSend);
        for(int i :diceArray){
                System.out.print(","+i);
        }
                  System.out.print("\r\n");
        
        player.sendString("DICE"+player.id+diceToSend);
        player.shake++;
    }
    
    private void removeDice(Player player) throws IOException{
        player.diceLeft--;
        if(player.diceLeft==0){
            player.sendString("OUTYou are out.");
        }
    }
    
    public void setGuess(int i,int j, int id){
                guess[0]=i;
                guess[1]=j;
                guess[2]=id;
            }
     
    public synchronized void guess (int i, int j, Player player){
        System.out.println("curerntPlayer id = "+players[currentTurn].id + " is guessing.");
        if(player.diceLeft<1){
            throw new IllegalStateException("No dice left");
        }else if (player.id!=players[currentTurn].id){
            throw new IllegalStateException("Not your turn.");
        }else if(doubting==true){
            throw new IllegalStateException("Player is Doubting.");
        
        }else if(!shakeCheck()){
            throw new IllegalStateException("Not all players have shaken their dice yet.");
        }else if(!(i>guess[0]&&j>=guess[1]||i>=guess[0]&&j>guess[1])){
            throw new IllegalStateException("Invalid guess.");
        }
        setGuess(i,j,player.id);
    }
    
    public synchronized void doubt(Player player) throws IOException{
        System.out.println("curerntPlayer id = "+players[currentTurn].id +". Player id: "+ player.id+ " is doubting.");
        if(player.id == guess[2]){
            throw new IllegalStateException("Can't Doubt your own guess");
        }
        if(doubting==true){
            throw new IllegalStateException("Somebody is already called doubt.");
        }
        if(guess[0]==0||guess[1]==0){
            throw new IllegalStateException("A guess must be made.");
        }
        doubting=true;
        sendToAll("DOUBT",player);
        sendToAll("CHECKAMOUNT",player);

        if(guess[0]>diceArray[guess[1]-1]){
        //correct doubt
        //COUNT dice and output
            
            sendToAll("CORRECT",player);
            currentTurn =guess[2];
            removeDice(players[guess[2]]);

        }else{
        //wrong doubt
          sendToAll("WRONG",player);
            currentTurn=player.id;
            removeDice(player);

        }
        round++;
        guess= new int[3];
        Arrays.fill(diceArray, 0);
        doubting =false;
        winCheck();
    }
    
    
    
    
    Runnable pingPlayers = new Runnable(){
        public void run()
        {          
            int i = 0;
            for(Player player:players)
            {
                if(player!=null&&player.isConnected)
                {
                    try
                    {
                        i++;
                        player.sendString("PING");
                    }
                    catch(IOException ioException)
                    {
                        try {
                            sendToAll("DISCONNECTED", player);
                            player.isConnected = false;
                        } catch (IOException ex) {
                            Logger.getLogger(GameThread.class.getName()).log(Level.SEVERE, null, ex);
                        }
                        i--;
                    }
                }
            }
            if(i<=1&&playerCount>1)
            {
            finished = true;
            executor.shutdown();
            }
        }
    };
    
    public class Player implements Runnable{
        String name;
        int shake = 0;
        Socket socket;
        int id;
        int cupColour;
        int diceColour;
        boolean ready = false;
        int diceLeft = 5;
        int [] dice;
        boolean isConnected;
        InputStream input;
        OutputStream output;
        
        public Player(Socket socket,int i){
            this.socket = socket;
            this.id=i;
            this.isConnected = true;
            System.out.println("Name: "+name+" id:"+ i);
        }

        @Override
        public void run() { 
            try {
                setUp();
                readyMessageReceiver();
                waitForReady();
                sendString("STARTED");            
                System.out.println("started");
                
                collectTurn();
                
            } catch (IOException ex) {
                Logger.getLogger(GameThread.class.getName()).log(Level.SEVERE, null, ex);
            } 
            
        }
        
        public void processGuess(int i, int j) throws IOException {
            try{
            guess(i,j, this);
            sendToAll("GUESS");
            endTurn();
            sendToAll("CURRENTTURN");

            }catch(IllegalStateException e){
                System.out.println(e.getMessage());
                
                sendString("EXC"+e.getMessage());
            }
        }
        public void processDoubt() throws IOException{
            try{
                doubt(this);
                
                sendToAll("CURRENTTURN");

            }catch(IllegalStateException e){
                System.out.println(e.getMessage());
               
                sendString("EXC"+e.getMessage());
            }
        }
//        boolean playerConnected()
//        {
//        
//        }
       
        public void collectTurn() throws IOException{
            
            sendString("CURRENTTURN"+players[currentTurn].id);
            
            while(!finished&&isConnected){

                String type = receiveString();
                System.out.println("type: " + type);

                if(type.startsWith("GUESS")){
                  //retrieve the guess from the string
                  type = type.substring(5);
                  String[] split = type.split(" ",2);
                  int guess1 = Integer.parseInt(split[0]);
                  int guess2 = Integer.parseInt(split[1]);

                  System.out.println("guess is: " + guess1+","+guess2);

                  processGuess(guess1, guess2);



                }else if(type.startsWith("DOUBT")){
                    processDoubt();
                }else if(type.startsWith("SHAKE")){
                    try{
                    shakeDice(this);
                    }catch(IllegalStateException e){
                        System.out.println(e.getMessage());
                        sendString("EXC"+e.getMessage()+"\r\n");
                    }
                    
                }
            }
            System.out.println("Discconected: "+id);
        }
        
        
        
        private void waitForReady(){
            while(!startCheck()){
                
                try {
                    readyCheck();
                    sleep(200);
                } catch (InterruptedException ex) {
                    Logger.getLogger(GameThread.class.getName()).log(Level.SEVERE, null, ex);
                }

                               
            }
        }
        
        private void setUp() throws IOException{

            input = socket.getInputStream();
            
            output = socket.getOutputStream();
            
            collectInfo(receiveString());
            System.out.println("Before send to all");
            sendAllPlayers();
            sendToAll("PLAYERS", this);
            sendString("ID"+this.id);

            System.out.println("After send to all");
        }
        void collectInfo(String s)
        {
            String[] split = s.split(" ",3);
            name = split[0];
            cupColour = Integer.parseInt(split[1]);
            diceColour = Integer.parseInt(split[2]);

        }
        void sendAllPlayers() throws IOException
        {
            
            String names = "PLAYERS";
            String ready = "";
            for(Player player:players){
                if(player!=null){
                    names=names+(player.name+" ");
                    if(player.ready)
                    {
                        ready=ready+1+player.cupColour+player.diceColour+"-";
                    }
                    else
                    {
                        ready=ready+0+player.cupColour+player.diceColour+"-";
                    }
                }
            }
            System.out.println(names);
            
            this.sendString(names+ready);

        }
        
        String receiveString() throws IOException{
            
            byte[] lenBytes = new byte[4];
            if(input.read(lenBytes, 0, 4)==-1){
            
               // System.out.println("LENBYTE IS -1");
            }
            int len = (((lenBytes[3] & 0xff) << 24) |
                      ((lenBytes[2] & 0xff) << 16) |
                      ((lenBytes[1] & 0xff) << 8) |
                      (lenBytes[0] & 0xff));
            
            byte[] receivedBytes = new byte[len];
            input.read(receivedBytes, 0, len);
            String string = new String(receivedBytes, 0, len);
           if(string.length()>120)
           {
           throw new IllegalStateException("SomethingwentwRong try again");
           }
            return string;
        }
        
        void sendString(String messageToSend) throws IOException{
            
            String toSend = messageToSend;
            byte[] toSendBytes = toSend.getBytes();
            int toSendLen = toSendBytes.length;
            byte[] toSendLenBytes = new byte[4];
            toSendLenBytes[0] = (byte)(toSendLen & 0xff);
            toSendLenBytes[1] = (byte)((toSendLen >> 8) & 0xff);
            toSendLenBytes[2] = (byte)((toSendLen >> 16) & 0xff);
            toSendLenBytes[3] = (byte)((toSendLen >> 24) & 0xff);
            output.write(toSendLenBytes);
            output.write(toSendBytes);
        }
        
        
        void readyMessageReceiver() throws IOException{
            Player p = this;
            Thread t = new Thread(new Runnable() {
                @Override
                public void run() {
                     while(!startCheck()){
                        
                        String in="";
                         try {
                             in = receiveString();
                         } catch (IOException ex) {
                             Logger.getLogger(GameThread.class.getName()).log(Level.SEVERE, null, ex);
                         }

                        if(in.startsWith("READY")){
                            try {
                                ready=true;
                                //output.writeBytes("READYYou are ready, Waiting on others\r\n");
                                sendToAll("READY",p);
                               // readyCheck();
                            } catch (IOException ex) {
                                Logger.getLogger(GameThread.class.getName()).log(Level.SEVERE, null, ex);
                            }
                        }
                        if(in.startsWith("UNREADY")){
                            try {
                                if(!readyCheck()){
                                    ready=false;
                                }
                                sendToAll("UNREADY",p);

                               // readyCheck();
                            } catch (IOException ex) {
                                Logger.getLogger(GameThread.class.getName()).log(Level.SEVERE, null, ex);
                            }
                        }
                    }
                }
            });
           t.start();
        }
    }
}
