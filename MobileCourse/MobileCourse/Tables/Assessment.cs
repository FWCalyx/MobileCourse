using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MobileCourse.Tables
{
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool Alert { get; set; }
        public int CourseID { get; set; }

        public Assessment()
        {

        }
    }
}
