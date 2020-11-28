using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OTS_API.Models
{
    /// <summary>
    /// 事件类型：
    ///     0: SystemMessage-> content: { title, content } !with relatedUser!
    ///     1: CourseAnnouncement-> content: { bulletinID, title, publisherID }
    ///     2: DiscussionCreated-> content: { disID, title, publisherID }
    ///     3: CoursewareUploaded-> content: { cwID, title, publisherID }
    ///     4: HomeworkCreated-> content: { hwID, title, publisherID }
    ///     5: HomeworkOpen-> content: { hwID, title }
    ///     6: HomeworkNearDDL-> content: { hwID, title }
    ///     7: HomeworkEnded-> content: { hwID, title }
    ///     8: HomeworkGraded-> content: { hwID, title, score } !with relatedUser!
    ///     9: ExamCreated-> content: { examID, title }
    ///     10: ExamOpen-> content: { examID, title }
    ///     11: ExamGraded-> content: { examID, title, score } !with relatedUser!
    /// </summary>
    public enum EventType
    {
        SystemMessage = 0,
        CourseAnnouncement = 1,
        DiscussionCreated = 2,
        CoursewareUploaded = 3,
        HomeworkCreated = 4,
        HomeworkOpen = 5,
        HomeworkNearDDL = 6,
        HomeworkEnded = 7,
        HomeworkGraded = 8,
        ExamCreated = 9,
        ExamOpen = 10,
        ExamGraded = 11
    }

    [Table("event")]
    public class Event
    {
        [Key]
        [Column("event_id")]
        public int EventID { get; set; }
        [Column("event_type")]
        public EventType EventType { get; set; }
        [Column("content")]
        public string Content { get; set; }
        [Column("course_id")]
        public int? CourseID { get; set; }
        [Column("course_name")]
        public string CourseName { get; set; }
        [Column("related_user")]
        public string RelatedUser { get; set; }
        [Column("time")]
        public DateTime Time { get; set; }
    }
}
