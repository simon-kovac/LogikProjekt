using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logik
{
    class GameController
    {
        public static Color[] answer = new Color[4];
        public static Color[] guess = new Color[4];
        public static Rating[] rating = new Rating[4];
        public static bool gameOn = true;
        public static bool didWin;

        public static void SetupGuess() //Nastaví guess na vsechny modre
        {
            for (int i = 0; i < 4; i++)
            {
                guess[i] = Color.blue;
            }
        }


        public static void GenerateAnswer() //Nahodne generuje spravnou odpoved
        {
            Random rnd = new Random();

            for (int i = 0; i < 4; i++)
            {
                switch (rnd.Next(0, 6))
                {
                    case 0:
                        answer[i] = Color.blue;
                        break;
                    case 1:
                        answer[i] = Color.green;
                        break;
                    case 2:
                        answer[i] = Color.red;
                        break;
                    case 3:
                        answer[i] = Color.yellow;
                        break;
                    case 4:
                        answer[i] = Color.orange;
                        break;
                    case 5:
                        answer[i] = Color.pink;
                        break;
                }
            }
        }

        public static void ChangeGuessColor(int pinIndex)
        { //Zmeni barvu na danem indexu guessu
            if (pinIndex < 0 || pinIndex > 3)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                if ((int)guess[pinIndex] == 5)
                {
                    guess[pinIndex] = (Color)0;
                }
                else
                {
                    guess[pinIndex] = (Color)((int)guess[pinIndex] + 1);
                }
            }
        }


        public static void Rate() //Zhodnoti guess, pokud je spravny tak ukonci hru
        {
            for (int i = 0; i < 4; i++)
            {
                if (answer[i] == guess[i])
                {
                    rating[i] = Rating.correct;
                }
                else if (ContainsColor(guess[i]))
                {
                    rating[i] = Rating.halfCorrect;
                }
                else
                {
                    rating[i] = Rating.wrong;
                }
            }
            GameBoard.UpdateRow();
            if (IsAllCorrect())
            {
                didWin = true;
            }
        }


        private static bool ContainsColor(Color color)//Zjistuje zda odpoved obsahuje danou barvu
        {
            for (int i = 0; i < 4; i++)
            {
                if (answer[i] == color)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsAllCorrect() //Kontroluje zda je rating kompletne spravne
        {
            for (int i = 0; i < 4; i++)
            {
                if (rating[i] != Rating.correct)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
