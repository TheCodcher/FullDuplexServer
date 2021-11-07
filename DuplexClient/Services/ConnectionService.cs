using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using DuplexDomain.Dto.RestreamHub;

namespace DuplexClient.Services
{
    //need more dto to event and ...
    class ConnectionService
    {
        HubConnection _connection;

        public event Action<string[]> OnExceptions;
        public event Action<string[]> OnUserListChanges;
        public event Action<string> OnGroupCreate;

        public ConnectionService(Func<Task<string>> tokenProvider)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("", opt =>
                {
                    opt.AccessTokenProvider = tokenProvider;
                })
                .Build();

            _connection.On<RestreamBaseResponseDto>(RestreamHubDefaults.CLIENT_EXCEPTION_METHOD, e => OnExceptions?.Invoke(e.Exceptions));
            _connection.On<RestreamGroupUsersDto>(RestreamHubDefaults.CLIENT_GROUP_USERS_METHOD, e => OnUserListChanges?.Invoke(e.GroupUsers));
            _connection.On<RestreamGroupIdResponseDto>(RestreamHubDefaults.CLIENT_GROUP_CODE_METHOD, e => OnGroupCreate?.Invoke(e.GroupCode));
        }

        public async Task Connect()
        {
            //to create chanel 
        }

        public async Task SendData(byte[] data)
        {

        }

        private async Task Recieve(byte[] data)
        {

        }

        public async Task JoinGroup(string groupCode)
        {
            var dto = new RestreamJoinRequestDto
            {
                GroupCode = groupCode
            };
            await _connection.SendAsync(RestreamHubDefaults.SERVER_JOIN_METHOD, dto);
        }

        public async Task CreateGroup()
        {
            await _connection.SendAsync(RestreamHubDefaults.SERVER_CREATE_METHOD);
        }
    }
}
