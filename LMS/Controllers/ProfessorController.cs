using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {

        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var query =
                from e in db.Enrolleds
                join s in db.Students on e.UId equals s.UId
                join c in db.Classes on e.ClassId equals c.ClassId
                join courses in db.Courses on c.CourseId equals courses.CourseId
                where c.Season == season && c.SemesterYear == year && courses.Listing == subject
                && courses.Num == num
                select new { fname = s.FName, lname = s.LName, uid = s.UId, dob = s.Dob, grade = e.Grade };
                
            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            if (category != null)
            {
                var query =
                    from a in db.Assignments
                    join cat in db.AssignmentCategories on a.CatId equals cat.CatId
                    join classes in db.Classes on cat.ClassId equals classes.ClassId
                    join courses in db.Courses on classes.CourseId equals courses.CourseId
                    where courses.Listing == subject && courses.Num == num
                    && classes.Season == season && classes.SemesterYear == year
                    && cat.Name == category
                    select new { aname = a.Name, cname = cat.Name, due = a.Due };
                return Json(query.ToArray());
            }
            else
            {
                var query =
                    from a in db.Assignments
                    join cat in db.AssignmentCategories on a.CatId equals cat.CatId
                    join classes in db.Classes on cat.ClassId equals classes.ClassId
                    join courses in db.Courses on classes.CourseId equals courses.CourseId
                    where courses.Listing == subject && courses.Num == num
                    && classes.Season == season && classes.SemesterYear == year
                    select new { aname = a.Name, cname = cat.Name, due = a.Due };
                return Json(query.ToArray());
            }
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var query =
                from cat in db.AssignmentCategories
                join classes in db.Classes on cat.ClassId equals classes.ClassId
                join courses in db.Courses on classes.CourseId equals courses.CourseId
                where courses.Listing == subject && courses.Num == num &&
                classes.Season == season && classes.SemesterYear == year
                select new { name = cat.Name, weight = cat.Weight };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            var query =
                from cat in db.AssignmentCategories
                join classes in db.Classes on cat.ClassId equals classes.ClassId
                join courses in db.Courses on classes.CourseId equals courses.CourseId
                where courses.Listing == subject && courses.Num == num &&
                classes.Season == season && classes.SemesterYear == year 
                select new {catName = cat.Name, classId = classes.ClassId};

            if (query.Any(x => x.catName == category))
            {
                return Json(new { success = false });
            }

            var query2 =
                from classes in db.Classes
                join courses in db.Courses on classes.CourseId equals courses.CourseId
                where courses.Listing == subject && courses.Num == num &&
                classes.Season == season && classes.SemesterYear == year
                select classes;

            AssignmentCategory ac = new AssignmentCategory();
            ac.Name = category;
            ac.Weight = (uint)catweight;
            ac.ClassId = query2.FirstOrDefault().ClassId;
            db.AssignmentCategories.Add(ac);
            db.SaveChanges();

            return Json(new { success = true });
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {

            var query =
                from cat in db.AssignmentCategories
                join classes in db.Classes on cat.ClassId equals classes.ClassId
                join courses in db.Courses on classes.CourseId equals courses.CourseId
                where courses.Listing == subject && courses.Num == num &&
                classes.Season == season && classes.SemesterYear == year &&
                cat.Name == category
                select new { catId = cat.CatId };

            if (!query.Any())
            {
                return Json(new { success = false });
            }

            Assignment a = new Assignment();
            a.Name = asgname;
            a.Points = (uint)asgpoints;
            a.Due = asgdue;
            a.Contents = asgcontents;
            a.CatId = query.FirstOrDefault().catId;
            db.Assignments.Add(a);
            db.SaveChanges();

            var enrolledStudents = (from course in db.Courses
                                join cls in db.Classes on course.CourseId equals cls.CourseId
                                join e in db.Enrolleds on cls.ClassId equals e.ClassId
                                where course.Listing == subject
                                      && course.Num == num
                                      && cls.Season == season
                                      && cls.SemesterYear == year
                                select e.UId).ToList();

            // Update grade for each student
            foreach (var uid in enrolledStudents)
                {
                UpdateStudentGrade(uid, subject, num, season, year);
                }

            return Json(new { success = true });
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var query =
                from a in db.Assignments
                join cat in db.AssignmentCategories on a.CatId equals cat.CatId
                join classes in db.Classes on cat.ClassId equals classes.ClassId
                join courses in db.Courses on classes.CourseId equals courses.CourseId
                join sub in db.Submissions on a.AssignmentId equals sub.AssignmentId
                join s in db.Students on sub.UId equals s.UId
                where courses.Listing == subject && courses.Num == num &&
                classes.Season == season && classes.SemesterYear == year &&
                cat.Name == category && a.Name == asgname
                select new { fname = s.FName, lname = s.LName, uid = s.UId, time = sub.Time, score = sub.Score };


            return Json(query.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            var query =
                from a in db.Assignments
                join cat in db.AssignmentCategories on a.CatId equals cat.CatId
                join classes in db.Classes on cat.ClassId equals classes.ClassId
                join courses in db.Courses on classes.CourseId equals courses.CourseId
                join sub in db.Submissions on a.AssignmentId equals sub.AssignmentId
                join s in db.Students on sub.UId equals s.UId
                where courses.Listing == subject && courses.Num == num &&
                classes.Season == season && classes.SemesterYear == year &&
                cat.Name == category && a.Name == asgname && sub.UId == uid
                select sub;

            if(query == null)
            {
                return Json(new { success = false });
            }

            query.FirstOrDefault().Score = (uint?)score;
            db.SaveChanges();

            UpdateStudentGrade(uid, subject, num, season, year);

            return Json(new { success = true });
        }
        private void UpdateStudentGrade(string uid, string subject, int num, string season, int year)
        {
            var theClass = (from c in db.Classes
                            join course in db.Courses on c.CourseId equals course.CourseId
                            where course.Listing == subject && course.Num == num &&
                                  c.Season == season && c.SemesterYear == year
                            select c).FirstOrDefault();

            if (theClass == null)
                return;

            int classId = theClass.ClassId;

            var categories = db.AssignmentCategories
                               .Where(cat => cat.ClassId == classId)
                               .ToList();

            double totalWeightedScore = 0.0;
            double totalWeightUsed = 0.0;

            foreach (var cat in categories)
            {
            var assignments = db.Assignments
                                .Where(a => a.CatId == cat.CatId)
                                .ToList();

            if (assignments.Count == 0)
                continue; 

            double totalEarned = 0.0;
            double totalPossible = 0.0;

            foreach (var a in assignments)
            {
            var submission = db.Submissions
                               .Where(s => s.AssignmentId == a.AssignmentId && s.UId == uid)
                               .FirstOrDefault();

            uint earned = (submission != null && submission.Score != null) ? submission.Score.Value : 0;
            totalEarned += earned;
            totalPossible += a.Points;
            }

            if (totalPossible == 0)
                continue; 

            double percentage = totalEarned / totalPossible;
            totalWeightedScore += percentage * cat.Weight;
            totalWeightUsed += cat.Weight;
            }

            double scaledScore = (totalWeightUsed == 0) ? 0.0 : (totalWeightedScore * (100.0 / totalWeightUsed));

            string letterGrade = GetLetterGrade(scaledScore);

            var enrollment = db.Enrolleds
                               .Where(e => e.UId == uid && e.ClassId == classId)
                               .FirstOrDefault();

            if (enrollment != null)
            {
            enrollment.Grade = letterGrade;
            db.SaveChanges();
        }
        }
        private string GetLetterGrade(double score)
        {
            if (score >= 93) return "A";
            if (score >= 90) return "A-";
            if (score >= 87) return "B+";
            if (score >= 83) return "B";
            if (score >= 80) return "B-";
            if (score >= 77) return "C+";
            if (score >= 73) return "C";
            if (score >= 70) return "C-";
            if (score >= 67) return "D+";
            if (score >= 63) return "D";
            if (score >= 60) return "D-";
            return "--";
        }

        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query =
                from p in db.Professors
                join classes in db.Classes on p.UId equals classes.TeacherId
                join courses in db.Courses on classes.CourseId equals courses.CourseId
                where p.UId == uid
                select new
                {
                    subject = courses.Listing,
                    number = courses.Num,
                    name = courses.Name,
                    season = classes.Season,
                    year = classes.SemesterYear
                };

            return Json(query.ToArray());
        }


        
        /*******End code to modify********/
    }
}

