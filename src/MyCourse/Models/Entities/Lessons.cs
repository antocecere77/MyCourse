using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCourse.Models.Entities
{
    [Table("Lessons")]
    public partial class Lesson
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; } //00:00:00

        public virtual Course Course { get; set; }
    }
}
