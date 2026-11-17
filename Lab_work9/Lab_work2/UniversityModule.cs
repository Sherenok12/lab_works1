using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityApp.Repository;
using UniversityApp.Services;


namespace UniversityApp
{
    public class UniversityModule
    {
        public UniversityRepository Repository { get; }
        public UniversityInfoService InfoService { get; }
        public AssessmentService AssessmentService { get; }

        public UniversityModule()
        {
            Repository = new UniversityRepository();

            InfoService = new UniversityInfoService(Repository);

            AssessmentService = new AssessmentService(Repository);
        }
    }
}
