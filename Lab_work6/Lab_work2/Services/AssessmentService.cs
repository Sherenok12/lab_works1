using System;
using System.Linq;
using UniversityApp.Models;
using UniversityApp.Repository;

namespace UniversityApp.Services
{
   
    public class AssessmentService
    {
        private readonly UniversityRepository _repository;
        private readonly Action<string> _log;

        public AssessmentService(UniversityRepository repository, Action<string>? log = null)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _log = log ?? (_ => { });
        }

        public Enrollment EnrollStudent(int studentId, int courseId)
        {
            _log($"[EnrollStudent] Program -> AssessmentService: EnrollStudent(studentId={studentId}, courseId={courseId})");
            _log("   1. AssessmentService -> UniversityRepository: EnrollStudent(studentId, courseId)");
            Enrollment enrollment = Execute(() => _repository.EnrollStudent(studentId, courseId));

            _log($"   2. UniversityRepository -> Enrollment: створено запис, Status = {enrollment.Status}");
            _log("   3. AssessmentService --> Program: Enrollment");
            return enrollment;
        }

        public Enrollment AssignGrade(int studentId, int courseId, int grade)
        {
            _log($"[AssignGrade] Program -> AssessmentService: AssignGrade(studentId={studentId}, courseId={courseId}, grade={grade})");
            _log("   1. AssessmentService -> UniversityRepository: SetGrade(studentId, courseId, grade)");
            Execute(() => { _repository.SetGrade(studentId, courseId, grade); return true; });

            Enrollment enrollment = GetEnrollment(studentId, courseId);
            _log($"   2. UniversityRepository -> Enrollment: UpdateStatusAfterGrade() => Status = {enrollment.Status}");

            _log("   3. AssessmentService --> Program: Enrollment");
            return enrollment;
        }

        public Enrollment StartRetake(int studentId, int courseId)
        {
            _log($"[StartRetake] Program -> AssessmentService: StartRetake(studentId={studentId}, courseId={courseId})");
            _log("   1. AssessmentService -> UniversityRepository: RetakeCourse(studentId, courseId)");
            Execute(() => { _repository.RetakeCourse(studentId, courseId); return true; });
            Enrollment enrollment = GetEnrollment(studentId, courseId);
            _log($"   2. UniversityRepository -> Enrollment: StartRetake() => Status = {enrollment.Status}, Grade = {(enrollment.Grade?.ToString() ?? "null")}");
            _log("   3. AssessmentService --> Program: Enrollment");
            return enrollment;
        }


        private Enrollment GetEnrollment(int studentId, int courseId)
        {
            Enrollment? enrollment = _repository.GetEnrollments()
                .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

            return enrollment
                ?? throw new InvalidOperationException("Запис студента на курс не знайдено після виконання операції");
        }


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
