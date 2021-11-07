using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using DuplexDomain.Dto.RestreamHub;

namespace FullDuplexServer.Controllers
{
    [Authorize]
    public class RestreamHub : Hub
    {
        Dictionary<string, string> userRooms = new Dictionary<string, string>();
        HashSet<int> roomsIndx = new HashSet<int>();

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (userRooms.TryGetValue(Context.ConnectionId, out var roomName))
            {
                userRooms.Remove(Context.ConnectionId);
                await SendGroupUsersToGroupAsync(roomName);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public Task Recieve(byte[] data)
        {
            return Clients.OthersInGroup(userRooms[Context.ConnectionId]).SendAsync(RestreamHubDefaults.CLIENT_DATA_METHOD, data);
        }

        public async Task Join(string groupId)
        {
            groupId = groupId.ToUpper();
            if (userRooms.ContainsValue(groupId))
            {
                if (userRooms.TryAdd(Context.ConnectionId, groupId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
                    await SendGroupUsersToGroupAsync(groupId);
                }
                else
                {
                    if (userRooms[Context.ConnectionId] == groupId)
                        await SendExceptionToCallerAsync(RestreamBaseResponseDto.SetException("you are already in this group"));
                    else
                        await SendExceptionToCallerAsync(RestreamBaseResponseDto.SetException("you are already in another group"));
                }
            }
            else
            {
                await SendExceptionToCallerAsync(RestreamBaseResponseDto.SetException("group not found"));
            }
        }

        public async Task Create()
        {
            var group = CreateGroupId();
            await SendGroupCodeToCallerAsync(new RestreamGroupIdResponseDto { GroupCode = group });
        }

        private async Task SendExceptionToCallerAsync<T>(T message) where T: RestreamBaseResponseDto
        {
            await Clients.Caller.SendAsync(RestreamHubDefaults.CLIENT_EXCEPTION_METHOD, message);
        }

        private async Task SendGroupCodeToCallerAsync(RestreamGroupIdResponseDto message)
        {
            await Clients.Caller.SendAsync(RestreamHubDefaults.CLIENT_GROUP_CODE_METHOD, message);
        }

        private async Task SendGroupUsersToGroupAsync(string groupId)
        {
            var users = userRooms.Where(v => v.Value == groupId).Select(v => v.Key).ToArray();
            await Clients.Groups(groupId).SendAsync(RestreamHubDefaults.CLIENT_GROUP_USERS_METHOD, users);
        }

        private string CreateGroupId()
        {
            var num = new Random((int)DateTime.Now.Ticks).Next(10000, 100000);
            while (roomsIndx.Contains(num))
            {
                num++;
            }
            roomsIndx.Add(num);
            var roomName = num.ToString("X2").ToUpper();
            userRooms.Add(Context.ConnectionId, roomName);
            return roomName;
        }
    }
}
