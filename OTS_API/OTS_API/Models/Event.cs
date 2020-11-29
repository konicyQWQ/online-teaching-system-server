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
    ///     0: SystemMessage-> content: { title, content } !with relatedUser!x
    ///     1: CourseAnnouncement-> content: { id, title } !with relatedUser!x
    ///     2: DiscussionCreated-> content: { id, title } !with relatedUser!x
    ///     3: CoursewareUploaded-> content: { id, title } !with relatedUser!x
    ///     4: HomeworkCreated-> content: { id, title } !with relatedUser!x
    ///     5: HomeworkOpen-> content: { id, title }x
    ///     6: HomeworkNearDDL-> content: { id, title }x
    ///     7: ExamCreated-> content: { id, title } !with relatedUser!x
    ///     8: ExamOpen-> content: { id, title }
    ///     9: HomeworkGraded-> content: { id, title, score } !with relatedUser!x
    ///     10: ExamGraded-> content: { id, title, score } !with relatedUser!
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
        ExamCreated = 7,
        ExamOpen = 8,
        HomeworkGraded = 9,
        ExamGraded = 10
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

    public class EventResList
    {
        public int TotalCount { get; set; }
        public List<Event> EventList { get; set; }
    }

    public class ExamGradedEventContent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Score { get; set; }
    }

    public class HWGradedEventContent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Score { get; set; }
    }
}
