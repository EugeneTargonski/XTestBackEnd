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
    public class UnplannedDaysController : ControllerBase
    {
        private readonly ILogger<UnplannedDaysController> _logger;
        private readonly IRepository<UnplannedDay> _unplannedDayRepository;
        public UnplannedDaysController(IRepository<UnplannedDay> unplannedDayRepository, ILogger<UnplannedDaysController> logger)
        {
            _unplannedDayRepository = unplannedDayRepository;
            _logger = logger;
        }
        // GET: api/Users
        [HttpGet]
        public IEnumerable<UnplannedDay> Get()
        {
            return _unplannedDayRepository.GetList();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public UnplannedDay Get(int id)
        {
            return _unplannedDayRepository.GetById(id);
        }

        // POST: api/Users
        [HttpPost]
        public void Post([FromBody] UnplannedDay value)
        {
            //if (_unplannedDayRepository.GetByFieldValue("GoogleID", value.GoogleID, false).Count == 0)
                _unplannedDayRepository.Create(value);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public void Put([FromBody] UnplannedDay value)
        {
            _unplannedDayRepository.Create(value);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _unplannedDayRepository.Delete(id);
        }
    }
}