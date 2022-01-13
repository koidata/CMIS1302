/** 
 * Name: Shubhang Arashanapalli 
 * Panther ID: 002420196
 * Some Notes: This code has the functionality to allow player controlled turns...
 * i.e, you can "press any key to continue", however I commented it out and implemented automation of turns instead
 * to make the gameplay video go faster. As more turns occur, the automation gets faster.
 * 
 * Also, apologies for the late submission! 
 * **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace PlayCards
{
    // extensions

    public enum SUIT // create an enumeration of all the suits
    {
        CLUBS,
        DIAMONDS,
        SPADES,
        HEARTS
    }

    public static class Extensions 
    {
        public static void Enqueue (this Queue<Card> cards, Queue<Card> newCards )
        {
            foreach (var card in newCards)
            {
                cards.Enqueue(card); //populate the Queue of Card with cards
            }
        }
    }

    // objects
    public class Card // defines the Card properties
    {
        public string DisplayCard { get; set; }
        public SUIT Suit { get; set; }
        public int Value { get; set; }


    }
    public class Player
    {
        public string Name { get; set; }
        public Queue<Card> Deck { get; set; }

        public Player() { } 

        public Player(string name)
        {
            Name = name;
        }

        public Queue<Card> Deal(Queue<Card> cards)       //create a queue of cards 
        {
            Queue<Card> player1cards = new Queue<Card>(); 
            Queue<Card> player2cards = new Queue<Card>();

            int counter = 2;
            while (cards.Any()) //while the deck has cards 
            {
                if (counter % 2 == 0) //the dealer usually deals the first card to the other player
                {
                    player2cards.Enqueue(cards.Dequeue());
                }
                else
                {
                    player1cards.Enqueue(cards.Dequeue());
                }
                counter++;
            }

            Deck = player1cards;
            return player2cards;
        }
    }



    public static class DeckOfCards
    {
        public static Queue<Card> GenerateCards() // generates 13 cards for each of the 4 suits
        {
            Queue<Card> cards = new Queue<Card>(); //instantiate a queue of cards

            for (int i = 2; i <= 14; i++) //starting at i=2 since Ace is valued higher than king
            {
                foreach (SUIT suit in Enum.GetValues(typeof(SUIT)))
                {
                    cards.Enqueue(new Card()
                    {
                        Suit = suit,
                        Value = i,
                        DisplayCard = GetShortName(i, suit)
                    });
                }
            }

            return Shuffle(cards);
        }

        /** After looking at some tips online, I discovered the "Fisher-Yates" Algorithm for shuffling cards **/

        private static Queue<Card> Shuffle(Queue<Card> cards)
        {
            List<Card> swappedCards = cards.ToList(); // creates a list containing the 52 cards
            Random r = new Random(DateTime.Now.Millisecond);
            for (int n = swappedCards.Count - 1; n > 0; n--) // starting at position 51 of the list of 52 cards
            {
                int k = r.Next(n + 1); // returns a random number between 1 and 52

                Card temp = swappedCards[n]; // temp = the card at 'n' position of the deck
                swappedCards[n] = swappedCards[k]; //swap the selected card with the random 'k' position of the deck
                swappedCards[k] = temp; // set the random 'k' position as the new starting position of the next cycle

            }

            Queue<Card> shuffledCards = new Queue<Card>();
            foreach (var card in swappedCards)
            {
                shuffledCards.Enqueue(card);
            }
            return shuffledCards;
        }

        private static string GetShortName(int value, SUIT suit)
        {
            string valueDisplay = "";
            if (value >= 2 && value <= 10)
            {
                valueDisplay = value.ToString();
            }

            else if (value == 11)
            {
                valueDisplay = "J";

            }
            else if (value == 12)
            {
                valueDisplay = "Q";

            }
            else if (value == 13)
            {
                valueDisplay = "K";

            }
            else if (value == 14)
            {
                valueDisplay = "A";

            }

            return valueDisplay;
        }


    }


    public class DrawCard // draw the card to be displayed
    {
        public static void DrawOutline(int xcord, int ycord)
        {

            int x = xcord * 11;
            int y = ycord;

            Console.SetCursorPosition(x, y);
            Console.WriteLine("┌─────────┐\n");

            for (int i = 0; i < 8; i++)
            {
                Console.SetCursorPosition(x, y + 1 + i);

                if (i != 7)
                {
                    Console.WriteLine("|         |");
                }

                else
                {
                    Console.WriteLine("└─────────┘");
                }
            }


        }

        public static void DrawSuitValue(SUIT suit, int xcord, int ycord)
        {

            String cardSuit = " ";
            int x = xcord * 11;
            int y = ycord;

            //Encode the suits with proper unicode symbols 

            foreach (SUIT s in Enum.GetValues(typeof(SUIT)))
            {
                if (suit == SUIT.HEARTS)
                {
                    cardSuit = "\u2665";
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                if (suit == SUIT.DIAMONDS)
                {
                    cardSuit = "\u2666";
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                if (suit == SUIT.CLUBS)
                {
                    cardSuit = "\u2663";
                    Console.ForegroundColor = ConsoleColor.White;
                }

                if (suit == SUIT.SPADES)
                {
                    cardSuit = "\u2660";
                    Console.ForegroundColor = ConsoleColor.White;
                }


            }

            Console.SetCursorPosition(x + 5, y + 4);
            Console.Write(cardSuit);
            Console.ForegroundColor = ConsoleColor.White;
        } 

        
    }
    

    public class Game : DrawCard // the main game engine that will define the rules and turn mechanic 
    {
        private Player Player1;
        private Player Player2;
        public int TurnCount;



        public Game(string player1name, string player2name)
        {
            Player1 = new Player(player1name);
            Player2 = new Player(player2name);

            var cards = DeckOfCards.GenerateCards(); //Returns a shuffled set of cards

            var deck = Player1.Deal(cards); //Returns Player2's deck.  Player1 keeps his.
            Player2.Deck = deck;

        }

        public bool IsEndOfGame()
        {
            
            if (!Player1.Deck.Any())
            {
                Console.WriteLine(Player1.Name + " is out of cards!  " + Player2.Name + " WINS!");
                Console.WriteLine("TURNS: " + TurnCount.ToString());
                return true;
            }
            else if (!Player2.Deck.Any())
            {
                Console.WriteLine(Player2.Name + " is out of cards!  " + Player1.Name + " WINS!");
                Console.WriteLine("TURNS: " + TurnCount.ToString());
                return true;
            }
            else if (TurnCount > 500)
            {
                Console.WriteLine("Infinite turns, lets call it a draw.");
                return true;
            }
            return false;
        }

        
        public void PlayTurn() //controls the process of each turn
        {
            Console.Clear();
            Queue<Card> pool = new Queue<Card>();

            var player1card = Player1.Deck.Dequeue();
            var player2card = Player2.Deck.Dequeue();

            // will be used to count the number of cards each player has after each turn 
            int deck1 = 0;
            int deck2 = 0;


            

            

            pool.Enqueue(player1card);
            pool.Enqueue(player2card);

            // set the x and y coordinates to begin drawing the playing cards:

            int x = 0, y = 2;

            // create a header for each card:

                Console.WriteLine(Player1.Name + " plays: " + "       " + Player2.Name + " plays: ");
                

            // draw the player's card

                // Draw Suit

                DrawCard.DrawOutline(x, y);
                DrawCard.DrawSuitValue(player1card.Suit, x, y);

            // Draw in the card value

                Console.SetCursorPosition(x + 2, y + 1);
                Console.Write(player1card.DisplayCard);

                Console.SetCursorPosition(x + 8, y + 7);
                Console.Write(player1card.DisplayCard);

            // draw the computer's hand

            x += 2; //shift the x coordinate to the right so the computer's hand does not overlap the player hand

                // Draw Suit

                DrawCard.DrawOutline(x, y);
                DrawCard.DrawSuitValue(player2card.Suit, x, y);

            // Draw in the card value

                Console.SetCursorPosition(x + 22, y + 1);
                Console.Write(player2card.DisplayCard);

                Console.SetCursorPosition(x + 28, y + 7);
                Console.Write(player2card.DisplayCard);

            y += 8; // increment y so the following game text does not over lap the card

                


                while (player1card.Value == player2card.Value)
                {
                    Console.SetCursorPosition(x, y + 2);
                    Console.WriteLine("Draw!\nAdd 3 cards to the pool!");
                    // Console.WriteLine("Press any key to continue...");
                    // Console.ReadKey();
                    Thread.Sleep(2000);
                    Console.Clear();
                    if (Player1.Deck.Count < 4)
                    {
                        Player1.Deck.Clear();
                        return;
                    }
                    if (Player2.Deck.Count < 4)
                    {
                        Player2.Deck.Clear();
                        return;
                    }

                    // draw 3 cards from both sides

                    pool.Enqueue(Player1.Deck.Dequeue());
                    pool.Enqueue(Player1.Deck.Dequeue());
                    pool.Enqueue(Player1.Deck.Dequeue());

                    pool.Enqueue(Player2.Deck.Dequeue());
                    pool.Enqueue(Player2.Deck.Dequeue());
                    pool.Enqueue(Player2.Deck.Dequeue());

                    // now flip the next card from both sides

                    player1card = Player1.Deck.Dequeue();
                    player2card = Player2.Deck.Dequeue();

                    pool.Enqueue(player1card);
                    pool.Enqueue(player2card);



                x = 0;
                y = 2;

                Console.WriteLine(Player1.Name + " plays: " + "       " + Player2.Name + " plays: ");


                // Draw Suit

                DrawCard.DrawOutline(x, y);
                DrawCard.DrawSuitValue(player1card.Suit, x, y);

                // Draw in the card value

                Console.SetCursorPosition(x + 2, y + 1);
                Console.Write(player1card.DisplayCard);

                Console.SetCursorPosition(x + 8, y + 7);
                Console.Write(player1card.DisplayCard);

                // draw the computer's hand

                x += 2; //shift the x coordinate to the right so the computer's hand does not overlap the player hand

                // Draw Suit

                DrawCard.DrawOutline(x, y);
                DrawCard.DrawSuitValue(player2card.Suit, x, y);

                // Draw in the card value

                Console.SetCursorPosition(x + 22, y + 1);
                Console.Write(player2card.DisplayCard);

                Console.SetCursorPosition(x + 28, y + 7);
                Console.Write(player2card.DisplayCard);

                y += 8; // increment y so the following game text does not over lap the card



            }

                if (player1card.Value < player2card.Value)
                {
                    Player2.Deck.Enqueue(pool);
                    Console.SetCursorPosition(x, y + 2);
                    Console.WriteLine(Player2.Name + " takes the hand!");
                }
                else
                {
                    Player1.Deck.Enqueue(pool);
                    Console.SetCursorPosition(x, y + 2);
                    Console.WriteLine(Player1.Name + " takes the hand!");
                }

                foreach (Card card in Player1.Deck)
                {
                    deck1++;
                }

                foreach (Card card in Player2.Deck)
                {
                    //player1deckTotal.Enqueue(Player2.Deck);
                    deck2++;
                }

                Console.WriteLine("Your total number of cards: " + deck1);
                Console.WriteLine("Computer's total number of cards: " + deck2);


                TurnCount++;
            
         }

        

    }

    
    class PlayCards
    {
        
        static void Main(string[] args)
        {


            RunGame();




        }


        static void RunGame()
        {
            int totalTurnCount = 0;
            int turnTime = 10;

            Console.WriteLine("Welcome To My Card Game\n");
            Console.WriteLine("The rules are simple, each player will draw a card.\nThe player with the highest value card keeps both cards.");
            Console.WriteLine("If there is a draw, each player adds 3 cards to a pool\nand then plays the next card.\nThe winner of this round keeps all 10 cards in play.\n");
            Console.WriteLine("Once a player has collected all 52 cards they have won.\n");

            Console.WriteLine("Press Any Key To Begin...");
            Console.ReadKey();

            Console.Clear();

            //Create game

            Game game = new Game("Player", "Computer");
            while (!game.IsEndOfGame())
            {
                game.PlayTurn();

                //Console.WriteLine("Press any key to play the next turn...");
                //Console.ReadKey();

                if (game.TurnCount >= 10 && game.TurnCount <= 50)
                {
                    turnTime = 5;
                }

                if (game.TurnCount >= 51 && game.TurnCount <= 100)
                {
                    turnTime = 3;
                }

                if (game.TurnCount >= 101 && game.TurnCount <= 500)
                {
                    turnTime = 1;
                }

                TurnTime(turnTime); // to speed up gameplay video I automated the process of playing turns

            }

            totalTurnCount += game.TurnCount;

            Console.WriteLine("Total turns: " + totalTurnCount);
            Console.WriteLine("Press enter to start over. Press escape to exit.");

            ConsoleKeyInfo keyinfo;

            keyinfo = Console.ReadKey();

            if (keyinfo.Key == ConsoleKey.Enter)
            {
                Console.Clear();
                string[] startover = new string[0];
                Main(startover);
            }

            if (keyinfo.Key == ConsoleKey.Escape)
            {
                Console.Clear();
                Console.WriteLine("Thanks for playing");
                Environment.Exit(0);

            }

        }
        static void TurnTime(int i)
        {
            Thread.Sleep(100 * i);
        }

        

        
    }

    
}
