using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading;
using System.Threading.Tasks;
namespace youknow.Hubs
{
    public class ync : Hub
    {
        private readonly TimeSpan BroadcastInterval =
            TimeSpan.FromMilliseconds(2000);        
        private Timer _broadcastLoop;
        private bool isRunning = false;
        
        public async Task JoinRoom(string roomName, string username, string usertoken, string contents, string title, string id, string idReply, string userTokenReply, string userNameReply)
        {
            await Groups.Add(Context.ConnectionId, roomName);            
            Clients.Group(roomName).addChatMessage(roomName, username, usertoken, contents, title, id, idReply, userTokenReply, userNameReply);
        }
        public async Task broadCast(string roomName,string type, string username, string usertoken, string contents, string title, string id,string idNews)
        {
            await Groups.Add(Context.ConnectionId, roomName);            
            Clients.Group(roomName).broadCast(roomName,type,username, usertoken, contents, title, id, idNews);
        }
        public async Task noticeNews(string roomName)
        {
            await Groups.Add(Context.ConnectionId, roomName);
            Clients.Group(roomName).noticeNews(roomName);
        }
        public async Task LeaveRoom(string roomName)
        {
            await Groups.Remove(Context.ConnectionId, roomName);
        }
        public async Task LeaveRoomComment(string token)
        {
            await Groups.Remove(Context.ConnectionId, token);
        }
        
    }
}