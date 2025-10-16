namespace TodoList.Server;
public class TaskRecord
{
	private readonly long _ID;
	private readonly string _taskName;
	private readonly string _taskDetails;
	private readonly DateTime _taskDeadline;
	private readonly DateTime _taskCreationDate;
	private readonly double _taskGrade;
	private readonly double _taskWeight;
	private readonly string _taskProjectName;
	private readonly long _taskProjectID;
	private readonly string _projectCategoryName;
	private readonly long _projectCategoryID;
	private readonly string _taskCustomization;
	private readonly long _taskCompletionStatus;

	public long ID => _ID;
	public string TaskName => _taskName;
	public string TaskDetails => _taskDetails;
	public DateTime TaskDeadline => _taskDeadline;
	public DateTime TaskCreationDate => _taskCreationDate;
	public double TaskWeight => _taskWeight;
	public double TaskGrade => _taskGrade;
	public string TaskProjectName => _taskProjectName;
	public long TaskProjectID => _taskProjectID;
	public string ProjectCategoryName => _projectCategoryName;
	public long ProjectCategoryID => _projectCategoryID;
	public string TaskCustomization => _taskCustomization;
	public long TaskCompletionStatus => _taskCompletionStatus;

	public static TaskRecord validateTaskRecordInputModel(TaskRecordInputModel model)
	{

		long id = Convert.ToInt64(SQLiteInterface.getUniqueID(SQLiteInterface.SQLIDColumn.ID));

		//tpid invalid expectations: any non-integer value, anything negative that is not -1
		//-1 means "not made yet"
		long pcid = 0; long tpid = 0;


		if (Convert.ToInt64(model.InputProjectCategoryID) == -1)
		{
			pcid = SQLiteInterface.getUniqueID(SQLiteInterface.SQLIDColumn.ProjectCategoryID);
		}

		if (Convert.ToInt64(model.InputTaskProjectID) == -1)
		{
			tpid = SQLiteInterface.getUniqueID(SQLiteInterface.SQLIDColumn.ProjectID);
		}

		double grade = Convert.ToDouble(model.InputTaskGrade);

		if (grade < 0) {
			throw new ArgumentException("Grade cannot be negative");
		}

		double weight = Convert.ToDouble(model.InputTaskWeight);

		if (weight < 0) {
			throw new ArgumentException("Weight cannot be negative");
		}
		DateTime deadline;
		try
		{
			deadline = DateTime.Parse($"{model.InputTaskDeadline}");
		}
		catch
		{
			deadline = DateTime.MinValue;
		}

		//{"InputTaskName":"","InputTaskDetails":"","InputTaskDeadline":"2222-02-03T14:30","InputTaskCustomization":"","InputTaskWeight":"0","InputTaskGrade":"0","InputTaskProjectID":"-1","InputProjectCategoryID":"-1","InputTaskCompletionStatus":"101"}


			return new TaskRecord(
				ID: id,
				taskName: model.InputTaskName,
				taskDetails: model.InputTaskDetails,
				taskDeadline: deadline,
				taskCreationDate: null,
				taskGrade: grade,
				taskWeight: weight,
				taskProjectName: model.InputTaskProjectName,
				taskProjectID: tpid,
				projectCategoryName: model.InputProjectCategoryName,
				projectCategoryID: pcid,
				taskCustomization: model.InputTaskCustomization,
				taskCompletionStatus: Convert.ToInt64(model.InputTaskCompletionStatus)
			);
		


		throw new NotImplementedException();
	}

	public TaskRecord(long ID, string taskName, string taskDetails, DateTime taskDeadline, DateTime? taskCreationDate, double taskGrade, double taskWeight, string taskProjectName, long taskProjectID, string? projectCategoryName, long projectCategoryID, string taskCustomization, long taskCompletionStatus)
	{
		this._ID = ID;
		this._taskName = taskName;
		this._taskDetails = taskDetails;
		this._taskDeadline = taskDeadline;
		this._taskCreationDate = taskCreationDate ?? DateTime.Now;
		this._taskGrade = taskGrade;
		this._taskWeight = taskWeight;
		this._taskProjectName = taskProjectName ?? string.Empty;
		this._taskProjectID = taskProjectID;
		this._projectCategoryName = projectCategoryName ?? string.Empty;
		this._projectCategoryID = projectCategoryID;
		this._taskCustomization = taskCustomization;
		this._taskCompletionStatus = taskCompletionStatus;
	}
}

public class TaskRecordInputModel
{
	private readonly string _taskName;
	private readonly string _taskDetails;
	private readonly string _taskDeadline;
	private readonly string _taskCustomization;
	private readonly string _taskWeight;
	private readonly string _taskGrade;
	private readonly string _taskProjectName;
	private readonly string _taskProjectID;
	private readonly string _projectCategoryName;
	private readonly string _projectCategoryID;
	private readonly string _taskCompletionStatus;

	public string InputTaskName => _taskName;
	public string InputTaskDetails => _taskDetails;
	public string InputTaskDeadline => _taskDeadline;
	public string InputTaskCustomization => _taskCustomization;
	public string InputTaskWeight => _taskWeight;
	public string InputTaskGrade => _taskGrade;
	public string InputTaskProjectName => _taskProjectName;
	public string InputTaskProjectID => _taskProjectID;
	public string InputProjectCategoryName => _projectCategoryName;
	public string InputProjectCategoryID => _projectCategoryID;
	public string InputTaskCompletionStatus => _taskCompletionStatus;

	//constructor parameters NEED to correspond to the names found in the JSON sent over here
	public TaskRecordInputModel(string inputTaskName, string inputTaskDetails, string inputTaskDeadline, string inputTaskCustomization, string inputTaskWeight, string inputTaskGrade, string inputTaskProjectName, string inputTaskProjectID, string inputProjectCategoryName, string inputProjectCategoryID, string inputTaskCompletionStatus)
	{
		this._taskName = inputTaskName;
		this._taskDetails = inputTaskDetails;
		this._taskDeadline = inputTaskDeadline;
		this._taskCustomization = inputTaskCustomization;
		this._taskGrade = inputTaskGrade;
		this._taskWeight = inputTaskWeight;
		this._taskProjectName = inputTaskProjectName;
		this._taskProjectID = inputTaskProjectID;
		this._projectCategoryName = inputProjectCategoryName;
		this._projectCategoryID = inputProjectCategoryID;
		this._taskCompletionStatus = inputTaskCompletionStatus;
	}

}

public class TaskRecordOutputModel
{
	private readonly long _ID;
	private readonly string _taskName;
	private readonly string _taskDetails;
	private readonly string _taskDeadline;
	private readonly string _taskCreationDate;
	private readonly double _taskWeight;
	private readonly double _taskGrade;
	private readonly string _taskProjectName;
	private readonly long _taskProjectID;
	private readonly string _projectCategoryName;
	private readonly long _projectCategoryID;
	private readonly string _taskCustomization;
	private readonly long _taskCompletionStatus;
	
	public long ID => _ID;
	public string OutputTaskName => _taskName;
	public string OutputTaskDetails => _taskDetails;
	public string OutputTaskDeadline => _taskDeadline;
	public string OutputTaskCreationDate => _taskCreationDate;
	public double OutputTaskWeight => _taskWeight;
	public double OutputTaskGrade => _taskGrade;
	public string OutputTaskProjectName => _taskProjectName;
	public long OutputTaskProjectID => _taskProjectID;
	public string OutputProjectCategoryName => _projectCategoryName;
	public long OutputProjectCategoryID => _projectCategoryID;
	public string OutputTaskCustomization => _taskCustomization;
	public long OutputTaskCompletionStatus => _taskCompletionStatus;


	public TaskRecordOutputModel(TaskRecord model)
	{
		this._ID = model.ID;
		this._taskName = model.TaskName;
		this._taskDetails = model.TaskDetails;
		this._taskDeadline =
			model.TaskDeadline != DateTime.MinValue ?
				model.TaskDeadline.ToString("s") :
				""
				;
		this._taskCreationDate = 
				model.TaskCreationDate != DateTime.MinValue ?
				model.TaskCreationDate.ToString("s") :
				""
				;
		this._taskWeight = model.TaskWeight;
		this._taskGrade = model.TaskGrade;
		this._taskProjectName = model.TaskProjectName;
		this._taskProjectID = model.TaskProjectID; 
		this._projectCategoryName = model.ProjectCategoryName;
		this._projectCategoryID = model.ProjectCategoryID;
		this._taskCustomization = model.TaskCustomization;
		this._taskCompletionStatus = model.TaskCompletionStatus;
	}

}


public class TimeSyncModel
{
	private readonly int _clientOffset;
	private readonly int _serverOffset;

	public int ClientOffset => _clientOffset;
	public int ServerOffset => _serverOffset;

	public TimeSyncModel(int clientOffset, int serverOffset)
	{
		_clientOffset = clientOffset;
		_serverOffset = serverOffset;
	}
}