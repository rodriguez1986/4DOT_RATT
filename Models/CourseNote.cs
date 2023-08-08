namespace _4DOT_RATT.Models
{
    public class CourseNote
    {
        public int Id { get; set; }
        public decimal? Mark { get; set; }
        public int SubjectID { get; set; }
        //[ForeignKey("Id")]
        //public Subject? Subject { get; set; }
        public int StudentID { get; set; }
    }
}
