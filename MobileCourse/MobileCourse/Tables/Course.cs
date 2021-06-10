using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MobileCourse.Tables
{
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Notes { get; set; }
        public int TermID { get; set; }
        public bool Alert { get; set; }
        public string DateString { get; set; }

        public Course()
        {

        }

    }
}
