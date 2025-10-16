using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoList.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class TodoController : ControllerBase
{


    [HttpGet("uid/{type}")]
    public IActionResult GetMaxID(long type)
    {
        if (type > 2) return BadRequest("Invalid type.");

        try
        {
            return Ok(SQLiteInterface.getUniqueID((SQLiteInterface.SQLIDColumn)type));
        }
        catch (Exception error)
        {
            string message = Config.Development ? $"{error.Message}\n{error.StackTrace}" : $"The following type is not supported: {type}.";
            Console.WriteLine(message);
            return BadRequest(message);
        }
        
    }

    // GET: api/<TodoController>
    // URL: .../todo/
    [HttpGet("task/")]
    public IActionResult Get()
    {
        IEnumerable<TaskRecordOutputModel> outputs = SQLiteInterface.retrieveRecords().Select<TaskRecord, TaskRecordOutputModel>(x => new TaskRecordOutputModel(x));
        if (outputs.Any())
        {
            return Ok(outputs);
        }

        return BadRequest();
    }

    // GET api/<TodoController>/5/7/0
    // URL: .../todo/5/7/0
    [HttpGet("task/{id}/{idend}")]
    public IActionResult Get(long id, long idend)
    {
        //IEnumerable<TaskRecordOutputModel>
        try
        {

            return Ok(SQLiteInterface.retrieveRecords(id, idend).Select<TaskRecord, TaskRecordOutputModel>(x => new TaskRecordOutputModel(x)));
        }
        catch(Exception error)
        {
            string message = Config.Development ? $"{error.Message}\n{error.StackTrace}" : "No tasks with these parameters.";
            Console.WriteLine(message);
            return BadRequest(message);
        }
    }

    // GET api/<TodoController>/5
    // URL: .../todo/5/
    [HttpGet("task/{id}")]
    public IActionResult Get(long id)
    {
        try {
            return Ok(SQLiteInterface.retrieveRecords(id, id).Select<TaskRecord, TaskRecordOutputModel>(x => new TaskRecordOutputModel(x)));
        }
        catch(Exception error)
        {
            string message = Config.Development ? $"{error.Message}\n{error.StackTrace}" : "No tasks with these parameters.";
            Console.WriteLine(message);
            return BadRequest(message);
            
        }
    }

    //timezone offset is in minutes
    //Date.getTimezoneOffset gets timezone offset in minutes
    //suppose z = offset
    //the representation is GMT-z
    //i.e. GMT-5 is 300, and GMT+1 is -60
    [HttpGet("time/{timezoneOffset}")]
    public IActionResult Get(string timezoneOffset)
    {
        try
        {
            int offsetMins = Convert.ToInt32(timezoneOffset);
            TimeZoneInfo zoneInfo = TimeZoneInfo.Local;
            //Console.WriteLine($"display: {zoneInfo.DisplayName}, is daylight savings time: {zoneInfo.IsDaylightSavingTime(DateTime.Now)}");

            int hours = zoneInfo.BaseUtcOffset.Hours;
            if (zoneInfo.IsDaylightSavingTime(DateTime.Now)) hours++;
            int minutes = zoneInfo.BaseUtcOffset.Minutes + hours*60;
            //Console.WriteLine(minutes);
            return Ok(new TimeSyncModel(offsetMins*(-1), minutes));
        }
        catch (Exception error)
        {
            string message = Config.Development ? $"{error.Message}\n{error.StackTrace}" : "Invalid input.";
            return BadRequest(message);
        }

    }



    // POST api/<TodoController>/post
    [HttpPost("post")]
    public IActionResult Post([FromBody] TaskRecordInputModel value)
    {
        //Console.WriteLine($"{value.TaskName} received");
        try
        {
            SQLiteInterface.insertRecord(TaskRecord.validateTaskRecordInputModel(value));

            return Ok($"Grape");
        } catch (Exception error) {
            string message = Config.Development ? $"{error.Message}\n{error.StackTrace}" : "No tasks with these parameters.";
            return BadRequest(message);
        }
    }

    



    // DELETE api/<TodoController>/5
    // URL: .../todo/5
    [HttpDelete("{id}")]
    public void Delete(long id)
    {
        Delete(id,id);
    }

    // DELETE api/<TodoController>/5/7
    // URL: .../todo/5/7/
    [HttpDelete("{id}/{idend}")]
    public void Delete(long id,long idend)
    {
        SQLiteInterface.deleteRecords(id,idend);
    }


}
