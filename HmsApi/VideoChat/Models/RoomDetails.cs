﻿namespace HmsApi.VideoChat.Models
{
    public record RoomDetails(
        string Id,
        string Name,
        int ParticipantCount,
        int MaxParticipants);
}
