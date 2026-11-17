using System.Collections.Generic;
using System.Linq;
using UniversityApp.Models;
using UniversityApp.Repository;

namespace UniversityApp.Services
{
    public class AcademicAnalyticsService
    {
        private readonly UniversityRepository _repository;

        public AcademicAnalyticsService(UniversityRepository repository)
        {
            _repository = repository;
        }

        public int GetStudentEarnedCredits(int studentId)
        {
            var studentEnrollments = _repository.GetEnrollments()
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Passed); 

            int totalCredits = 0;
            foreach (var enrollment in studentEnrollments)
            {
                var course = enrollment.Course; 
                if (course != null)
                {
                    totalCredits += course.Credits;
                }
            }

            return totalCredits;
        }

        public IEnumerable<Student> GetActiveStudents()
        {
            var activeStudentIds = _repository.GetEnrollments()
                .Where(e => e.Status == EnrollmentStatus.Enrolled)
                .Select(e => e.StudentId)
                .Distinct();

            return _repository.GetStudents().Where(s => activeStudentIds.Contains(s.Id));
        }
    }
}