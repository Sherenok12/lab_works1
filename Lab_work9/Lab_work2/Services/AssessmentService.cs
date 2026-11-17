using System;
using System.Linq;
using UniversityApp.Models;
using UniversityApp.Repository;

namespace UniversityApp.Services
{
    /// <summary>
    /// Модуль «Облік успішності», побудований за UML-діаграмами послідовності
    /// (diagrams/01_enroll_student, 02_set_grade, 03_retake).
    ///
    /// Ланцюжок викликів зі схем:
    ///   Користувач -> Program -> AssessmentService -> UniversityRepository -> Enrollment
    ///
    /// Кожен метод відповідає одному сценарію діаграми, а номери кроків
    /// у коментарях збігаються з номерами повідомлень на діаграмі.
    /// Якщо передати делегат log, сервіс виводить трасування виконання
    /// у вигляді тих самих повідомлень, що намальовані на діаграмі.
    /// </summary>
    public class AssessmentService
    {
        private readonly UniversityRepository _repository;
        private readonly Action<string> _log;

        public AssessmentService(UniversityRepository repository, Action<string>? log = null)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _log = log ?? (_ => { });
        }

        // ---------------------------------------------------------------
        // Діаграма 01. Запис студента на курс
        // ---------------------------------------------------------------
        public Enrollment EnrollStudent(int studentId, int courseId)
        {
            _log($"[EnrollStudent] Program -> AssessmentService: EnrollStudent(studentId={studentId}, courseId={courseId})");

            // 1. AssessmentService -> UniversityRepository: EnrollStudent(...)
            _log("   1. AssessmentService -> UniversityRepository: EnrollStudent(studentId, courseId)");
            Enrollment enrollment = Execute(() => _repository.EnrollStudent(studentId, courseId));

            // 2. UniversityRepository -> Enrollment: new Enrollment(...), Status = Enrolled
            _log($"   2. UniversityRepository -> Enrollment: створено запис, Status = {enrollment.Status}");

            // 3. UniversityRepository --> AssessmentService --> Program: Enrollment
            _log("   3. AssessmentService --> Program: Enrollment");
            return enrollment;
        }

        // ---------------------------------------------------------------
        // Діаграма 02. Виставлення оцінки
        // ---------------------------------------------------------------
        public Enrollment AssignGrade(int studentId, int courseId, int grade)
        {
            _log($"[AssignGrade] Program -> AssessmentService: AssignGrade(studentId={studentId}, courseId={courseId}, grade={grade})");

            // 1. AssessmentService -> UniversityRepository: SetGrade(...)
            //    Усередині репозиторію (гілки alt на діаграмі):
            //      - grade < 0 або grade > 100      -> ArgumentException
            //      - запис не знайдено              -> ArgumentException
            //      - Status == Passed               -> InvalidOperationException
            //      - інакше: Grade = grade; Enrollment.UpdateStatusAfterGrade()
            _log("   1. AssessmentService -> UniversityRepository: SetGrade(studentId, courseId, grade)");
            Execute(() => { _repository.SetGrade(studentId, courseId, grade); return true; });

            Enrollment enrollment = GetEnrollment(studentId, courseId);

            // 2. UniversityRepository -> Enrollment: UpdateStatusAfterGrade()
            //    alt: Grade >= 60 -> Passed, Grade < 60 -> Failed
            _log($"   2. UniversityRepository -> Enrollment: UpdateStatusAfterGrade() => Status = {enrollment.Status}");

            // 3. AssessmentService --> Program: Enrollment (з оновленим статусом)
            _log("   3. AssessmentService --> Program: Enrollment");
            return enrollment;
        }

        // ---------------------------------------------------------------
        // Діаграма 03. Перездача курсу
        // ---------------------------------------------------------------
        public Enrollment StartRetake(int studentId, int courseId)
        {
            _log($"[StartRetake] Program -> AssessmentService: StartRetake(studentId={studentId}, courseId={courseId})");

            // 1. AssessmentService -> UniversityRepository: RetakeCourse(...)
            //    alt: запис не знайдено -> ArgumentException
            //    Enrollment.StartRetake():
            //      alt: Status != Failed -> InvalidOperationException
            //           Status == Failed -> Status = Retake; Grade = null
            _log("   1. AssessmentService -> UniversityRepository: RetakeCourse(studentId, courseId)");
            Execute(() => { _repository.RetakeCourse(studentId, courseId); return true; });

            Enrollment enrollment = GetEnrollment(studentId, courseId);

            // 2. UniversityRepository -> Enrollment: StartRetake()
            _log($"   2. UniversityRepository -> Enrollment: StartRetake() => Status = {enrollment.Status}, Grade = {(enrollment.Grade?.ToString() ?? "null")}");

            // 3. AssessmentService --> Program: Enrollment (Status = Retake)
            _log("   3. AssessmentService --> Program: Enrollment");
            return enrollment;
        }

        // ---------------------------------------------------------------
        // Допоміжні методи
        // ---------------------------------------------------------------

        /// <summary>
        /// Знаходить запис студента на курс (використовується після успішної операції).
        /// </summary>
        private Enrollment GetEnrollment(int studentId, int courseId)
        {
            Enrollment? enrollment = _repository.GetEnrollments()
                .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

            return enrollment
                ?? throw new InvalidOperationException("Запис студента на курс не знайдено після виконання операції");
        }

        /// <summary>
        /// Виконує виклик репозиторію. Виняток не гаситься: він логується
        /// і піднімається вгору до Program, де його перехоплює catch
        /// (на діаграмі це повідомлення-виняток у гілках alt).
        /// </summary>
        private T Execute<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                _log($"   ! {ex.GetType().Name}: {ex.Message}");
                throw;
            }
        }
    }
}
