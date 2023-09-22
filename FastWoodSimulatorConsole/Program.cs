using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        // 1. Ввести одномерный массив с количеством элементов N
        int N = 1000000; // Задайте желаемый размер массива
        double[] a = new double[N];
        double[] b = new double[N];
        int K = 10; // Задайте параметр сложности

        Random random = new Random();
        for (int i = 0; i < N; i++)
        {
            a[i] = random.NextDouble(); // Заполняем массив случайными значениями
        }

        // 2. Обработать каждый элемент массива с использованием многопоточности
        Stopwatch stopwatch = new Stopwatch();

        // Без использования многопоточности
        stopwatch.Start();
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < K; j++)
            {
                b[i] += Math.Pow(a[i], 1.789);
            }
        }
        stopwatch.Stop();
        Console.WriteLine($"Время обработки без многопоточности: {stopwatch.ElapsedMilliseconds} мс");

        // С использованием многопоточности
        stopwatch.Reset();
        stopwatch.Start();
        Parallel.For(0, N, i =>
        {
            for (int j = 0; j < K; j++)
            {
                b[i] += Math.Pow(a[i], 1.789);
            }
        });
        stopwatch.Stop();
        Console.WriteLine($"Время обработки с многопоточностью: {stopwatch.ElapsedMilliseconds} мс");

        // 5. Сравнить результаты
        bool resultsMatch = true;
        for (int i = 0; i < N; i++)
        {
            if (Math.Abs(b[i] - b[0]) > double.Epsilon)
            {
                resultsMatch = false;
                break;
            }
        }

        if (resultsMatch)
        {
            Console.WriteLine("Результаты совпадают.");
        }
        else
        {
            Console.WriteLine("Результаты не совпадают.");
        }
    }
}
