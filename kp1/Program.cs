using System;
using System.IO;

namespace MinElementSearch
{
    class Program
    {
        const string project = @"..\..\..";
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            int[,] matrix = null;
            int rows = 0, cols = 0;
            while (true)
            {
                Console.WriteLine("\n--- МЕНЮ---");
                Console.WriteLine("1. Ввести масив вручну");
                Console.WriteLine("2. Згенерувати автоматично (та записати у файл)");
                Console.WriteLine("3. Зчитати з файлу");
                Console.WriteLine("4. Вихід");
                Console.Write("Ваш вибір: ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        InputManual(out matrix, out rows, out cols);
                        FindMinimum(matrix, rows, cols);
                        break;
                    case "2":
                        GenerateRandom(out matrix, out rows, out cols);
                        FindMinimum(matrix, rows, cols);
                        break;
                    case "3":
                        ReadFromFile(out matrix, out rows, out cols);
                        FindMinimum(matrix, rows, cols);
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір.");
                        break;
                }
            }
        }
        // Метод пошуку мінімального елемента (Основна логіка)
        static void FindMinimum(int[,] matrix, int rows, int cols)
        {
            if (matrix == null || rows == 0 || cols == 0) return;
            int minVal = matrix[0, 0];
            int minRow = 0;
            int minCol = 0;
            // Складність O(N * M)
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (matrix[i, j] < minVal)
                    {
                        minVal = matrix[i, j];
                        minRow = i;
                        minCol = j;
                    }
                }
            }
            Console.WriteLine($"\nРЕЗУЛЬТАТ:");
            Console.WriteLine($"Мінімальний елемент: {minVal}");
            Console.WriteLine($"Індекси: [{minRow}, {minCol}] (Рядок:{minRow + 1}, Стовпець: {minCol + 1})");
        }
        static void InputManual(out int[,] matrix, out int rows, out int cols)
        {
            Console.Write("Введіть кількість рядків: ");
            rows = int.Parse(Console.ReadLine());
            Console.Write("Введіть кількість стовпців: ");
            cols = int.Parse(Console.ReadLine());
            matrix = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                string[] line = Console.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < cols; j++)
                {
                    if (j < line.Length)
                        matrix[i, j] = int.Parse(line[j]);
                    else // Якщо в рядку менше чисел, ніж треба - просимо ввести
                    {
                        Console.Write($"Введіть елемент [{i},{j}]: ");
                        matrix[i, j] = int.Parse(Console.ReadLine());
                    }
                }
            }
        }
        static void GenerateRandom(out int[,] matrix, out int rows, out int cols)
        {
            Console.Write("Введіть кількість рядків: ");
            rows = int.Parse(Console.ReadLine());
            Console.Write("Введіть кількість стовпців: ");
            cols = int.Parse(Console.ReadLine());
            matrix = new int[rows, cols];
            Random rnd = new Random();
            string fileContent = $"{rows} {cols}\n";
            Console.WriteLine("Згенерований масив:");
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = rnd.Next(-100, 100); // Числа від -100 до 100
                    Console.Write(matrix[i, j] + "\t");
                    fileContent += matrix[i, j] + " ";
                }
                Console.WriteLine();
                fileContent += "\n";
            }

            File.WriteAllText($@"{project}\matrix_data.txt", fileContent);
            Console.WriteLine("Дані записано у файл 'matrix_data.txt'.");
        }
        static void ReadFromFile(out int[,] matrix, out int rows, out int cols)
        {
            rows = 0;
            cols = 0;
            matrix = null;

            if (!File.Exists($@"{project}\matrix_data.txt"))
            {
                Console.WriteLine("Файл не знайдено!");
                return;
            }

            string[] lines = File.ReadAllLines($@"{project}\matrix_data.txt");
            string[] dims = lines[0].Split(' ');
            rows = int.Parse(dims[0]);
            cols = int.Parse(dims[1]);
            matrix = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                string[] nums = lines[i + 1].Split(new[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = int.Parse(nums[j]);
                }
            }
            Console.WriteLine("Дані успішно зчитано з файлу.");
        }
    }
}