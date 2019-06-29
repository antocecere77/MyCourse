using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MyCourse.Models.Options;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public class MemoryCachedCoursesService : ICachedCourseService
    {

        private readonly ICourseService courseService;
        private readonly IMemoryCache memoryCache;
        private readonly IOptionsMonitor<CachingOptions> cachingOptions;

        public MemoryCachedCoursesService(ICourseService courseService, IMemoryCache memoryCache, IOptionsMonitor<CachingOptions> cachingOptions) {
            this.courseService = courseService;
            this.memoryCache = memoryCache;
            this.cachingOptions = cachingOptions;
        }

        public Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            return memoryCache.GetOrCreateAsync($"Course{id}", cacheEntry => 
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(GetExpirationCacheInSeconds()));
                return courseService.GetCourseAsync(id);
            });
        }

        public Task<List<CourseViewModel>> GetCoursesAsync()
        {         
           return memoryCache.GetOrCreateAsync($"Courses", cacheEntry => {
               cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(GetExpirationCacheInSeconds()));
               return courseService.GetCoursesAsync();
           });
        }

        private int GetExpirationCacheInSeconds() {
            return cachingOptions.CurrentValue.CacheDurationInSeconds;
        }
    }
}