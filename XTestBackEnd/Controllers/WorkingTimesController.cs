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
    public class WorkingTimesController : ControllerBase
    {
        private readonly ILogger<WorkingTimesController> _logger;
        private readonly IRepository<WorkingTime> _workingTimeRepository;
        public WorkingTimesController(IRepository<WorkingTime> workingTimeRepository, ILogger<WorkingTimesController> logger)
        {
            _workingTimeRepository = workingTimeRepository;
            _logger = logger;
        }
        // GET: api/Users
        [HttpGet]
        public IEnumerable<WorkingTime> Get()
        {
            return _workingTimeRepository.GetList();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public WorkingTime Get(int id)
        {
            return _workingTimeRepository.GetById(id);
        }

        // POST: api/Users
        [HttpPost]
        public void Post([FromBody] WorkingTime value)
        {
            _workingTimeRepository.Create(value);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public void Put([FromBody] WorkingTime value)
        {
            _workingTimeRepository.Create(value);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _workingTimeRepository.Delete(id);
        }
    }
}
