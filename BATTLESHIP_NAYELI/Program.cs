using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BATTLESHIP_NAYELI
{
    using System;

        internal class Program
        {
            static void Main(string[] args)
        {
            bool exitGame = false;

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("*********Batalla Naval*********");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("¡Bienvenido a Batalla Naval!");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("*******************************");
            Console.ResetColor();

            while (!exitGame)
            {
                Console.WriteLine("\nSeleccione una opción:");
                Console.WriteLine("1. Iniciar Juego");
                Console.WriteLine("2. Salir del Juego");
                Console.Write("Opción: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        PlayGame();
                        break;
                    case "2":
                        exitGame = true;
                        Console.WriteLine("¡Gracias por jugar!");
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Por favor, seleccione una opción válida.");
                        break;
                }
            }
        }

        static void PlayGame()
        {
            const int boardSize = 10;
            const int numShips = 4;

            var playerBoard = new Board(boardSize);
            var computerBoard = new Board(boardSize);
            var random = new Random();

            InitializeShips(playerBoard, numShips, random);
            InitializeShips(computerBoard, numShips, random);

            int playerScore = 0;
            int computerScore = 0;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("¡Hundan los barcos enemigos para ganar!");
            Console.ResetColor();

            // Mostrar tablero de barcos del jugador
            Console.WriteLine("\nTablero del jugador (Posición de los barcos):");
            playerBoard.DisplayWithShips();

            // Mostrar tablero de barcos de la computadora
            Console.WriteLine("\nTablero de la computadora (Posición de los barcos):");
            computerBoard.DisplayWithShips();

            while (!playerBoard.AllShipsSunk() && !computerBoard.AllShipsSunk())
            {
                Console.WriteLine("\nTablero del jugador:");
                playerBoard.Display();

                Console.WriteLine("\nTablero de la computadora:");
                computerBoard.Display();

                Console.WriteLine("\n¡Es tu turno! Ingresa las coordenadas del ataque (por ejemplo, 'A5'), o escribe 'Salir' para salir del juego:");
                string playerMove = Console.ReadLine().ToUpper();

                if (playerMove == "SALIR")
                {
                    Console.WriteLine("¡Saliendo del juego!");
                    return;
                }

                if (playerMove.Length < 2 || !char.IsLetter(playerMove[0]) || !char.IsDigit(playerMove[1]))
                {
                    Console.WriteLine("Entrada inválida. Por favor, ingrese una coordenada válida o escriba 'Salir'.");
                    continue;
                }

                int col = playerMove[0] - 'A';
                int row = int.Parse(playerMove.Substring(1)) - 1;

                if (col < 0 || col >= boardSize || row < 0 || row >= boardSize)
                {
                    Console.WriteLine("Coordenada fuera de rango. Por favor, ingrese una coordenada válida o escriba 'Salir'.");
                    continue;
                }

                bool hit = computerBoard.Attack(row, col);
                if (hit)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("¡Has golpeado un barco enemigo!");
                    Console.ResetColor();
                    playerScore++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("¡Agua!");
                    Console.ResetColor();
                }

                if (computerBoard.AllShipsSunk())
                    break;

                Console.WriteLine("\nPresione Enter para el turno de la computadora...");
                Console.ReadLine();

                (row, col) = GetRandomMove(random, boardSize);
                hit = playerBoard.Attack(row, col);
                if (hit)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("¡La computadora ha golpeado tu barco!");
                    Console.ResetColor();
                    computerScore++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("¡La computadora ha fallado!");
                    Console.ResetColor();
                }

                Console.WriteLine("\nPresione Enter para continuar...");
                Console.ReadLine();

                // Limpiar la consola para el próximo turno
                Console.Clear();
            }

            Console.WriteLine("\n¡Juego terminado!");
            Console.WriteLine("Puntajes:");
            Console.WriteLine($"Jugador: {playerScore}");
            Console.WriteLine($"Computadora: {computerScore}");

            if (playerScore > computerScore)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("¡Felicidades, has ganado!");
                Console.ResetColor();
            }
            else if (playerScore < computerScore)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("¡La computadora ha ganado!");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("¡Es un empate!");
                Console.ResetColor();
            }
        }

        static void InitializeShips(Board board, int numShips, Random random)
        {
            for (int i = 1; i <= numShips; i++)
            {
                int size = 5 - i; // Modificar tamaño de los barcos
                bool isHorizontal = random.Next(2) == 0;
                board.PlaceRandomShip(size, isHorizontal, random);
            }
        }

        static (int, int) GetRandomMove(Random random, int boardSize)
        {
            int row = random.Next(boardSize);
            int col = random.Next(boardSize);
            return (row, col);
        }
    }

    class Board
    {
        private char[,] _board;
        private HashSet<(int, int)> _occupied;

        public Board(int size)
        {
            _board = new char[size, size];
            _occupied = new HashSet<(int, int)>();
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < _board.GetLength(0); i++)
            {
                for (int j = 0; j < _board.GetLength(1); j++)
                {
                    _board[i, j] = '-';
                }
            }
        }

        public void Display()
        {
            Console.Write("  ");
            for (int i = 0; i < _board.GetLength(1); i++)
            {
                Console.Write((char)('A' + i) + " ");
            }
            Console.WriteLine();

            for (int i = 0; i < _board.GetLength(0); i++)
            {
                Console.Write((i + 1) + " ");
                for (int j = 0; j < _board.GetLength(1); j++)
                {
                    char cell = _board[i, j];
                    if (cell == 'X')
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(cell + " ");
                        Console.ResetColor();
                    }
                    else if (cell == 'O')
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(cell + " ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(cell + " ");
                    }
                }
                Console.WriteLine();
            }
        }

        public void DisplayWithShips()
        {
            Console.Write("  ");
            for (int i = 0; i < _board.GetLength(1); i++)
            {
                Console.Write((char)('A' + i) + " ");
            }
            Console.WriteLine();

            for (int i = 0; i < _board.GetLength(0); i++)
            {
                Console.Write((i + 1) + " ");
                for (int j = 0; j < _board.GetLength(1); j++)
                {
                    char cell = _board[i, j];
                    if (_occupied.Contains((i, j)))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("S ");
                    }
                    else
                    {
                        Console.Write(cell + " ");
                    }
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }

        public bool Attack(int row, int col)
        {
            if (_occupied.Contains((row, col)))
            {
                _occupied.Remove((row, col));
                _board[row, col] = 'X';
                return true;
            }
            else
            {
                _board[row, col] = 'O';
                return false;
            }
        }

        public bool AllShipsSunk()
        {
            return _occupied.Count == 0;
        }

        public void PlaceRandomShip(int size, bool isHorizontal, Random random)
        {
            int row, col;
            do
            {
                row = random.Next(_board.GetLength(0));
                col = random.Next(_board.GetLength(1));
            } while (!CanPlaceShip(row, col, size, isHorizontal));

            for (int i = 0; i < size; i++)
            {
                if (isHorizontal)
                    _occupied.Add((row, col + i));
                else
                    _occupied.Add((row + i, col));
            }
        }

        private bool CanPlaceShip(int row, int col, int size, bool isHorizontal)
        {
            if (isHorizontal && col + size > _board.GetLength(1))
                return false;
            if (!isHorizontal && row + size > _board.GetLength(0))
                return false;

            for (int i = 0; i < size; i++)
            {
                if (isHorizontal && _occupied.Contains((row, col + i)))
                    return false;
                if (!isHorizontal && _occupied.Contains((row + i, col)))
                    return false;
            }
            return true;
        }
    }
}
