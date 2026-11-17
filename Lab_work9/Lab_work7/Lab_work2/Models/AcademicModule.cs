using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::UniversityApp.Repository;
using UniversityApp.Services;

namespace Lab_work2.Models
{
    

    namespace UniversityApp
    {
        public class AcademicModule
        {
            public AcademicAnalyticsService AnalyticsService { get; }

            public AcademicModule(UniversityRepository repository)
            {
                // Новий модуль використовує спільний репозиторій для аналізу даних
                AnalyticsService = new AcademicAnalyticsService(repository);
            }
        }
    }
}
