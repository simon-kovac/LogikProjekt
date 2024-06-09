using System;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Logik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BuildBoard();
            GameController.gameOn = true;
            GameController.didWin = false;
            GameBoard.ClearBoard();
            UpdateGuessHistory();
            GameController.GenerateAnswer();
            GameController.SetupGuess();
        }

        private void BuildBoard() //Zavolá veškeré metody pro přidání potřebných elementů
        {
            AddRatingGrids();
            AddHistoryRects();
            AddCurrentGuessBtns();
        }

        public void AddRatingGrids() //Přídá 2x2 gridy na hodnocení do každé řady 
        {
            for (int y = 0; y < 7; y++)
            {
                Grid nestedGrid = new Grid
                {
                    ShowGridLines = true
                };

                for (int i = 0; i < 2; i++)
                {
                    nestedGrid.RowDefinitions.Add(new RowDefinition());
                    nestedGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                for (int r = 0; r < 2; r++)
                {
                    for (int c = 0; c < 2; c++)
                    {
                        Rectangle rect = new Rectangle
                        {
                            Fill = Brushes.White,
                            Stroke = Brushes.Black
                        };
                        Grid.SetRow(rect, r);
                        Grid.SetColumn(rect, c);
                        nestedGrid.Children.Add(rect);
                    }
                }

                Grid.SetRow(nestedGrid, y);
                Grid.SetColumn(nestedGrid, 0);
                ratingGrid.Children.Add(nestedGrid);
            }
        }
        public void AddHistoryRects() //Přidá čtverce do každé buňky hlavní herní plochy
        {
            for (int y = 0; y < 7;y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Fill = Brushes.White,
                        Stroke = Brushes.Black
                    };
                    Grid.SetRow(rect, y);
                    Grid.SetColumn(rect, x);
                    guessHistoryGrid.Children.Add(rect);
                }
            }
        }
        public void AddCurrentGuessBtns() //Přidá tlačítka pro zobrazení aktuálního guessu
        {
            for (int i = 0; i < 4; i++)
            {
                Button btn = new Button {};
                btn.Click += ChangeColor;
                btn.Background = new SolidColorBrush(Colors.Blue);
                Grid.SetColumn(btn, i);
                currentGuessGrid.Children.Add(btn);
            }
        }

        public void ChangeColor(object sender, RoutedEventArgs e)
        {
            if (GameController.gameOn)
            {
                Button clickedButton = sender as Button;
                int columnIndex = Grid.GetColumn(clickedButton);
                GameController.ChangeGuessColor(columnIndex);
                UpdateGuess(columnIndex);
            }
        }

        public void UpdateGuessHistory() //Vykresli historii guessu
        {
            foreach (UIElement element in guessHistoryGrid.Children) //Vykresli guessy
            {
                if (element is Rectangle rect)
                {
                    int x = Grid.GetColumn(rect);
                    int y = Grid.GetRow(rect);
                    rect.Fill = new SolidColorBrush(GetColor(GameBoard.guesses[x, y]));
                }
            }
            foreach (UIElement element in ratingGrid.Children) //Vykresli rating jednotlivych guessu
            {
                if (element is Grid nestedGrid)
                {
                    int y = Grid.GetRow(nestedGrid);

                    foreach (UIElement nestedElement in nestedGrid.Children)
                    {
                        if (nestedElement is Rectangle rect)
                        {
                            int row = Grid.GetRow(nestedElement);
                            int column = Grid.GetColumn(nestedElement);
                            int index = GetGridIndex(row, column, 2);

                            rect.Fill = new SolidColorBrush(GetRatingColor(GameBoard.ratings[index, y]));
                        }
                    }
                }
            }

            if (GameController.didWin) //Pokud má všechny odpovědi správně, ukončí hru na výhře
            {
                EndGame(true);
            }
            else if(GameController.gameOn && GameBoard.currentGuessRow == 7) //Pokud došly pokusy, ukončí hru na prohře
            {
                EndGame(false);
            }
        }

        public void UpdateGuess(int index) //Vykresli barvy aktuálního guessu
        {
           foreach(UIElement element in currentGuessGrid.Children)
            {
                if (Grid.GetColumn(element) == index)
                {
                    if(element is Button button)
                    {
                        button.Background = new SolidColorBrush(GetColor(GameController.guess[index]));
                    }
                }
            }
        }

        public System.Windows.Media.Color GetColor(Color color) //Převede enumeraci Color na realnou barvu
        {
            switch (color)
            {
                case Color.blue:
                    return System.Windows.Media.Colors.Blue;
                case Color.green:
                    return System.Windows.Media.Colors.Green;
                case Color.red:
                    return System.Windows.Media.Colors.Red;
                case Color.yellow:
                    return System.Windows.Media.Colors.Yellow;
                case Color.orange:
                    return System.Windows.Media.Colors.Orange;
                case Color.pink:
                    return System.Windows.Media.Colors.Pink;
                default:
                    return System.Windows.Media.Colors.DarkGray;
            }
        }

        public System.Windows.Media.Color GetRatingColor(Rating rating) //Převede enumeraci Rating na odpovídající barvu
        {
            switch (rating)
            {
                case Rating.correct:
                    return System.Windows.Media.Colors.LawnGreen;
                case Rating.halfCorrect:
                    return System.Windows.Media.Colors.Orange;
                case Rating.wrong:
                    return System.Windows.Media.Colors.Red;
                default:
                    return System.Windows.Media.Colors.DarkGray;
            }
        }

        public void SendAnswer(object sender, RoutedEventArgs e) //Odešle guess
        {
            if (GameController.gameOn)
            {
                GameController.Rate();
                UpdateGuessHistory();
            }
        }

        public int GetGridIndex(int row, int column, int numberOfColumns) //Vrátí pořadí prvku v tabulce, počítá se stejným způsobem jako se čte, zleva do prava, odzhora dolu
        {
            return row * numberOfColumns + column;
        }

        private void NewTry(object sender, RoutedEventArgs e) //Zahájí nový pokud
        {
            if (!GameController.gameOn)
            {
                GameController.gameOn = true;
                GameController.didWin = false;
                GameBoard.ClearBoard();
                btnResult.Content = "L O G I K";
                btnResult.Background = new SolidColorBrush(Colors.Black);
                btnResult.Foreground = new SolidColorBrush(Colors.DarkRed);
                UpdateGuessHistory();
                for(int i = 0; i < 4; i++)
                {
                    UpdateGuess(i);
                }
                GameController.GenerateAnswer();
                GameController.SetupGuess();
            }
        }

        public void EndGame(bool victory)
        {
            GameController.gameOn = false;
            if(victory)
            {
                btnResult.Content = "V Ý H R A";
                btnResult.Background = new SolidColorBrush(Colors.LightGreen);
                btnResult.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                btnResult.Content = "P R O H R A";
                btnResult.Background = new SolidColorBrush(Colors.Red);
                btnResult.Foreground = new SolidColorBrush(Colors.DarkRed);
            }
        }
    }
}