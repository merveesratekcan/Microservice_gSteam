using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameService.Base;

namespace GameService.Entities;

    public class MyGame:BaseModel
    {
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
    }
