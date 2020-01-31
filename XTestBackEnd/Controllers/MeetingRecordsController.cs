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
    public class MeetingRecordsController : ControllerBase
    {
        private readonly ILogger<MeetingRecordsController> _logger;
        private readonly IRepository<MeetingRecord> _meetingRecordRepository;
        public MeetingRecordsController(IRepository<MeetingRecord> meetingRecordRepository, ILogger<MeetingRecordsController> logger)
        {
            _meetingRecordRepository = meetingRecordRepository;
            _logger = logger;
        }
        // GET: api/Users
        [HttpGet]
        public IEnumerable<MeetingRecord> Get()
        {
            return _meetingRecordRepository.GetList();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public MeetingRecord Get(int id)
        {
            return _meetingRecordRepository.GetById(id);
        }

        // POST: api/Users
        [HttpPost]
        public void Post([FromBody] MeetingRecord value)
        {
            _meetingRecordRepository.Create(value);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public void Put([FromBody] MeetingRecord value)
        {
            _meetingRecordRepository.Create(value);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _meetingRecordRepository.Delete(id);
        }
    }
}
