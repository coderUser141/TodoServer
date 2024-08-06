using TodoList.Server;
using System.Data.SQLite;

static class SQLiteInterface
{
	public static string ConnectionString = "Data Source=tasklist.db;Version=3;FailIfMissing=True;";

	public static int getUniqueID()
	{
		List<int> ints = new List<int>();
		int uniqueID = 0;
        using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
        {
            using (SQLiteCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "SELECT taskID FROM tasklist ORDER BY taskID ASC";
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
					ints.Add(reader.GetInt32(0));
                }
                conn.Close();
            }
        }
		if (ints.Count == 0)
		{
			uniqueID = 0;
		}
		else {
			//go through all ints from 0 -> maximum int value
			for(int i = 0; i <= ints.Max() + 1; ++i)
			{
				//if not found within range
				if(ints.Contains(i) == false)
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
				cmd.CommandText = $"INSERT INTO tasklist VALUES ({record.ID},\"{record.TaskName}\",\"{record.TaskDetails}\",\"{record.TaskCreationDate.Ticks}\",\"{record.TaskDeadline.Ticks}\");";
				cmd.ExecuteNonQuery();
				conn.Close();
			}
		}
	}

	public static IEnumerable<TaskRecord> retrieveRecords()
	{
		List<TaskRecord> records = new List<TaskRecord>();
		using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
		{
			using (SQLiteCommand cmd = conn.CreateCommand())
			{
				conn.Open();
				cmd.CommandText = "SELECT taskID,taskName, taskDetails, taskDeadline,taskCreationDate FROM tasklist; --additional stuff may be added here later for filtering";
				SQLiteDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
                {
                    records.Add(
                        new TaskRecord(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            new DateTime(reader.GetInt64(3)),
                            new DateTime(reader.GetInt64(4))
                        )
                    );
                }
				conn.Close();
			}
		}
		return records;
	}
	public static IEnumerable<TaskRecord> retrieveRecords(int id,int idend)
	{
		List<TaskRecord> records = new List<TaskRecord>();
		using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
		{
			using (SQLiteCommand cmd = conn.CreateCommand())
			{
				conn.Open();
				cmd.CommandText = $"SELECT taskID,taskName, taskDetails, taskDeadline, taskCreationDate FROM tasklist WHERE taskID BETWEEN {id} AND {idend}; --additional stuff may be added here later for filtering";
				SQLiteDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					records.Add(
						new TaskRecord(
							reader.GetInt32(0), 
							reader.GetString(1), 
							reader.GetString(2), 
							new DateTime(reader.GetInt64(3)), 
							new DateTime(reader.GetInt64(4))
						)
					);
				}
				conn.Close();
			}
		}
		return records;
	}

	public static void deleteRecords(int id, int idend) {
		using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
		{
			using (SQLiteCommand cmd = conn.CreateCommand())
			{
				conn.Open();
				cmd.CommandText = $"DELETE FROM tasklist WHERE taskID BETWEEN {id} AND {idend};";
				cmd.ExecuteNonQuery();
				conn.Close();
			}
		}
	}


}