using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_experiment.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private static ConcurrentDictionary<int, string> m_db =
            new ConcurrentDictionary<int, string>();
        private static int m_counter = 0;

        static ValuesController()
        {
            m_db.TryAdd(++m_counter, "value1");
            m_db.TryAdd(++m_counter, "value2");
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return m_db.Values;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (m_db.TryGetValue(id, out string value))
            {
                return Ok(value);
            }
            else
            {
                return NotFound();
            }
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            if (value == null)
            {
                return BadRequest();
            }

            int id;
            while (!m_db.TryAdd(id = Interlocked.Increment(ref m_counter), value))
            {
                // Spin until it works.
            }

            return CreatedAtAction(nameof(Get), new { id }, value);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string value)
        {
            if (value == null)
            {
                return BadRequest();
            }

            if (m_db.TryGetValue(id, out string oldValue))
            {
                if (m_db.TryUpdate(id, value, oldValue))
                {
                    return Ok(value);
                }
                else
                {
                    return StatusCode(409);
                }
            }
            else
            {
                return NotFound();
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (m_db.TryRemove(id, out string value))
            {
                return Ok(value);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
