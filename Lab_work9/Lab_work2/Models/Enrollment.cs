using System;

namespace UniversityApp.Models
{
    public class Enrollment
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int? Grade { get; set; }

        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;

        public void UpdateStatusAfterGrade()
        {
            Status = Grade >= 60 ? EnrollmentStatus.Passed : EnrollmentStatus.Failed;
        }

        public void StartRetake()
        {
            if (Status != EnrollmentStatus.Failed)
                throw new InvalidOperationException(
                    "Перездача можлива лише зі стану 'Не зараховано'");
            Status = EnrollmentStatus.Retake;
            Grade = null;
        }
    }
}