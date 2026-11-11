namespace LinqDemo
{
    /// <summary>Студент — данные, по которым будут строиться запросы.</summary>
    public class Student
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public int Age { get; set; }
        public double Mark { get; set; }   // средний балл

        public Student(string name, string group, int age, double mark)
        {
            Name = name;
            Group = group;
            Age = age;
            Mark = mark;
        }

        // Чтобы Console.WriteLine(student) печатал понятную строку
        public override string ToString()
        {
            return Name + " (" + Group + ", возраст " + Age + ", балл " + Mark + ")";
        }
    }

    /// <summary>Куратор группы — вторая "таблица", нужна для примера с join.</summary>
    public class Curator
    {
        public string Group { get; set; }
        public string Teacher { get; set; }

        public Curator(string group, string teacher)
        {
            Group = group;
            Teacher = teacher;
        }
    }
}
