using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Interfaces;
using Contracts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace XTestBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingTypesController : ControllerBase
    {
        private readonly ILogger<MeetingTypesController> _logger;
        private readonly IRepository<MeetingType> _meetingTypeRepository;
        public MeetingTypesController(IRepository<MeetingType> meetingTypeRepository, ILogger<MeetingTypesController> logger)
        {
            _meetingTypeRepository = meetingTypeRepository;
            _logger = logger;
        }
        // GET: api/Users
        [HttpGet]
        public IEnumerable<MeetingType> Get()
        {
            return _meetingTypeRepository.GetList();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public MeetingType Get(int id)
        {
            return _meetingTypeRepository.GetById(id);
        }

        // POST: api/Users
        [HttpPost]
        public void Post([FromBody] MeetingType value)
        {
            _meetingTypeRepository.Create(value);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public void Put([FromBody] MeetingType value)
        {
            _meetingTypeRepository.Create(value);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _meetingTypeRepository.Delete(id);
        }
    }
}
