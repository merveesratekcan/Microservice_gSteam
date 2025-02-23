using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameService.DTOs
{
    public class MyGameDTO
    {
        public Guid GameId { get; set; }
        public Guid UserId { get; set; }
    }
}