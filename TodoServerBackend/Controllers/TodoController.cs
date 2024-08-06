using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoList.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        // GET: api/<TodoController>
        [HttpGet]
        public IEnumerable<TaskRecordOutputModel> Get()
        {
            return SQLiteInterface.retrieveRecords().Select<TaskRecord,TaskRecordOutputModel>(x => new TaskRecordOutputModel(x));
        }

		// GET api/<TodoController>/5/7
		[HttpGet("{id}/{idend}")]
        public IEnumerable<TaskRecordOutputModel> Get(int id, int idend)
        {
            try
            {
                return SQLiteInterface.retrieveRecords(id, idend).Select<TaskRecord, TaskRecordOutputModel>(x => new TaskRecordOutputModel(x));
            }
            catch
            {
                return new List<TaskRecordOutputModel>();
            }
		}

		// GET api/<TodoController>/5
		[HttpGet("{id}")]
        public TaskRecordOutputModel Get(int id)
        {
            try { 
                TaskRecordOutputModel model = Get(id, id).First();
				//Console.WriteLine($"{model.TaskName} sent");
                return model;
			}
			catch
			{
				return new TaskRecordOutputModel(new TaskRecord(99999, "Invalid","No tasks with those conditions", DateTime.MinValue, DateTime.Now));
			}
		}

        // POST api/<TodoController>
        [HttpPost("post")]
        public void Post([FromBody] TaskRecordInputModel value)
        {
            //Console.WriteLine($"{value.TaskName} received");
            SQLiteInterface.insertRecord(new TaskRecord(SQLiteInterface.getUniqueID(),value.TaskName, value.TaskDetails, DateTime.Parse(value.TaskDeadline),null));
        }


        // DELETE api/<TodoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            Delete(id,id);
        }

		// DELETE api/<TodoController>/5/7
		[HttpDelete("{id}/{idend}")]
		public void Delete(int id,int idend)
		{
			SQLiteInterface.deleteRecords(id,idend);
		}

		[HttpGet("uid")]
        public int GetMaxID()
        {
            return SQLiteInterface.getUniqueID();
        }
    }
}
