using HmsApi.VideoChat.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HmsApi.VideoChat.Services
{
    public interface IVideoService
    {
        string GetTwilioJwt(string identity);
        Task<IEnumerable<RoomDetails>> GetAllRoomsAsync();
    }
}
