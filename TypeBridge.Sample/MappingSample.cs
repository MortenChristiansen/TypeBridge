using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeBridge.Sample
{
    class MappingSample
    {
        private A _a;

        public A AProperty { get; set; }

        public void Sample()
        {
            //// Property assignment
            //var c1 = new C();
            //var a1 = new A();
            //c1.BValue = a1.Map();

            //// Variable assignment
            //var b2 = new B();
            //A a2 = b2.Map();

            //// Generic constructors
            //var b3 = new B();
            //var f3 = new F<A>(b3.Map());

            //// Property assignments
            //AProperty = b3.Map();

            //// Generic methods
            //var b4 = new B();
            //b4.DoStuff<A>(b2.Map());
            //b4.DoThing(b2.Map());

            //// Object initializers
            //var d5 = new Other.D
            //{
            //    AValue = b2.Map()
            //};

            //// Constructor arguments
            //var d6 = new C(b2.Map());

            //// Nested types, directly assignable
            //var c7 = new C(new A());
            //G g7 = c7.Map();
            //A a7 = new A();

            //// Mapping collections
            //var m1 = new M11();
            //M22 m2 = m1.Map();

            //// Map to base type
            //BaseA a8base = b3.Map();

            //// Map nested types - both direct match and one requiring a mapping
            //var g9 = new G();
            //C c9 = g9.Map();

            // Extension
            //var h10 = new H();
            //var k10 = new K();
            //A a10 = h10.Map().Extend(Fun());
            //J j10 = h10.Map();

            // TODO: Support destination type constructors with arguments
            // TODO: Support mapping property names?

            //var aList = new List<A>();
            //B[] bArray = aList.Map();
            //List<B> bList = aList.Map();

            //var m2 = new M2();
            //var m1 = new M1
            //{
            //    Items = m2.Items.Map()
            //};
        }

        private J Fun(List<B> blist)
        {
            return default;
        }

        private static List<ActivityCourseStudent> MapStudents(SaveActivityCourseCommand cmd) =>
            cmd.Students.Map();
    }

    public class H
    {
        public int Age { get; set; }
    }

    public class K
    {
        public string Name { get; set; }
    }

    public class J
    {
        public int Age { get; set; }
    }

    public class M1
    {
        public A[] Items { get; set; }
    }

    public class M2
    {
        public IEnumerable<B> Items { get; set; }
    }

    public class M11
    {
        public List<M1> Items { get; set; }
    }

    public class M22
    {
        public M2[] Items { get; set; }
    }

    public abstract class BaseA
    {
        public int Age { get; set; }
    }

    public class A : BaseA, IA
    {
        public string Name { get; set; }
    }

    public class BaseB
    {
        public string Name { get; set; }
    }

    public class B : BaseB
    {
        public int Age { get; set; }

        public void DoThing(A a)
        {

        }

        public void DoStuff<T>(A a, T x)
        {

        }

        public void DoStuff<T>(T x)
        {

        }
    }

    public class C
    {
        public A AValue { get; set; }
        public B BValue { get; set; }

        public C()
        {

        }

        public C(A a)
        {
            AValue = a;
        }

        public C(A a, B b)
        {
            AValue = a;
            BValue = b;
        }
    }

    public interface IA
    {
        int Age { get; set; }
        string Name { get; set; }
    }

    public class F<T>
    {
        public F(T t)
        {

        }

        public F(A a, T t)
        {

        }
    }

    public class G
    {
        public IA AValue { get; set; }
        public B BValue { get; set; }
    }




    public class SaveActivityCourseCommand
    {
        public Guid ActivityCourseId { get; set; }
        public string SchoolCode { get; set; }
        public Guid CourseAggregateId { get; set; }
        public string UvmDepartmentNumber { get; set; }
        public Guid DayCalendarId { get; set; }
        public Date StartDate { get; set; }
        public Date EndDate { get; set; }
        public Date[] SchoolDaysInPeriod { get; set; }
        public SaveActivityCourse_StudentDto[] Students { get; set; }
    }

    public class SaveActivityCourse_StudentDto
    {
        public Guid StudentId { get; set; }
        public string CivilRegistrationNumber { get; set; }
        public CourseStudentType CourseStudentType { get; set; }
        public Guid EducationId { get; set; }
        public Guid? SpecialisationId { get; set; }
        public bool EducationIsValid { get; set; }
        public SaveActivityCourse_ParticipationPeriodDto[] ParticipationPeriods { get; set; }
        public SaveActivityCourse_CountingPeriodDto[] AllCountingPeriodsForEducation { get; set; }
    }

    public class SaveActivityCourse_ParticipationPeriodDto
    {
        public Date From { get; set; }
        public Date To { get; set; }
        public Guid? TmkId { get; set; }
        public Guid? SchoolPeriodId { get; set; }
    }

    public class SaveActivityCourse_CountingPeriodDto
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public int CountingDay { get; set; }
        public decimal Duration { get; set; }
    }


    public struct Date
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }






    public class ActivityCourseSavedEvent
    {
        public ActivityCourseSavedEvent(
            Guid activityCourseId,
            string schoolCode,
            Guid courseAggregateId,
            bool isNew,
            Date startDate,
            Date endDate,
            string uvmDepartmentNumber,
            Guid dayCalendarId,
            Date[] schoolDaysInPeriod,
            ActivityCourseStudentDto[] students)
        {
            ActivityCourseId = activityCourseId;
            SchoolCode = schoolCode;
            CourseAggregateId = courseAggregateId;
            IsNew = isNew;
            StartDate = startDate;
            EndDate = endDate;
            UvmDepartmentNumber = uvmDepartmentNumber;
            DayCalendarId = dayCalendarId;
            SchoolDaysInPeriod = schoolDaysInPeriod;
            Students = students;
        }

        public Guid Id => ActivityCourseId;

        public Guid ActivityCourseId { get; set; }
        public string SchoolCode { get; set; }
        public Guid CourseAggregateId { get; set; }
        public bool IsNew { get; set; }
        public Date StartDate { get; set; }
        public Date EndDate { get; set; }
        public string UvmDepartmentNumber { get; set; }
        public Guid DayCalendarId { get; set; }
        public Date[] SchoolDaysInPeriod { get; set; }
        public ActivityCourseStudentDto[] Students { get; set; }

        public class ActivityCourseStudentDto
        {
            private ActivityCourseStudentDto() { }

            public ActivityCourseStudentDto(Guid activityCourseStudentId, string civilRegistrationNumber, CourseStudentType courseStudentType, Guid educationId, Guid? specialisationId, bool educationIsValid, ActivityParticipationPeriodDto[] participationPeriods, CountingPeriodDto[] allCountingPeriodsForEducation)
            {
                ActivityCourseStudentId = activityCourseStudentId;
                CivilRegistrationNumber = civilRegistrationNumber;
                CourseStudentType = courseStudentType;
                EducationId = educationId;
                SpecialisationId = specialisationId;
                EducationIsValid = educationIsValid;
                ParticipationPeriods = participationPeriods;
                AllCountingPeriodsForEducation = allCountingPeriodsForEducation;
            }

            public Guid ActivityCourseStudentId { get; set; }
            public string CivilRegistrationNumber { get; set; }
            public CourseStudentType CourseStudentType { get; set; }
            public Guid EducationId { get; set; }
            public Guid? SpecialisationId { get; set; }
            public bool EducationIsValid { get; set; }
            public ActivityParticipationPeriodDto[] ParticipationPeriods { get; set; }
            public CountingPeriodDto[] AllCountingPeriodsForEducation { get; set; }
        }

        public class ActivityParticipationPeriodDto
        {
            private ActivityParticipationPeriodDto() { }

            public ActivityParticipationPeriodDto(Date from, Date to, Guid? tmkId, Guid? schoolPeriodId)
            {
                From = from;
                To = to;
                TmkId = tmkId;
                SchoolPeriodId = schoolPeriodId;
            }

            public Date From { get; set; }
            public Date To { get; set; }
            public Guid? TmkId { get; set; }
            public Guid? SchoolPeriodId { get; set; }
        }

        public class CountingPeriodDto
        {
            private CountingPeriodDto() { }

            public CountingPeriodDto(Guid countingPeriodId, int number, int countingDay, decimal duration)
            {
                CountingPeriodId = countingPeriodId;
                Number = number;
                CountingDay = countingDay;
                Duration = duration;
            }

            public Guid CountingPeriodId { get; set; }
            public int Number { get; set; }
            public int CountingDay { get; set; }
            public decimal Duration { get; set; }
        }
    }

    public class BaseEntity
    {
        public Guid Id { get; set; }
    }

    public class ActivityCourseStudent : BaseEntity
    {
        public ActivityCourseStudent(Guid id, string civilRegistrationNumber, CourseStudentType courseStudentType, Guid educationId, Guid? specialisationId, bool educationIsValid, IEnumerable<ActivityParticipationPeriod> participationPeriods, IEnumerable<CountingPeriod> allCountingPeriodsForEducation)
        {
            Id = id;
            CourseStudentType = courseStudentType;
            EducationId = educationId;
            SpecialisationId = specialisationId;
            EducationIsValid = educationIsValid;
            ParticipationPeriods = participationPeriods?.ToList() ?? new List<ActivityParticipationPeriod>();
            AllCountingPeriodsForEducation = allCountingPeriodsForEducation?.ToList() ?? new List<CountingPeriod>();
            CivilRegistrationNumber = civilRegistrationNumber;
        }

        public string CivilRegistrationNumber { get; }
        public CourseStudentType CourseStudentType { get; }
        public Guid EducationId { get; }
        public Guid? SpecialisationId { get; }
        public bool EducationIsValid { get; }
        public IReadOnlyList<ActivityParticipationPeriod> ParticipationPeriods { get; }
        public IReadOnlyList<CountingPeriod> AllCountingPeriodsForEducation { get; }
    }

    public sealed class ActivityParticipationPeriod
    {
        public ActivityParticipationPeriod(Date from, Date to, Guid? tmkId, Guid? schoolPeriodId)
        {
            From = from;
            To = to;
            TmkId = tmkId;
            SchoolPeriodId = schoolPeriodId;
        }

        public Date From { get; }
        public Date To { get; }
        public Guid? TmkId { get; }
        public Guid? SchoolPeriodId { get; }

        public override bool Equals(object obj) =>
            obj is ActivityParticipationPeriod v && TmkId == v.TmkId && SchoolPeriodId == v.SchoolPeriodId;

        public override int GetHashCode() =>
            From.GetHashCode() ^ To.GetHashCode() ^ (TmkId?.GetHashCode() ?? 0) ^ (SchoolPeriodId?.GetHashCode() ?? 0);
    }

    public enum CourseStudentType
    {
        GG,
        AA
    }

    public sealed class CountingPeriod
    {
        public CountingPeriod(Guid countingPeriodId, int number, int countingDay, decimal duration)
        {
            Number = number;
            CountingDay = countingDay;
            Duration = duration;
            CountingPeriodId = countingPeriodId;
        }

        public Guid CountingPeriodId { get; }
        public int Number { get; }
        public int CountingDay { get; }
        public decimal Duration { get; }

        public override bool Equals(object obj) =>
            obj is CountingPeriod v && CountingPeriodId == v.CountingPeriodId && Number == v.Number && CountingDay == v.CountingDay && Duration == v.Duration;

        public override int GetHashCode() =>
            CountingPeriodId.GetHashCode() ^ Number.GetHashCode() ^ CountingDay.GetHashCode() ^ Duration.GetHashCode();
    }
}

namespace TypeBridge.Other
{
    public class D
    {
        public Sample.A AValue { get; set; }
        public Sample.B BValue { get; set; }

        public D()
        {

        }

        public D(Sample.A a)
        {
            AValue = a;
        }
    }
}
