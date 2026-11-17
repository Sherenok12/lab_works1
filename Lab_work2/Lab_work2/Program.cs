
using System;
using UniversityApp.Repository;
using UniversityApp.Services;

namespace UniversityApp
{
    class Program
    {
        static UniversityRepository repository = new UniversityRepository();

        static UniversityInfoService infoService =
            new UniversityInfoService(repository);

        static void Main()
        {
            AddTestData();

            int choice = -1;

            do
            {
                Console.WriteLine("\n *СИСТЕМА УНІВЕРСИТЕТУ*");
                Console.WriteLine("1. Переглянути групи та студентів");
                Console.WriteLine("2. Переглянути студентські квитки");
                Console.WriteLine("3. Переглянути курси та студентів");
                Console.WriteLine("4. Переглянути інформацію про студента");
                Console.WriteLine("5. Додати групу");
                Console.WriteLine("6. Зареєструвати студента");
                Console.WriteLine("7. Видати студентський квиток");
                Console.WriteLine("8. Записати студента на курс");
                Console.WriteLine("9. Виставити або змінити оцінку");
                Console.WriteLine("10. Додати кафедру");
                Console.WriteLine("11. Додати викладача");
                Console.WriteLine("12. Створити курс");
                Console.WriteLine("13. Переглянути кафедри та викладачів");
                Console.WriteLine("14. Переглянути курси викладачів");
                Console.WriteLine("15. Звіт про успішність");
                Console.WriteLine("16. Повний звіт");
                Console.WriteLine("0. Вихід");
                Console.Write("Ваш вибір: ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Введіть номер пункту меню.");
                    choice = -1;
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        case 1:
                            infoService.PrintGroupsWithStudents();
                            break;

                        case 2:
                            infoService.PrintStudentCards();
                            break;

                        case 3:
                            infoService.PrintCoursesWithStudents();
                            break;

                        case 4:
                            ShowStudentInfo();
                            break;

                        case 5:
                            AddGroup();
                            break;

                        case 6:
                            AddStudent();
                            break;

                        case 7:
                            AddStudentCard();
                            break;

                        case 8:
                            EnrollStudent();
                            break;

                        case 9:
                            SetGrade();
                            break;

                        case 10:
                            AddDepartment();
                            break;

                        case 11:
                            AddTeacher();
                            break;

                        case 12:
                            AddCourse();
                            break;

                        case 13:
                            infoService.PrintDepartmentsWithTeachers();
                            break;

                        case 14:
                            infoService.PrintTeachersWithCourses();
                            break;

                        case 15:
                            infoService.PrintStudentPerformance();
                            break;

                        case 16:
                            infoService.PrintAll();
                            break;

                        case 0:
                            Console.WriteLine("Програму завершено.");
                            break;

                        default:
                            Console.WriteLine("Такого пункту меню немає.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Помилка: " + ex.Message);
                }

            } while (choice != 0);
        }


        static void AddTestData()
        {
            var depIpz = repository.AddDepartment("ІПЗ");
            var depKn = repository.AddDepartment("КН");

            var teacher1 = repository.AddTeacher("Треньов М.Г.", depIpz.Id);
            var teacher2 = repository.AddTeacher("Коваленко І.П.", depIpz.Id);
            var teacher3 = repository.AddTeacher("Мельник О.В.", depKn.Id);

            var group1 = repository.AddGroup("ПД-31");
            var group2 = repository.AddGroup("ПД-32");

            var student1 = repository.AddStudent("Тараканов Родион", group1.Id);
            var student2 = repository.AddStudent("Трошкін Роман", group2.Id);
            var student3 = repository.AddStudent("Шеренок Олександр", group2.Id);

            

            repository.AddStudentCard(
                student1.Id, "S-01", new DateTime(2026, 9, 1));

            repository.AddStudentCard(
                student2.Id, "S-02", new DateTime(2026, 9, 1));

            repository.AddStudentCard(
                student3.Id, "S-03", new DateTime(2026, 9, 1));

            var course1 = repository.AddCourse(
                "Програмування", 5, teacher1.Id);

            var course2 = repository.AddCourse(
                "Бази даних", 4, teacher2.Id);

            var course3 = repository.AddCourse(
                "Вища математика", 3, teacher3.Id);

            repository.EnrollStudent(student1.Id, course1.Id, 91);
            repository.EnrollStudent(student1.Id, course2.Id, 88);
            repository.EnrollStudent(student2.Id, course1.Id, 91);
            repository.EnrollStudent(student2.Id, course3.Id, 84);
            repository.EnrollStudent(student3.Id, course2.Id, 77);
            repository.EnrollStudent(student3.Id, course3.Id, 90);


            Console.WriteLine("\n*ДЕМОНСТРАЦІЯ СТАНІВ КУРСУ*");

            Console.WriteLine("Студент: " + student3.FullName);
            Console.WriteLine("Курс: " + course2.Title);

            Console.WriteLine("\n1. Початковий стан: Enrolled");

            repository.SetGrade(student3.Id, course2.Id, 40);
            Console.WriteLine("2. Після оцінки 40: Failed");

            repository.RetakeCourse(student3.Id, course2.Id);
            Console.WriteLine("3. Після перездачі: Retake");

            repository.SetGrade(student3.Id, course2.Id, 85);
            Console.WriteLine("4. Після оцінки 85: Passed");

            Console.WriteLine("\n*ДЕМОНСТРАЦІЮ ЗАВЕРШЕНО*");
            Console.WriteLine("Натисніть Enter для переходу до меню.");
            Console.ReadLine();
        }


        static void ShowStudentInfo()
        {
            Console.WriteLine("Список студентів:");

            foreach (var student in repository.GetStudents())
            {
                Console.WriteLine($"{student.Id}. {student.FullName}");
            }

            Console.Write("Введіть ID студента: ");
            int id = ReadNumber();

            infoService.PrintStudentFullInfo(id);
        }

        static void AddGroup()
        {
            string name = ReadText("Введіть назву групи: ");

            var group = repository.AddGroup(name);

            Console.WriteLine($"Групу {group.Name} додано.");
        }


        static void AddStudent()
        {
            Console.WriteLine("Список груп:");

            foreach (var group in repository.GetGroups())
            {
                Console.WriteLine($"{group.Id}. {group.Name}");
            }

            Console.Write("Введіть ID групи: ");
            int groupId = ReadNumber();

            string fullName = ReadText("Введіть ПІБ студента: ");

            var student = repository.AddStudent(fullName, groupId);

            Console.WriteLine(
                $"Студента додано. Його ID: {student.Id}");
        }

        static void AddStudentCard()
        {
            Console.WriteLine("Список студентів:");

            foreach (var student in repository.GetStudents())
            {
                Console.WriteLine($"{student.Id}. {student.FullName}");
            }

            Console.Write("Введіть ID студента: ");
            int studentId = ReadNumber();

            string cardNumber = ReadText("Введіть номер квитка: ");

            repository.AddStudentCard(
                studentId, cardNumber, DateTime.Today);

            Console.WriteLine("Студентський квиток видано.");
        }

        static void EnrollStudent()
        {
            Console.WriteLine("Список студентів:");

            foreach (var student in repository.GetStudents())
            {
                Console.WriteLine($"{student.Id}. {student.FullName}");
            }

            Console.Write("Введіть ID студента: ");
            int studentId = ReadNumber();

            Console.WriteLine("Список курсів:");

            foreach (var course in repository.GetCourses())
            {
                Console.WriteLine($"{course.Id}. {course.Title}");
            }

            Console.Write("Введіть ID курсу: ");
            int courseId = ReadNumber();

            repository.EnrollStudent(studentId, courseId);

            Console.WriteLine("Студента записано на курс.");
        }

        static void SetGrade()
        {
            Console.WriteLine("Список студентів:");

            foreach (var student in repository.GetStudents())
            {
                Console.WriteLine($"{student.Id}. {student.FullName}");
            }

            Console.Write("Введіть ID студента: ");
            int studentId = ReadNumber();

            Console.WriteLine("Список курсів:");

            foreach (var course in repository.GetCourses())
            {
                Console.WriteLine($"{course.Id}. {course.Title}");
            }

            Console.Write("Введіть ID курсу: ");
            int courseId = ReadNumber();

            Console.Write("Введіть оцінку від 0 до 100: ");
            int grade = ReadNumber();

            repository.SetGrade(studentId, courseId, grade);

            Console.WriteLine("Оцінку збережено.");
        }

        static void AddDepartment()
        {
            string name = ReadText("Введіть назву кафедри: ");

            var department = repository.AddDepartment(name);

            Console.WriteLine($"Кафедру {department.Name} додано.");
        }

        static void AddTeacher()
        {
            Console.WriteLine("Список кафедр:");

            foreach (var department in repository.GetDepartments())
            {
                Console.WriteLine($"{department.Id}. {department.Name}");
            }

            Console.Write("Введіть ID кафедри: ");
            int departmentId = ReadNumber();

            string fullName = ReadText("Введіть ПІБ викладача: ");

            var teacher = repository.AddTeacher(fullName, departmentId);

            Console.WriteLine(
                $"Викладача {teacher.FullName} додано. ID: {teacher.Id}");
        }

        static void AddCourse()
        {
            string title = ReadText("Введіть назву курсу: ");

            Console.Write("Введіть кількість кредитів: ");
            int credits = ReadNumber();

            Console.WriteLine("Список викладачів:");

            foreach (var teacher in repository.GetTeachers())
            {
                Console.WriteLine($"{teacher.Id}. {teacher.FullName}");
            }

            Console.Write("Введіть ID викладача: ");
            int teacherId = ReadNumber();

            var course = repository.AddCourse(title, credits, teacherId);

            Console.WriteLine($"Курс {course.Title} створено.");
        }
        static int ReadNumber()
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int number))
                {
                    return number;
                }

                Console.Write("Введіть ціле число: ");
            }
        }

        static string ReadText(string message)
        {
            while (true)
            {
                Console.Write(message);

                string text = Console.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text.Trim();
                }

                Console.WriteLine("Поле не може бути порожнім.");
            }
        }




    }
}
