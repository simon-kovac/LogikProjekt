using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

public enum Color { blue, green, red, yellow, orange, pink, empty }
public enum Rating { correct, halfCorrect, wrong, empty }

namespace Logik
{
    class GameBoard
    {
        public static Color[,] guesses = new Color[4, 7]; //Ukládá historii pokusů
        public static Rating[,] ratings = new Rating[4, 7]; //Ukládá historii hodnocení
        public static int currentGuessRow = 0; //Urcuje kolikaty je aktualni pokus

        

        public static void ClearBoard() //Resetuje historii pokusů
        {
            for (int y = 0; y < 7; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    guesses[x, y] = Color.empty;
                    ratings[x, y] = Rating.empty;
                }
            }
            currentGuessRow = 0;
            GameController.SetupGuess();
        }

        public static void UpdateRow() //Aktualizuje poslední guess podle aktualnich informaci v GuessController
        {
            
            for (int i = 0; i < 4; i++)
            {
                guesses[i, currentGuessRow] = GameController.guess[i];
                ratings[i, currentGuessRow] = GameController.rating[i];
            }
            currentGuessRow++;
            
        }
    }
}
