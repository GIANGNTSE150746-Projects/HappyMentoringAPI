using BusinessObjectLibrary;
using DataAccessLibrary.Interfaces;
using HmsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataAccessLibrary.Business_Entity
{
    public class RatingBusinessEntity
    {
        private IUnitOfWork work; 
        public RatingBusinessEntity(IUnitOfWork work)
        {
            this.work = work;
        }

        public async Task<IEnumerable<Rating>> GetRatingsAsync()
        {
            return (await work.Ratings.GetAllAsync())
                .Where(r => !r.IsDeleted);
        }

        public async Task<Rating> GetRatingAsync(string id)
        {
            return (await work.Ratings.GetAllAsync())
                .Where(r => !r.IsDeleted && r.Id.Equals(id))
                .FirstOrDefault();
        }

        public async Task<Rating> AddRating(Rating rating)
        {
            string id = HmsUtils.CreateGuid();
            rating.Id = id;
            rating.IsDeleted = false;
            rating.CreatedDate = DateTime.Now;
            await work.Ratings.AddAsync(rating);
            await work.Save();
            return rating;
        }

        public async Task<Rating> UpdateRating(Rating rating)
        {
            work.Ratings.Update(rating);
            await work.Save();
            return rating;
        }

        public async Task RemoveRating(string id)
        {
            Rating rating = await work.Ratings.GetAsync(id);
            rating.IsDeleted = true;
            work.Ratings.Update(rating);
            await work.Save();
        }
    }
}
