using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTS_API.Models;
using OTS_API.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.IO;
using OTS_API.Utilities;

namespace OTS_API.Services
{
    public class CourseService
    {
        private readonly ILogger<CourseService> logger;
        private readonly OTSDbContext dbContext;

        public CourseService(ILogger<CourseService> logger, OTSDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        /// <summary>
        /// 获取课程信息
        /// </summary>
        /// <param name="id">课程代码</param>
        /// <returns>课程信息</returns>
        public async Task<Course> GetCourseAsync(int id)
        {
            try
            {
                var course = await dbContext.Courses.FindAsync(id);
                return course;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<CourseResList> GetCoursesAsync(int start, int limit)
        {
            try
            {
                var courseList = await dbContext.Courses.ToListAsync();
                var totalCount = courseList.Count;
                if(limit > totalCount - start)
                {
                    limit = totalCount - start;
                }
                courseList = courseList.GetRange(start, limit);
                var resList = new List<CourseWithTeachers>();
                foreach(var course in courseList)
                {
                    var teacherList = await this.GetCourseTeachersAsync(course.Id);
                    var res = new CourseWithTeachers()
                    {
                        Course = course,
                        Teachers = teacherList
                    };
                    resList.Add(res);
                }
                return new CourseResList()
                {
                    TotalCount = totalCount,
                    ResList = resList
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable to Get Course Info");
            }
        }

        public async Task<CourseResList> GetCoursesAsync(string keyword, int start, int limit)
        {
            try
            {
                var courseList = await dbContext.Courses.Where(c => c.Name.Contains(keyword)).ToListAsync();
                var totalCount = courseList.Count;
                if (limit > totalCount - start)
                {
                    limit = totalCount - start;
                }
                courseList = courseList.GetRange(start, limit);
                var resList = new List<CourseWithTeachers>();
                foreach (var course in courseList)
                {
                    var teacherList = await this.GetCourseTeachersAsync(course.Id);
                    var res = new CourseWithTeachers()
                    {
                        Course = course,
                        Teachers = teacherList
                    };
                    resList.Add(res);
                }
                return new CourseResList()
                {
                    TotalCount = totalCount,
                    ResList = resList
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable to Get Course Info");
            }
        }

        public async Task<List<User>> GetCourseTeachersAsync(int courseID)
        {
            try
            {
                var idList = await dbContext.UserCourse.Where(uc => uc.UserRole == UserRole.Teacher && uc.CourseId == courseID).ToListAsync();
                var teacherList = new List<User>();
                foreach(var id in idList)
                {
                    var t = await dbContext.Users.FindAsync(id.UserId);
                    t.Introduction = null;
                    t.Password = null;
                    teacherList.Add(t);
                }
                return teacherList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable to Get Teacher Info!");
            }
        }

        public async Task RemoveCourseTeachersAsync(int courseID)
        {
            try
            {
                dbContext.UserCourse.RemoveRange(await dbContext.UserCourse.Where(uc => uc.CourseId == courseID).ToArrayAsync());
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task UpdateCourseAysnc(Course course)
        {
            try
            {
                dbContext.Courses.Update(course);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable to Update Course Info!");
            }
        }

        public Task<bool> DeleteCourseAsync(string id)
        {
            return Task.Run(() =>
            {
                return true;
            });
        }

        public async Task<int> AddCourseAysnc(Course course)
        {
            try
            {
                await dbContext.Courses.AddAsync(course);
                await dbContext.SaveChangesAsync();
                var courseFileRoot = Config.privateFilePath + "Course" + course.Id;
                Directory.CreateDirectory(courseFileRoot);
                Directory.CreateDirectory(courseFileRoot + "/Courseware");
                Directory.CreateDirectory(courseFileRoot + "/Homework");
                return course.Id;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task AddTeacherToCouseAsync(int courseId, string teacherId)
        {
            try
            {
                var uc = new UserCourse()
                {
                    CourseId = courseId,
                    UserId = teacherId,
                    UserRole = UserRole.Teacher
                };
                await dbContext.UserCourse.AddAsync(uc);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Add Teacher(id: " + teacherId + ") Failed!");
            }
        }

        public async Task<UserCourse> GetUserCourseAsync(string userID, int courseID)
        {
            try
            {
                var res = await dbContext.UserCourse.FindAsync(userID, courseID);
                return res;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed");
            }
        }

        public async Task AddBulletinAsync(Bulletin bulletin)
        {
            try
            {
                await dbContext.Bulletins.AddAsync(bulletin);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task UpdateBulletinAsync(Bulletin bulletin)
        {
            try
            {
                dbContext.Bulletins.Update(bulletin);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task DeleteBulletinAsync(int bulletinID)
        {
            try
            {
                var bulletinToDelete = await dbContext.Bulletins.FindAsync(bulletinID);
                if(bulletinToDelete == null)
                {
                    throw new Exception("Cannot Find Bulletin(id: " + bulletinID + ")!");
                }
                dbContext.Bulletins.Remove(bulletinToDelete);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<Bulletin> GetBulletinAsync(int bulletinID)
        {
            try
            {
                var res = await dbContext.Bulletins.FindAsync(bulletinID);
                if (res == null)
                {
                    throw new Exception("Cannot Find Bulletin(id: " + bulletinID + ")!");
                }
                return res;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<Bulletin>> GetCourseBulletinsAsync(int courseID)
        {
            try
            {
                var list = await dbContext.Bulletins.Where(b => b.CourseId == courseID).ToListAsync();
                return list;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task AddCoursewareAsync(Courseware courseware)
        {
            try
            {
                await dbContext.Coursewares.AddAsync(courseware);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<Courseware> GetCoursewareAsync(int id)
        {
            try
            {
                var res = await dbContext.Coursewares.FindAsync(id);
                if(res == null)
                {
                    throw new Exception("Unable to Find Courseware(id: " + id + ")!");
                }
                return res;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task UpdateCousewareAsync(Courseware courseware)
        {
            try
            {
                dbContext.Coursewares.Update(courseware);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task DeleteCoursewareAsync(int id)
        {
            try
            {
                var cwToDelete = await dbContext.Coursewares.FindAsync(id);
                if(cwToDelete == null)
                {
                    throw new Exception("Unable to Find Courseware(id: " + id + ")!");
                }
                dbContext.Coursewares.Remove(cwToDelete);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task AddFileToCourseware(int coursewareID, int fileID)
        {
            try
            {
                var coursewareFile = new CoursewareFile()
                {
                    CoursewareId = coursewareID,
                    FileId = fileID
                };
                await dbContext.CoursewareFile.AddAsync(coursewareFile);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<List<Models.File>> GetCoursewareFilesAsync(int coursewareID)
        {
            try
            {
                var cfList = await dbContext.CoursewareFile.Where(cf => cf.CoursewareId == coursewareID).ToListAsync();
                var fileList = new List<Models.File>();
                foreach(var cf in cfList)
                {
                    var fileInfo = await dbContext.Files.FindAsync(cf.FileId);
                    fileList.Add(fileInfo);
                }
                return fileList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable to Get Courseware FileList!");
            }
        }

        public async Task RemoveCoursewareFilesAsync(int coursewareID)
        {
            try
            {
                var arrayToRemove = await dbContext.CoursewareFile.Where(cf => cf.CoursewareId == coursewareID).ToArrayAsync();
                dbContext.CoursewareFile.RemoveRange(arrayToRemove);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable Reset Courseware FileList!");
            }
        }

        public async Task<List<CoursewareWithFiles>> GetCoursewareWithFilesAsync(int courseID)
        {
            try
            {
                var coursewareList = await dbContext.Coursewares.Where(c => c.CourseId == courseID).ToListAsync();
                var cwfList = new List<CoursewareWithFiles>();
                foreach(var courseware in coursewareList)
                {
                    var cwf = new CoursewareWithFiles()
                    {
                        Courseware = courseware,
                        Files = await GetCoursewareFilesAsync(courseware.Id)
                    };
                    cwfList.Add(cwf);
                }
                return cwfList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable to Get Courseware FileList!");
            }
        }

        public async Task<List<User>> GetCourseUsersAsync(int courseID)
        {
            try
            {
                var ucList = await dbContext.UserCourse.Where(uc => uc.CourseId == courseID).ToListAsync();
                var resList = new List<User>();
                foreach(var uc in ucList)
                {
                    var userInfo = await dbContext.Users.FindAsync(uc.UserId);
                    userInfo.Password = null;
                    userInfo.Introduction = null;
                    userInfo.Role = uc.UserRole;
                    resList.Add(userInfo);
                }
                return resList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Unable to Get Courseware FileList!");
            }
        }

        /// <summary>
        /// 创建所有组
        /// </summary>
        /// <param name="courseID"></param>
        /// <param name="groupCount"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public async Task AddCourseGroupAsync(int courseID, int groupCount, int maxCount)
        {
            try
            {
                await this.RemoveCourseGroupAsync(courseID);
                for(int i = 1; i <= groupCount; i++)
                {
                    var cg = new CourseGroup()
                    {
                        CourseId = courseID,
                        GroupId = i,
                        MaxCount = maxCount
                    };
                    await dbContext.CourseGroups.AddAsync(cg);
                }
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<CourseGroup> GetCourseGroupAsync(int courseID, int groupID)
        {
            try
            {
                var group = await dbContext.CourseGroups.FindAsync(groupID, courseID);
                if(group == null)
                {
                    throw new Exception("Group Not Found!");
                }
                return group;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<UserGroup> GetUserGroupAsync(int groupID, string userID, int courseID)
        {
            try
            {
                var ug = await dbContext.UserGroup.FindAsync(groupID, userID, courseID);
                return ug;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 获取课程的所有组列表
        /// </summary>
        /// <param name="courseID"></param>
        /// <returns></returns>
        public async Task<List<CourseGroup>> GetCourseGroupListAsync(int courseID)
        {
            try
            {
                var cgList = await dbContext.CourseGroups.Where(cg => cg.CourseId == courseID).ToListAsync();
                return cgList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task RemoveCourseGroupAsync(int courseID)
        {
            try
            {
                var cgList = await this.GetCourseGroupListAsync(courseID);
                dbContext.CourseGroups.RemoveRange(cgList);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 获取单组的成员列表
        /// </summary>
        /// <param name="courseID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public async Task<List<UserGroup>> GetGroupUserListAsync(int courseID, int groupID)
        {
            try
            {
                var resList = await dbContext.UserGroup.Where(ug => ug.CourseId == courseID && ug.GroupId == groupID).ToListAsync();
                return resList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<GroupMemberInfo> GetGroupMemberAsync(int courseID, int groupID, string userID)
        {
            try
            {
                var userInfo = await dbContext.Users.FindAsync(userID);
                if(userInfo == null)
                {
                    throw new Exception("User Not Found!");
                }
                userInfo.Password = null;
                userInfo.Introduction = null;
                var ug = await this.GetUserGroupAsync(groupID, userID, courseID);
                if(ug == null)
                {
                    throw new Exception("User-Group is Not Valid!");
                }
                return new GroupMemberInfo()
                {
                    UserInfo = userInfo,
                    UserGroup = ug
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 获取单组的成员信息列表
        /// </summary>
        /// <param name="courseID"></param>
        /// <param name="gourpID"></param>
        /// <returns></returns>
        public async Task<List<GroupMemberInfo>> GetGroupMemberListAsync(int courseID, int gourpID)
        {
            try
            {
                var resList = new List<GroupMemberInfo>();
                var ugList = await this.GetGroupUserListAsync(courseID, gourpID);
                foreach(var ug in ugList)
                {
                    var userInfo = await dbContext.Users.FindAsync(ug.UserId);
                    if(userInfo == null)
                    {
                        continue;
                    }
                    userInfo.Password = null;
                    userInfo.Introduction = null;
                    resList.Add(new GroupMemberInfo()
                    {
                        UserInfo = userInfo,
                        UserGroup = ug
                    });
                }
                return resList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 获取单组成员人数
        /// </summary>
        /// <param name="courseID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public async Task<int> GetGroupMemberCountAsync(int courseID, int groupID)
        {
            try
            {
                var ugList = await this.GetGroupUserListAsync(courseID, groupID);
                return ugList.Count;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 获取组信息
        /// </summary>
        /// <param name="courseID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public async Task<GroupInfo> GetGroupInfoAsync(int courseID, int groupID)
        {
            try
            {
                var cg = await this.GetCourseGroupAsync(courseID, courseID);
                var members = await this.GetGroupMemberListAsync(courseID, groupID);
                return new GroupInfo()
                {
                    CourseGroup = cg,
                    GroupMemberCount = members.Count,
                    Members = members
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<GroupInfo> GetGroupInfoAsync(CourseGroup group)
        {
            try
            {
                var members = await this.GetGroupMemberListAsync(group.CourseId, group.GroupId);
                return new GroupInfo()
                {
                    CourseGroup = group,
                    GroupMemberCount = members.Count,
                    Members = members
                };
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 获取课程所有组信息
        /// </summary>
        /// <param name="courseID"></param>
        /// <returns></returns>
        public async Task<List<GroupInfo>> GetGroupInfoListAsync(int courseID)
        {
            try
            {
                var groups = await this.GetCourseGroupListAsync(courseID);
                var resList = new List<GroupInfo>();
                foreach(var group in groups)
                {
                    resList.Add(await this.GetGroupInfoAsync(group));
                }
                return resList;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task<GroupIdentity> GetGroupIdentityAsync(int courseID, int groupID, string userID)
        {
            try
            {
                var ug = await this.GetUserGroupAsync(groupID, userID, courseID);
                if(ug == null)
                {
                    throw new Exception("User-Group Not Valid!");
                }
                return ug.Identity;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        public async Task SetGroupIdentityAsync(int courseID, int groupID, string userID, GroupIdentity identity)
        {
            try
            {
                var ug = await this.GetUserGroupAsync(groupID, userID, courseID);
                if (ug == null)
                {
                    throw new Exception("User-Group Not Valid!");
                }
                ug.Identity = identity;
                dbContext.UserGroup.Update(ug);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 添加成员到组，带人数验证
        /// </summary>
        /// <param name="stuID"></param>
        /// <param name="courseID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public async Task AddStuToGroup(string stuID, int courseID, int groupID)
        {
            try
            {
                var count = await this.GetGroupMemberCountAsync(courseID, groupID);
                var groupInfo = await this.GetCourseGroupAsync(courseID, groupID);
                if(count < groupInfo.MaxCount)
                {
                    var ug = new UserGroup()
                    {
                        GroupId = groupID,
                        UserId = stuID,
                        CourseId = courseID,
                        Identity = count == 0 ? GroupIdentity.Leader : GroupIdentity.Member
                    };
                    await dbContext.UserGroup.AddAsync(ug);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Group is Full!");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }

        /// <summary>
        /// 删除某组成员，带验证
        /// </summary>
        /// <param name="stuID"></param>
        /// <param name="courseID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public async Task RemoveStuFromGroup(string stuID, int courseID, int groupID)
        {
            try
            {
                var ug = await this.GetUserGroupAsync(groupID, stuID, courseID);
                if(ug == null)
                {
                    throw new Exception("User-Group Not Valid!");
                }
                dbContext.UserGroup.Remove(ug);
                await dbContext.SaveChangesAsync();
                if (ug.Identity == GroupIdentity.Leader)
                {
                    var groupMembers = await this.GetGroupUserListAsync(courseID, groupID);
                    if(groupMembers.Count > 0)
                    {
                        var memberToUpdate = groupMembers[0];
                        memberToUpdate.Identity = GroupIdentity.Leader;
                        dbContext.UserGroup.Update(memberToUpdate);
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new Exception("Action Failed!");
            }
        }
    }
}
