using TodoList.Server;
using System.Data.SQLite;
using System.Text;
using System.Data.Entity.Infrastructure;
using System.Data;

/*
 *	Task Name: Name of the task
 *	Task Description: (Optional) Description of the task
 *	Task Creation Date: The time the server creates the task
 *	Task Deadline: The deadline of the task
 *	Task Customization: Customization for the task
 *	Task Grade: Grade of the task
 *	Task Weight: Weight for course, or progress for the over-arching project
 *	Task project/course: The project or course that the task falls under
 *		Name, ID
 *	Project/Course category: The category for the project/course
 *		Name, ID
 */





static class SQLiteInterface
{
	public static string ConnectionString = "Data Source=tasklist.db;Version=3;FailIfMissing=True;";



	public enum SQLIDColumn
	{
		ID,
		ProjectID,
		ProjectCategoryID,

	}

	public enum DBColumns {
		ID,
		Name,
		Details,
		CreationDate,
		Deadline,
		Grade,
		Weight,
		ProjectName,
		ProjectID,
		ProjectCategoryName,
		ProjectCategoryID,
		Customization,
		CompletionStatus
	}



	public static long getInt64(SQLiteDataReader reader, DBColumns column)
	{
		switch (column)
		{
			case DBColumns.ID:
			case DBColumns.CompletionStatus:
			case DBColumns.ProjectID:
			case DBColumns.ProjectCategoryID:
			case DBColumns.Deadline:
			case DBColumns.CreationDate:
				{
					int index = reader.GetOrdinal(column.ToString());
					bool isNull = reader.IsDBNull(index);
					if (isNull)
					{
						return -1;
					}
					else
					{
						return reader.GetInt64(index);
					}
				}
			default:
				return 0;
		}
	}

	public static string getString(SQLiteDataReader reader, DBColumns column)
	{
		switch (column)
		{
			case DBColumns.Name:
			case DBColumns.Details:
			case DBColumns.ProjectName:
			case DBColumns.ProjectCategoryName:
			case DBColumns.Customization:
				{
					int index = reader.GetOrdinal(column.ToString());
					bool isNull = reader.IsDBNull(index);
					if (isNull)
					{
						return string.Empty;
					}
					else
					{
						return reader.GetString(index);
					}
				}
			default:
				return string.Empty;
		}
	}
	
	public static double getDouble(SQLiteDataReader reader, DBColumns column)
	{
		switch (column)
		{
			case DBColumns.Grade:
			case DBColumns.Weight:
				{
					int index = reader.GetOrdinal(column.ToString());
					bool isNull = reader.IsDBNull(index);
					if (isNull)
					{
						return 0;
					}
					else
					{
						return reader.GetDouble(index);
					}
				}
			default:
				return 0;
		}
	}

	public static DateTime getDateTime(SQLiteDataReader reader, DBColumns column)
	{
		switch (column)
		{
			case DBColumns.Deadline:
			case DBColumns.CreationDate:
				{
					int index = reader.GetOrdinal(column.ToString());
					bool isNull = reader.IsDBNull(index);
					if (isNull)
					{
						return DateTime.MinValue;
					}
					else
					{
						return new DateTime(reader.GetInt64(index));
					}
				}
			default:
				return DateTime.MinValue;
		}
	}

	public static TaskRecord GetTaskRecordFromDataReader(SQLiteDataReader reader)
	{
		//to make it easier to debug if sql has a funny moment
		long ID = getInt64(reader, DBColumns.ID);
		string taskName = getString(reader, DBColumns.Name);
		string taskDetails = getString(reader, DBColumns.Details);

		DateTime taskDeadline = getDateTime(reader, DBColumns.Deadline);
		DateTime taskCreationDate = getDateTime(reader, DBColumns.CreationDate);

		double taskGrade = getDouble(reader, DBColumns.Grade);
		double taskWeight = getDouble(reader, DBColumns.Weight);

		string taskProjectName = getString(reader, DBColumns.ProjectName);
		long taskProjectID = getInt64(reader, DBColumns.ProjectID);
		string projectCategoryName = getString(reader, DBColumns.ProjectCategoryName);
		long projectCategoryID = getInt64(reader, DBColumns.ProjectCategoryID);

		string taskCustomization = getString(reader, DBColumns.Customization);
		long taskCompletionStatus = getInt64(reader, DBColumns.CompletionStatus);

		return
			new TaskRecord(
				ID: ID,
				taskName: taskName,
				taskDetails: taskDetails,
				taskDeadline: taskDeadline,
				taskCreationDate: taskCreationDate,
				taskGrade: taskGrade,
				taskWeight: taskWeight,
				taskProjectName: taskProjectName,
				taskProjectID: taskProjectID,
				projectCategoryName: projectCategoryName,
				projectCategoryID: projectCategoryID,
				taskCustomization: taskCustomization,
				taskCompletionStatus: taskCompletionStatus
			);
	}

	public static long getUniqueID(SQLIDColumn column = SQLIDColumn.ID)
	{
		List<long> ints = new List<long>();
		long uniqueID = 0;
		string columnName = string.Empty;

		columnName = column.ToString();
		//Console.WriteLine(columnName);

		using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
		{
			using (SQLiteCommand cmd = conn.CreateCommand())
			{
				conn.Open();
				cmd.CommandText = $"SELECT {columnName} FROM tasklist ORDER BY {columnName} ASC";
				SQLiteDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					if (reader.IsDBNull(0))
					{
						continue; //skip
					}
					ints.Add(reader.GetInt64(0));
				}
				conn.Close();
			}
		}
		if (ints.Count == 0)
		{
			uniqueID = 0;
		}
		else
		{
			//go through all ints from 0 -> maximum long value
			for (long i = 0; i <= ints.Max() + 1; ++i)
			{
				//if not found within range
				if (ints.Contains(i) == false)
				{
					uniqueID = i;
					break;
				}
			}
		}
		return uniqueID;
	}



	public static void insertRecord(TaskRecord record)
	{
		using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
		{
			using (SQLiteCommand cmd = conn.CreateCommand())
			{
				conn.Open();
				//sql sanitize
				cmd.CommandText = $"INSERT INTO tasklist (ID, Name, Details, CreationDate, Deadline, Grade, Weight, ProjectName, ProjectID, ProjectCategoryName, ProjectCategoryID, Customization, CompletionStatus) VALUES ({record.ID},\"{record.TaskName}\",\"{record.TaskDetails}\",\"{record.TaskCreationDate.Ticks}\",\"{record.TaskDeadline.Ticks}\",\"{record.TaskGrade}\",\"{record.TaskWeight}\",\"{record.TaskProjectName}\",\"{record.TaskProjectID}\",\"{record.ProjectCategoryName}\",\"{record.ProjectCategoryID}\",\"{record.TaskCustomization}\",\"{record.TaskCompletionStatus}\");";
				cmd.ExecuteNonQuery();
				conn.Close();
			}
		}
	}



	public static IEnumerable<TaskRecord> retrieveRecords()
	{
		List<TaskRecord> records = new List<TaskRecord>();

		StringBuilder builder = new StringBuilder();

		long i = 0;
		Array array = Enum.GetValues(typeof(DBColumns));
		for (; i < array.Length - 1; ++i)
		{
			builder.Append(array.GetValue(i));
			builder.Append(',');
		}

		builder.Append(array.GetValue(i));


		using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
		{
			using (SQLiteCommand cmd = conn.CreateCommand())
			{


				conn.Open();
				cmd.CommandText = $"SELECT {builder} FROM tasklist; --additional stuff may be added here later for filtering";
				//Console.WriteLine(cmd.CommandText);
				SQLiteDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					try
					{
						records.Add(GetTaskRecordFromDataReader(reader));
					}
					catch (Exception error)
					{
						Console.WriteLine($"{error.Message}\n{error.StackTrace}");
					}
				}
				//for (long j = 0; j < 13; ++j) Console.WriteLine(reader.GetFieldType(j));
				//Console.WriteLine($"Step Count: {reader.StepCount}");
				conn.Close();
			}
		}
		return records;
	}




	public static IEnumerable<TaskRecord> retrieveRecords(long id, long idend, SQLIDColumn column = SQLIDColumn.ID)
	{
		List<TaskRecord> records = new List<TaskRecord>();

		StringBuilder builder = new StringBuilder();

		long i = 0;
		Array array = Enum.GetValues(typeof(DBColumns));
		for (; i < array.Length - 1; ++i)
		{
			builder.Append(array.GetValue(i));
			builder.Append(',');
		}

		builder.Append(array.GetValue(i));

		using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
		{
			using (SQLiteCommand cmd = conn.CreateCommand())
			{
				conn.Open();
				cmd.CommandText = $"SELECT {builder} FROM tasklist WHERE {column} BETWEEN {id} AND {idend}; --additional stuff may be added here later for filtering";
				SQLiteDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					try
					{
						records.Add(GetTaskRecordFromDataReader(reader));
					}
					catch (Exception error)
					{
						Console.WriteLine($"{error.Message}\n{error.StackTrace}");
					}
				}
				//Console.WriteLine($"Step Count: {reader.StepCount}");
				conn.Close();
			}
		}
		return records;
	}



	public static void deleteRecords(long id, long idend, SQLIDColumn column = SQLIDColumn.ID)
	{
		using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
		{
			using (SQLiteCommand cmd = conn.CreateCommand())
			{
				conn.Open();
				cmd.CommandText = $"DELETE FROM tasklist WHERE {column} BETWEEN {id} AND {idend};";
				cmd.ExecuteNonQuery();
				conn.Close();
			}
		}
	}



	public static void updateRecordsCompletionStatus(TaskRecordInputModel model, long id, long idend, string noChange)
	{
        StringBuilder builder = new StringBuilder();


		//the below conditions will update the table
		if(model.InputProjectCategoryID != "0") builder.Append($"{DBColumns.ProjectCategoryID} = '{model.InputProjectCategoryID}',");
		if(model.InputProjectCategoryName != "") builder.Append($"{DBColumns.ProjectCategoryName} = '{model.InputProjectCategoryName}',");
		if(model.InputTaskCompletionStatus != "0" && model.InputTaskCompletionStatus != noChange) builder.Append($"{DBColumns.CompletionStatus} = '{model.InputTaskCompletionStatus}',");
		if(model.InputTaskCustomization != "") builder.Append($"{DBColumns.Customization} = '{model.InputTaskCustomization}',");
		if(model.InputTaskDeadline != "") builder.Append($"{DBColumns.Deadline} = '{model.InputTaskDeadline}',");
		if(model.InputTaskDetails != "") builder.Append($"{DBColumns.Details} = '{model.InputTaskDetails}',");
		if(model.InputTaskGrade != "0") builder.Append($"{DBColumns.Grade} = '{model.InputTaskGrade}',");
		if(model.InputTaskName != "") builder.Append($"{DBColumns.Name} = '{model.InputTaskName}',");
		if(model.InputTaskProjectID != "0") builder.Append($"{DBColumns.ProjectID} = '{model.InputTaskProjectID}',");
		if(model.InputTaskProjectName != "") builder.Append($"{DBColumns.ProjectName} = '{model.InputTaskProjectName}',");
		if(model.InputTaskWeight != "0") builder.Append($"{DBColumns.Weight} = '{model.InputTaskWeight}',");

		if(builder[builder.Length - 1] == ','){
            builder.Remove(builder.Length - 1, 1);
        }



        using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
		{
			using (SQLiteCommand cmd = conn.CreateCommand())
			{
				conn.Open();
				cmd.CommandText = $"UPDATE tasklist SET {builder} WHERE ID BETWEEN {id} AND {idend}";
				cmd.ExecuteNonQuery();
				conn.Close();
			}
		}
	}

}
