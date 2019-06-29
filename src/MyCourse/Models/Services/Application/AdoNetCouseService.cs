using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCourse.Models.Exceptions;
using MyCourse.Models.Options;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public class AdoNetCouseService : ICourseService
    {

        private readonly IDatabaseAccessor db;
        private readonly IMapper mapper;
        private readonly IOptionsMonitor<CoursesOptions> coursesOptions;

        private readonly ILogger<AdoNetCouseService> logger;

        public AdoNetCouseService(ILogger<AdoNetCouseService> logger, IDatabaseAccessor db, IMapper mapper, IOptionsMonitor<CoursesOptions> coursesOptions)
        {
            this.db = db;
            this.mapper = mapper;
            this.coursesOptions = coursesOptions;
            this.logger = logger;
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync()
        {
            FormattableString query = $"SELECT Id, Title, Author, ImagePath, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency FROM Courses";
            DataSet dataSet = await db.QueryAsync(query);
            var dataTable = dataSet.Tables[0];
            var courseList = mapper.Map<List<CourseViewModel>>(dataTable.Rows);

            //var courseList = new List<CourseViewModel>();
            // foreach(DataRow courseRow in dataTable.Rows) {
            //     CourseViewModel course = CourseViewModel.FromDataRow(courseRow);
            //     courseList.Add(course);
            // }
            return courseList;
        }

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {

            logger.LogInformation("Course {id} requested", id);

            FormattableString query = $@"SELECT Id, Title, Description, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency FROM Courses WHERE Id= {id}; 
            SELECT Id, Title, Description, Duration FROM Lessons WHERE CourseId= {id}";

            DataSet dataSet = await db.QueryAsync(query);

            //Coursek
            var courseTable = dataSet.Tables[0];
            if(courseTable.Rows.Count!=1) {
                logger.LogWarning("Course {id} not found", id);
                throw new CourseNotFoundException(id);
            }

            var courseRow = courseTable.Rows[0];
            //var courseDetailViewModel = CourseDetailViewModel.FromDataRow(courseRow);
             var courseDetailViewModel = mapper.Map<CourseDetailViewModel>(courseRow);

            //Course lessons
            var lessonDataTable = dataSet.Tables[1];
            courseDetailViewModel.Lessons = mapper.Map<List<LessonViewModel>>(lessonDataTable.Rows);

            //foreach(DataRow lessonRow in lessonDataTable.Rows) {
            //    LessonViewModel lessonViewModel = LessonViewModel.FromDataRow(lessonRow);
            //    courseDetailViewModel.Lessons.Add(lessonViewModel);
            //}

            return courseDetailViewModel;
        }

    }
}