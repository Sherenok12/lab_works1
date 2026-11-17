
using System;
using System.Collections.Generic;
using System.Linq;
using UniversityApp.Models;

namespace UniversityApp.Repository
{
    public class UniversityRepository
    {
        private readonly List<Group> _groups = new();
        private readonly List<Student> _students = new();
        private readonly List<StudentCard> _cards = new();
        private readonly List<Course> _courses = new();
        private readonly List<Enrollment> _enrollments = new();
        private readonly List<Department> _departments = new();
        private readonly List<Teacher> _teachers = new();
   

        private int _departmentId = 1;
        private int _teacherId = 1;
        private int _groupId = 1;
        private int _studentId = 1;
        private int _cardId = 1;
        private int _courseId = 1;

        public Group AddGroup(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Назва групи не може бути порожньою");

            var group = new Group
            {
                Id = _groupId++,
                Name = name
            };

            _groups.Add(group);
            return group;
        }

        public IReadOnlyList<Group> GetGroups()
        {
            return _groups;
        }

        public Student AddStudent(string fullName, int groupId)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Ім'я студента не може бути порожнім");

            Group group = _groups.FirstOrDefault(g => g.Id == groupId);

            if (group == null)
                throw new ArgumentException("Групу не знайдено");

            var student = new Student
            {
                Id = _studentId++,
                FullName = fullName,
                GroupId = groupId,
                Group = group
            };

            _students.Add(student);
            group.Students.Add(student);

            return student;
        }

        public IReadOnlyList<Student> GetStudents()
        {
            return _students;
        }

        public Student GetStudentById(int id)
        {
            return _students.FirstOrDefault(s => s.Id == id);
        }
        public StudentCard AddStudentCard(
            int studentId,
            string cardNumber,
            DateTime issueDate)
        {
            Student student = GetStudentById(studentId);

            if (student == null)
                throw new ArgumentException("Студента не знайдено");

            if (student.Card != null)
                throw new InvalidOperationException(
                    "У студента вже є студентський квиток");

            if (string.IsNullOrWhiteSpace(cardNumber))
                throw new ArgumentException("Номер квитка не може бути порожнім");

            if (_cards.Any(c => c.CardNumber == cardNumber))
                throw new InvalidOperationException(
                    "Такий номер студентського квитка вже існує");

            var card = new StudentCard
            {
                Id = _cardId++,
                StudentId = studentId,
                CardNumber = cardNumber,
                IssueDate = issueDate
            };

            _cards.Add(card);
            student.Card = card;

            return card;
        }

        public Department AddDepartment(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Назва кафедри не може бути порожньою");

            var department = new Department
            {
                Id = _departmentId++,
                Name = name
            };

            _departments.Add(department);
            return department;
        }

        public IReadOnlyList<Department> GetDepartments()
        {
            return _departments;
        }

        public Teacher AddTeacher(string fullName, int departmentId)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Ім'я викладача не може бути порожнім");

            Department department =
                _departments.FirstOrDefault(d => d.Id == departmentId);

            if (department == null)
                throw new ArgumentException("Кафедру не знайдено");

            var teacher = new Teacher
            {
                Id = _teacherId++,
                FullName = fullName,
                DepartmentId = departmentId,
                Department = department
            };

            _teachers.Add(teacher);
            department.Teachers.Add(teacher);

            return teacher;
        }

        public IReadOnlyList<Teacher> GetTeachers()
        {
            return _teachers;
        }

        public Teacher GetTeacherById(int id)
        {
            return _teachers.FirstOrDefault(t => t.Id == id);
        }

        public Course AddCourse(string title, int credits, int teacherId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Назва курсу не може бути порожньою");

            if (credits <= 0)
                throw new ArgumentException("Кількість кредитів має бути більшою за 0");

            Teacher teacher = GetTeacherById(teacherId);

            if (teacher == null)
                throw new ArgumentException("Викладача не знайдено");

            var course = new Course
            {
                Id = _courseId++,
                Title = title,
                Credits = credits,
                TeacherId = teacherId,
                Teacher = teacher
            };

            _courses.Add(course);
            teacher.Courses.Add(course);

            return course;
        }

        public IReadOnlyList<Course> GetCourses()
        {
            return _courses;
        }
        public Enrollment EnrollStudent(
            int studentId,
            int courseId,
            int? grade = null)
        {
            Student student = GetStudentById(studentId);

            if (student == null)
                throw new ArgumentException("Студента не знайдено");

            Course course = _courses.FirstOrDefault(c => c.Id == courseId);

            if (course == null)
                throw new ArgumentException("Курс не знайдено");

            if (_enrollments.Any(e =>
                e.StudentId == studentId && e.CourseId == courseId))
            {
                throw new InvalidOperationException(
                    "Студент уже записаний на цей курс");
            }

            if (grade.HasValue && (grade.Value < 0 || grade.Value > 100))
                throw new ArgumentException("Оцінка повинна бути від 0 до 100");

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                Student = student,
                CourseId = courseId,
                Course = course,
                Grade = grade
            };

            _enrollments.Add(enrollment);
            student.Enrollments.Add(enrollment);
            course.Enrollments.Add(enrollment);

            return enrollment;
        }

        public IReadOnlyList<Enrollment> GetEnrollments()
        {
            return _enrollments;
        }

        public void SetGrade(int studentId, int courseId, int grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException(
                    "Оцінка повинна бути від 0 до 100");

            Enrollment enrollment = _enrollments.FirstOrDefault(e =>
                e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
                throw new ArgumentException(
                    "Студент не записаний на цей курс");

            if (enrollment.Status == EnrollmentStatus.Passed)
                throw new InvalidOperationException("Курс уже зараховано, змінювати оцінку не можна");

            enrollment.Grade = grade;
            enrollment.UpdateStatusAfterGrade();
        }
        public void RetakeCourse(int studentId, int courseId)
        {
            var enrollment = _enrollments.FirstOrDefault(e =>
                e.StudentId == studentId && e.CourseId == courseId)
                ?? throw new ArgumentException("Студент не записаний на цей курс");

            enrollment.StartRetake();
        }
    }
}
