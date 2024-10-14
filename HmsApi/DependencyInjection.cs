using BusinessObjectLibrary;
using DataAccessLibrary.Implementations;
using DataAccessLibrary.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HmsApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            #region DbContext
            services.AddDbContext<HmsContext>(options =>
            {
                options.UseSqlServer(HmsLibrary.HmsConfiguration.ConnectionString);
            });
            #endregion

            #region Repository
            services.AddScoped<IGenericRepository<Cv>, GenericRepository<Cv>>();
            services.AddScoped<IGenericRepository<MentorSkill>, GenericRepository<MentorSkill>>();
            services.AddScoped<IGenericRepository<Rating>, GenericRepository<Rating>>();
            services.AddScoped<IGenericRepository<Request>, GenericRepository<Request>>();
            services.AddScoped<IGenericRepository<RequestSkill>, GenericRepository<RequestSkill>>();
            services.AddScoped<IGenericRepository<Seminar>, GenericRepository<Seminar>>();
            services.AddScoped<IGenericRepository<SeminarParticipant>, GenericRepository<SeminarParticipant>>();
            services.AddScoped<IGenericRepository<Skill>, GenericRepository<Skill>>();
            services.AddScoped<IGenericRepository<TeachingThread>, GenericRepository<TeachingThread>>();
            services.AddScoped<IGenericRepository<User>, GenericRepository<User>>();
            services.AddScoped<IGenericRepository<MentorDetail>, GenericRepository<MentorDetail>>();

            #endregion

            #region UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion

            return services;
        }
    }
}
