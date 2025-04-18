using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS.Controllers
{
    public class CommonController : Controller
    {
        private readonly LMSContext db;

        public CommonController(LMSContext _db)
        {
            db = _db;
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
        {
            var query =
                from d in db.Departments
                select new { name = d.Name, subject = d.Subject };

            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array representing the course catalog.
        /// Each object in the array should have the following fields:
        /// "subject": The subject abbreviation, (e.g. "CS")
        /// "dname": The department name, as in "Computer Science"
        /// "courses": An array of JSON objects representing the courses in the department.
        ///            Each field in this inner-array should have the following fields:
        ///            "number": The course number (e.g. 5530)
        ///            "cname": The course name (e.g. "Database Systems")
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetCatalog()
        {
            var query =
                from d in db.Departments
                join c in db.Courses on d.Subject equals c.Listing
                select new
                {
                    subject = d.Subject,
                    dname = d.Name,
                    cname = c.Name,
                    number = c.Num
                };

            var queryList = query.ToList(); // Force in-memory processing

            var result = queryList
                .GroupBy(val => new { val.subject, val.dname })
                .Select(group => new
                {
                    subject = group.Key.subject,
                    dname = group.Key.dname,
                    courses = group.Select(val => new
                    {
                        number = val.number,
                        cname = val.cname
                    }).ToList()
                })
                .ToList();

            return Json(result);
        }



        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject">The subject abbreviation, as in "CS"</param>
        /// <param name="number">The course number, as in 5530</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {
            var query =
                from course in db.Courses
                join c in db.Classes on course.CourseId equals c.CourseId
                join p in db.Professors on c.TeacherId equals p.UId
                where course.Listing == subject && course.Num == number
                select new { season = c.Season, year = c.SemesterYear, location = c.Location,
                start = c.StartTime, end = c.EndTime, fname = p.FName, lname = p.LName };
            return Json(query.ToArray());
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <returns>The assignment contents</returns>
        public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
        {
            var query =
                from a in db.Assignments
                join cat in db.AssignmentCategories on a.CatId equals cat.CatId
                join c in db.Classes on cat.ClassId equals c.ClassId
                join course in db.Courses on c.CourseId equals course.CourseId
                where course.Listing == subject && course.Num == num &&
                c.SemesterYear == year && c.Season == season && cat.Name == category &&
                a.Name == asgname
                select a.Contents;
            return Content(query.FirstOrDefault().ToString());
        }


        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment submission.
        /// Returns the empty string ("") if there is no submission.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <param name="uid">The uid of the student who submitted it</param>
        /// <returns>The submission text</returns>
        public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
        {
            var query =
                from a in db.Assignments
                join cat in db.AssignmentCategories on a.CatId equals cat.CatId
                join c in db.Classes on cat.ClassId equals c.ClassId
                join course in db.Courses on c.CourseId equals course.CourseId
                join sub in db.Submissions on a.AssignmentId equals sub.AssignmentId
                where course.Listing == subject && course.Num == num &&
                c.SemesterYear == year && c.Season == season && cat.Name == category &&
                a.Name == asgname && sub.UId == uid
                select sub.Contents;

            if(!query.Any()) {
                return Content("");
            }

            return Content(query.FirstOrDefault().ToString());
        }


        /// <summary>
        /// Gets information about a user as a single JSON object.
        /// The object should have the following fields:
        /// "fname": the user's first name
        /// "lname": the user's last name
        /// "uid": the user's uid
        /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
        ///               If the user is a Professor, this is the department they work in.
        ///               If the user is a Student, this is the department they major in.    
        ///               If the user is an Administrator, this field is not present in the returned JSON
        /// </summary>
        /// <param name="uid">The ID of the user</param>
        /// <returns>
        /// The user JSON object 
        /// or an object containing {success: false} if the user doesn't exist
        /// </returns>
        public IActionResult GetUser(string uid)
        {
            var profQuery = 
                from p in db.Professors
                join d in db.Departments on p.WorksIn equals d.Subject
                where p.UId == uid
                select new
                {
                    fname = p.FName,
                    lname = p.LName,
                    uid = p.UId,
                    department = d.Name
                };

            var profResult = profQuery.FirstOrDefault();

            if (profResult != null)
            {
                return Json(profResult);
            }

            var studentQuery = 
                from s in db.Students
                join d in db.Departments on s.Major equals d.Subject
                where s.UId == uid
                select new
                {
                    fname = s.FName,
                    lname = s.LName,
                    uid = s.UId,
                    department = d.Name
                };

            var studentResult = studentQuery.FirstOrDefault();

            if (studentResult != null)
            {
                return Json(studentResult);
            }

            var adminQuery = 
                from admin in db.Administrators
                where admin.UId == uid
                select new
                {
                    fname = admin.FName,
                    lname = admin.LName,
                    uid = admin.UId
                };

            var adminResult = adminQuery.FirstOrDefault();

            if (adminResult != null)
            {
                return Json(adminResult);
            }

            return Json(new { success = false });
        }



        /*******End code to modify********/
    }
}

