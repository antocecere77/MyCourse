using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public class EFCoreCoursesService : ICourseService
    {
        private readonly MyCourseDbContext dbContext;

        public EFCoreCoursesService(MyCourseDbContext dbContext)
        {
            this.dbContext = dbContext;

        }

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            IQueryable<CourseDetailViewModel> queryLinq = dbContext.Courses
                .AsNoTracking()
                .Include(course => course.Lessons)
                .Where(course => course.Id == id)
                .Select(course => CourseDetailViewModel.FromEntity(course));

            //Alternativa con sintassi dichiarativa
            // IQueryable<CourseDetailViewModel> queryLinq2 = 
            //     from course in dbContext.Courses.AsNoTracking()
            //     .Include(course => course.Lessons)
            //     where course.Id == id
            //     select CourseDetailViewModel.FromEntity(course);

                // .Select(course => new CourseDetailViewModel {
                //     Id = course.Id,
                //     Title = course.Title,
                //     Description = course.Description,
                //     ImagePath = course.ImagePath,
                //     Author = course.Author,
                //     Rating = course.Rating,
                //     CurrentPrice = course.CurrentPrice,
                //     FullPrice = course.FullPrice,
                //     Lessons = course.Lessons.Select(lesson => new LessonViewModel {
                //         Id = lesson.Id,
                //         Title = lesson.Title,
                //         Description = lesson.Description,
                //         Duration = lesson.Duration
                //     }).ToList()  
                // }); //.SingleAsync(); //Restituisce il primo elemento dell'elenco, ma se l'elenco ne contiene più di uno solleva un eccezione
                    //.FirstOrDefaultAsync(); //Restituisce null se l'elenco è vuoto e non solleva mai un'eccezione
                    //.SingleOrDefaultAsync(); //Tollera il fatto che l'elenco sia vuoto e in quel caso restituisce null, oppure se l'elenco contiene più di 1 elemento, solleva un'eccezione
                    //.FirstAsync(); //Restituisce il primo elemento, ma se l'elenco è vuoto solleva un'eccezione

            CourseDetailViewModel viewModel = await queryLinq.SingleAsync();
            return viewModel;
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync()
        {
            IQueryable<CourseViewModel> queryLinq = dbContext.Courses
            .AsNoTracking()
            .Select(course => CourseViewModel.FromEntity(course));

            // IQueryable<CourseViewModel> queryLinq2 =
            //     from course in dbContext.Courses.AsNoTracking()
            //     select CourseViewModel.FromEntity(course);

            // .Select(course => 
            //     new CourseViewModel {
            //         Id = course.Id,
            //         Title = course.Title,
            //         ImagePath = course.ImagePath,
            //         Author = course.Author,
            //         Rating = course.Rating,
            //         CurrentPrice = course.CurrentPrice,
            //         FullPrice = course.FullPrice                
            //     });

            List<CourseViewModel> courses = await queryLinq.ToListAsync();
            
            return courses;
        }
    }
}