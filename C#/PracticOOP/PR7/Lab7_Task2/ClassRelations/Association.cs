using System;

namespace ClassRelations
{
    // =========================================================================
    //  АССОЦИАЦИЯ — самая слабая связь: один объект просто ЗНАЕТ про другой
    //  и пользуется им. Никто никем не владеет, объекты живут сами по себе.
    //
    //  Как узнать в коде: в классе есть ссылка на другой объект (поле или
    //  параметр метода), но этот объект создан где-то снаружи и может
    //  в любой момент поменяться или вообще отсутствовать (null).
    //
    //  Пример: студент и преподаватель. Преподаватель не "состоит" из
    //  студентов, студент не "состоит" из преподавателя — они просто связаны.
    // =========================================================================

    /// <summary>Преподаватель.</summary>
    public class Teacher
    {
        public string Name { get; }

        public Teacher(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Преподаватель пользуется студентом только внутри метода —
        /// студент приходит как параметр. Это тоже ассоциация.
        /// </summary>
        public void CheckHomework(Student student)
        {
            Console.WriteLine(Name + " проверяет работу студента " + student.Name);
        }
    }

    /// <summary>Студент.</summary>
    public class Student
    {
        public string Name { get; }

        /// <summary>
        /// Ссылка на руководителя. Её можно менять (set), и она может
        /// быть пустой (null) — студент прекрасно существует и без неё.
        /// </summary>
        public Teacher Mentor { get; set; }

        public Student(string name)
        {
            Name = name;
        }

        public void AskQuestion()
        {
            if (Mentor == null)
            {
                Console.WriteLine(Name + ": мне некому задать вопрос");
            }
            else
            {
                Console.WriteLine(Name + " задаёт вопрос преподавателю " + Mentor.Name);
            }
        }
    }

    public static class AssociationDemo
    {
        public static void Run()
        {
            // Оба объекта создаются отдельно и независимо друг от друга
            Teacher ivanov = new Teacher("Иванов И.И.");
            Teacher petrova = new Teacher("Петрова А.С.");
            Student student = new Student("Олег");

            student.AskQuestion();            // руководителя пока нет

            student.Mentor = ivanov;          // связали
            student.AskQuestion();
            ivanov.CheckHomework(student);

            student.Mentor = petrova;         // связь легко поменять
            student.AskQuestion();

            student.Mentor = null;            // и разорвать
            Console.WriteLine("Связь разорвана, но оба объекта существуют: " +
                              student.Name + " и " + petrova.Name);
        }
    }
}
