using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Task3
{
    public class Student
    {
        public int RecordBookNum { get; set; }
        public string Name { get; set; }

        public Student(int recordBookNum, string name)
        {
            RecordBookNum = recordBookNum;
            Name = name;
        }
    }

    // Однозв'язний список на базі масиву
    public class ArrayList
    {
        private readonly int MAX_SIZE;
        private Student[] arr;
        private int currentSize;

        public ArrayList(int maxSize)
        {
            MAX_SIZE = maxSize;
            arr = new Student[MAX_SIZE];
            currentSize = 0;
        }

        // Додавання із сортуванням за номером залікової (за зростанням)
        public void Add(Student s)
        {
            if (currentSize >= MAX_SIZE) return;

            int i = 0;
            while (i < currentSize && arr[i].RecordBookNum < s.RecordBookNum)
            {
                i++;
            }

            for (int j = currentSize; j > i; j--)
            {
                arr[j] = arr[j - 1];
            }

            arr[i] = s;
            currentSize++;
        }

        public void Remove(int recordBookNum)
        {
            int i = 0;
            while (i < currentSize && arr[i].RecordBookNum != recordBookNum)
            {
                i++;
            }

            if (i < currentSize)
            {
                for (int j = i; j < currentSize - 1; j++)
                {
                    arr[j] = arr[j + 1];
                }
                currentSize--;
            }
        }

        public void ConductExam()
        {
            Console.WriteLine("[Теоретичне опитування (Масив)]");
            int sharedCondition = 10; // Початкова умова
            for (int i = 0; i < currentSize; i++)
            {
                Console.WriteLine($"Студент {arr[i].Name} (Залікова: {arr[i].RecordBookNum}) отримує умову: {sharedCondition}. Здає теорію.");
                sharedCondition += 5; // Формування/передача частини умови для наступного студента
            }

            Console.WriteLine("\n[Практичні завдання (Масив)]");
            sharedCondition = 20; // Нова початкова умова для практики
            for (int i = 0; i < currentSize; i++)
            {
                Console.WriteLine($"Студент {arr[i].Name} (Залікова: {arr[i].RecordBookNum}) отримує умову: {sharedCondition}. Здає практику.");
                sharedCondition += 7; // Формування умови для наступного
            }
        }
    }

    // Однозв'язний список на базі посилань
    // Клас вузла списку
    public class Node
    {
        public Student Data { get; set; }
        public Node? Next { get; set; }

        public Node(Student data)
        {
            Data = data;
            Next = null;
        }
    }

    public class PointerList
    {
        private Node? head;

        public PointerList()
        {
            head = null;
        }

        // Додавання із сортуванням за номером залікової (за зростанням)
        public void Add(Student s)
        {
            Node newNode = new Node(s);

            // Якщо список порожній або новий елемент має бути першим
            if (head == null || head.Data.RecordBookNum >= s.RecordBookNum)
            {
                newNode.Next = head;
                head = newNode;
                return;
            }

            // Шукаємо позицію для вставки
            Node current = head;
            while (current.Next != null && current.Next.Data.RecordBookNum < s.RecordBookNum)
            {
                current = current.Next;
            }

            // Переприсвоєння посилань (вставка вузла)
            newNode.Next = current.Next;
            current.Next = newNode;
        }

        public void Remove(int recordBookNum)
        {
            if (head == null) return;

            // Якщо видаляємо голову списку
            if (head.Data.RecordBookNum == recordBookNum)
            {
                head = head.Next;
                return;
            }

            Node current = head;
            while (current.Next != null && current.Next.Data.RecordBookNum != recordBookNum)
            {
                current = current.Next;
            }

            // Видаляємо вузол, пропускаючи його в ланцюжку
            if (current.Next != null)
            {
                current.Next = current.Next.Next;
            }
        }

        public void ConductExam()
        {
            Console.WriteLine("\n[Теоретичне опитування (Посилання)]");
            int sharedCondition = 10;
            Node current = head;
            while (current != null)
            {
                Console.WriteLine($"Студент {current.Data.Name} (Залікова: {current.Data.RecordBookNum}) отримує умову: {sharedCondition}. Здає теорію.");
                sharedCondition += 5;
                current = current.Next;
            }

            Console.WriteLine("\n[Практичні завдання (Посилання)]");
            sharedCondition = 20;
            current = head;
            while (current != null)
            {
                Console.WriteLine($"Студент {current.Data.Name} (Залікова: {current.Data.RecordBookNum}) отримує умову: {sharedCondition}. Здає практику.");
                sharedCondition += 7;
                current = current.Next;
            }
        }
    }

    public class JSONArr
    {
        [JsonPropertyName("ArraySize")]
        public int ArrSize { get; set; }
    }
    internal class Program
    {
        public int SIZE = 100_000;
        static void Main(string[] args)
        {
            var file = "ArrSize.json";
            string jsonString = File.ReadAllText(file);
            var config = JsonSerializer.Deserialize<JSONArr>(jsonString);

            Random random = new Random();
            Console.OutputEncoding = Encoding.UTF8;

            ArrayList arrayList = new ArrayList(config.ArrSize);
            PointerList ptrList = new PointerList();

            List<string> listNames = new List<string>
            {
                "Олександр",
                "Марія",
                "Іван",
                "Анна",
                "Петро",
                "Катерина",
                "Максим",
                "Дар'я",
                "Денис",
                "Олена",
                "Андрій",
                "Дмитро",
                "Вікторія",
                "Артем"
            };
            var arrNames = listNames.ToArray();
            double 
                elapsedArrayList = 0, 
                elapsedptrList = 0,
                elapsedAddFuncArrayList = 0,
                elapsedAddFuncPtrList = 0,
                elapsedRemovefuncArrList = 0,
                elapsedRemovefuncPtrList = 0;

            Student[] students = new Student[config.ArrSize];
            for (int i = 0; i < config.ArrSize; i++)
            {
                Student randomStudent = new Student(i, arrNames[random.Next(0, listNames.Count)]);
                students[i] = randomStudent;
            }

            // Заміряємо метод Add
            Stopwatch sw = Stopwatch.StartNew();
            foreach (var student in students)
            {
                arrayList.Add(student);
            }
            sw.Stop();
            elapsedAddFuncArrayList = sw.Elapsed.TotalMilliseconds;

            Stopwatch sw2 = Stopwatch.StartNew();
            foreach (var student in students)
            {
                ptrList.Add(student);
            }
            sw2.Stop();
            elapsedAddFuncPtrList = sw2.Elapsed.TotalMilliseconds;

            // Замірюємо список на базі масиву
            Stopwatch stopwatch = Stopwatch.StartNew();
            arrayList.ConductExam();
            stopwatch.Stop();
            elapsedArrayList = stopwatch.Elapsed.TotalMilliseconds;

            // Замірюємо список на базі посилань
            Stopwatch stopwatchptr = Stopwatch.StartNew();
            ptrList.ConductExam();
            stopwatchptr.Stop();
            elapsedptrList = stopwatchptr.Elapsed.TotalMilliseconds;

            // Заміряємо метод Remove
            Stopwatch sw3 = Stopwatch.StartNew();
            foreach (var student in students)
            {
                arrayList.Remove(student.RecordBookNum);
            }
            sw3.Stop();
            elapsedRemovefuncArrList = sw3.Elapsed.TotalMilliseconds;

            Stopwatch sw4 = Stopwatch.StartNew();
            foreach (var student in students)
            {
                ptrList.Remove(student.RecordBookNum);
            }
            sw4.Stop();
            elapsedRemovefuncPtrList = sw4.Elapsed.TotalMilliseconds;

            var msg = "";
            msg += $"Швидкість списку на базі масиву: {elapsedArrayList} мс\n";
            msg += $"Швидкість списку на базі посилань: {elapsedptrList} мс\n\n";

            msg += $"Швидкість методу Add списка на базі масиву: {elapsedAddFuncArrayList} мс\n";
            msg += $"Швидкість методу Add списка на базі посилань: {elapsedAddFuncPtrList} мс\n\n";

            msg += $"Швидкість методу Remove списка на базі масиву: {elapsedRemovefuncArrList} мс\n";
            msg += $"Швидкість методу Remove списка на базі посилань: {elapsedRemovefuncPtrList} мс\n";

            Console.WriteLine(msg);
            Console.ReadLine();
        }
    }
}
