using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        [HttpPost]
        [Route("Sync")]
        public GResponse<bool> Sync(SensorData Data)
        {
            return new GResponse<bool>
            {
                Message = "Succeeded",
                Data = true
            };
        }

        [HttpGet]
        [Route("SyncGet")]
        public GResponse<bool> SyncGet()
        {
            return new GResponse<bool>
            {
                Message = "Succeeded",
                Data = true
            };
        }
    }
}
