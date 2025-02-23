using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameService;
using Grpc.Net.Client;
using MassTransit.Futures.Contracts;

namespace OrderService.Services.GrpcFolder;

    public class GrpcMyGameClient
    {
        private readonly IConfiguration _config;
        public GrpcMyGameClient(IConfiguration config)
        {
            _config = config;
        }   
        public bool SaveMyGame(string UserId, string GameId)
        {
           var channel=GrpcChannel.ForAddress(_config["GrpcGame"]);
           var client=new GrpcMyGame.GrpcMyGameClient(channel);
           var request=new GetMyGameRequest
           {
               UserId=UserId,
               GameId=GameId
           };
           try
           {
               var response=client.GetMyGame(request);
               if(!string.IsNullOrEmpty(response.MyGame.UserId) && !string.IsNullOrEmpty(response.MyGame.GameId))
               {
                   return true;
               }
                return false;
           }
           catch(Exception ex)
           {
               throw ex;
           }
        }
    }
