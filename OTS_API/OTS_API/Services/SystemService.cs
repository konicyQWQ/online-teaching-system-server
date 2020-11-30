using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OTS_API.DatabaseContext;
using OTS_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTS_API.Services
{
    public class SystemService
    {
        private readonly ILogger<SystemService> logger;
        private readonly OTSDbContext dbContext;

        public SystemService(ILogger<SystemService> logger, OTSDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        public async Task<EventResList> GetRelatedEventListAsync(string userID, int start, int limit)
        {
            try
            {
                var courseList = await dbContext.UserCourse.Where(ue => ue.UserId == userID).Select(ue => ue.CourseId).ToListAsync();
                var eList = await dbContext.Events.Where(e => e.EventType == EventType.SystemMessage || ((int)e.EventType >= 1 && (int)e.EventType <= 8 && courseList.Contains(e.CourseID.Value)) || ((int)e.EventType >= 9 && e.RelatedUser == userID)).ToListAsync();
                eList.Sort((a, b) => b.Time.CompareTo(a.Time));
                var totalCount = eList.Count;
                if (limit > totalCount - start)
                {
                    limit = totalCount - start;
                }
                eList = eList.GetRange(start, limit);
                return new EventResList()
                {
                    TotalCount = totalCount,
                    EventList = eList
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task AddSystemMessageAsync(string title, string content, string userID)
        {
            try
            {
                var e = new Event()
                {
                    EventType = EventType.SystemMessage,
                    RelatedUser = userID,
                    Time = DateTime.Now,
                    Content = JsonConvert.SerializeObject(new { Title = title, Content = content })
                };
                await dbContext.Events.AddAsync(e);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task UpdateSystemMessageAsync(int id, string title, string content, string userID)
        {
            try
            {
                var eToUpdate = await dbContext.Events.FindAsync(id);
                if (eToUpdate == null)
                {
                    throw new Exception("Event Not Exist!");
                }
                eToUpdate.Content = JsonConvert.SerializeObject(new { Title = title, Content = content });
                eToUpdate.RelatedUser = userID;
                eToUpdate.Time = DateTime.Now;
                dbContext.Events.Update(eToUpdate);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task RemoveSystemMessageAsync(int id)
        {
            try
            {
                var eToRemove = await dbContext.Events.FindAsync(id);
                if (eToRemove == null)
                {
                    throw new Exception("Event Not Found!");
                }
                dbContext.Events.Remove(eToRemove);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<EventResList> GetSystemMessageListAsync(int start, int limit)
        {
            try
            {
                var list = await dbContext.Events.Where(e => e.EventType == EventType.SystemMessage).ToListAsync();
                list.Sort((a, b) => b.Time.CompareTo(a.Time));
                var totalCount = list.Count;
                if (limit > totalCount - start)
                {
                    limit = totalCount - start;
                }
                list = list.GetRange(start, limit);
                return new EventResList()
                {
                    TotalCount = totalCount,
                    EventList = list
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task AddCourseAnnonceEventAsync(int id, string title, string publisher, int courseID)
        {
            try
            {
                var e = new Event()
                {
                    EventType = EventType.CourseAnnouncement,
                    Content = JsonConvert.SerializeObject(new { Id = id, Title = title }),
                    CourseID = courseID,
                    CourseName = await this.GetCourseNameAsync(courseID),
                    RelatedUser = publisher,
                    Time = DateTime.Now
                };
                await dbContext.Events.AddAsync(e);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task AddDiscussionCreatedEventAsync(int id, string title, string publisher, int courseID)
        {
            try
            {
                var e = new Event()
                {
                    EventType = EventType.DiscussionCreated,
                    Content = JsonConvert.SerializeObject(new { Id = id, Title = title }),
                    CourseID = courseID,
                    CourseName = await this.GetCourseNameAsync(courseID),
                    RelatedUser = publisher,
                    Time = DateTime.Now
                };
                await dbContext.Events.AddAsync(e);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task AddCWUploadedEventAsync(int id, string title, string publisher, int courseID)
        {
            try
            {
                var e = new Event()
                {
                    EventType = EventType.CoursewareUploaded,
                    Content = JsonConvert.SerializeObject(new { Id = id, Title = title }),
                    CourseID = courseID,
                    CourseName = await this.GetCourseNameAsync(courseID),
                    RelatedUser = publisher,
                    Time = DateTime.Now
                };
                await dbContext.Events.AddAsync(e);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task AddHWCreatedEventAsync(int id, string title, string publisher, int courseID)
        {
            try
            {
                var e = new Event()
                {
                    EventType = EventType.HomeworkCreated,
                    Content = JsonConvert.SerializeObject(new { Id = id, Title = title }),
                    CourseID = courseID,
                    CourseName = await this.GetCourseNameAsync(courseID),
                    RelatedUser = publisher,
                    Time = DateTime.Now
                };
                await dbContext.Events.AddAsync(e);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task AddExamCreatedEventAsync(int id, string title, string publisher, int courseID)
        {
            try
            {
                var e = new Event()
                {
                    EventType = EventType.ExamCreated,
                    Content = JsonConvert.SerializeObject(new { Id = id, Title = title }),
                    CourseID = courseID,
                    CourseName = await this.GetCourseNameAsync(courseID),
                    RelatedUser = publisher,
                    Time = DateTime.Now
                };
                await dbContext.Events.AddAsync(e);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<string> GetCourseNameAsync(int courseID)
        {
            try
            {
                var course = await dbContext.Courses.FindAsync(courseID);
                if(course == null)
                {
                    throw new Exception("Uable to Find Course ID: " + courseID);
                }
                return course.Name;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task SetHomePagesAsync(List<HomePage> pages)
        {
            try
            {
                dbContext.HomePages.RemoveRange(dbContext.HomePages);
                await dbContext.SaveChangesAsync();
                await dbContext.HomePages.AddRangeAsync(pages);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<HomePage>> GetHomePagesAsync()
        {
            try
            {
                var list = await dbContext.HomePages.ToListAsync();
                list.Sort((a, b) => a.Id.CompareTo(b.Id));
                return list;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }
    }
}
