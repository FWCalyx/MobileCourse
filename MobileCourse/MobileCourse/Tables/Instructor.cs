using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MobileCourse.Tables
{
    public class Instructor
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int CourseID { get; set; }
        public Instructor()
        {

        }
    }
}
